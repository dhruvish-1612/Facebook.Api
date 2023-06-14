// <copyright file="UserRequestRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.Net;
    using Facebook.CustomException;
    using Facebook.Enums;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Contaning method Logic user request related.
    /// </summary>
    /// <seealso cref="IUserRequest" />
    public class UserRequestRepository : IUserRequestRepository
    {
        /// <summary>
        /// The facebook context.
        /// </summary>
        private readonly FacebookContext db;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRequestRepository"/> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        /// <param name="userRepository">The user repository.</param>
        public UserRequestRepository(FacebookContext facebookContext)
        {
            this.db = facebookContext;
        }

        /// <summary>
        /// Gets the friendship by identifier.
        /// </summary>
        /// <param name="friendshipId">The friendship identifier.</param>
        /// <returns> get freiendship by it's id object.</returns>
        public async Task<FriendShipModel> GetFriendRequestById(long friendshipId)
        {
            List<ValidationsModel> errors = new();
            FriendShipModel? friendshipModel = await this.db.Friendships.Where(friendship => friendship.FriendshipId.Equals(friendshipId)).Select(
               friendships => new FriendShipModel
               {
                   FriendshipId = friendshipId,
                   AcceptedUserId = friendships.ProfileAccept,
                   RequestedUserId = friendships.ProfileRequest,
                   AcceptedUserName = $"{friendships.ProfileAcceptNavigation.FirstName} {friendships.ProfileAcceptNavigation.LastName}",
                   RequestedUserName = $"{friendships.ProfileRequestNavigation.FirstName} {friendships.ProfileRequestNavigation.LastName}",
                   AcceptedUserAvtar = friendships.ProfileAcceptNavigation.Avatar ?? string.Empty,
                   RequestedUserAvtar = friendships.ProfileRequestNavigation.Avatar ?? string.Empty,
                   Status = friendships.Status == (int)FrienshipEnum.Request_Pending ? "Your Request Is Pending" :
                            friendships.Status == (int)FrienshipEnum.Request_Accepted ? "You Accept The Request" :
                            friendships.Status == (int)FrienshipEnum.Request_Rejected ? "You Reject The Request" :
                            "Please Enter Valid Input",
               }).FirstOrDefaultAsync();

            if (friendshipModel == null)
            {
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "friend request not found"));
                throw new AggregateValidationException { Validations = errors };
            }

            return friendshipModel;
        }

        /// <summary>
        /// Sends the freind request.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="acceptId">The accept identifier.</param>
        /// <returns> if request send successfully then return true else return false.</returns>
        public async Task<FriendShipModel> SendFreindRequest(long requestId, long acceptId)
        {
            List<ValidationsModel> errors = new();
            await this.ValidationSendFreindRequest(requestId, acceptId);
            Friendship friendObj = await this.db.Friendships.FirstOrDefaultAsync(friend => ((friend.ProfileRequest == requestId && friend.ProfileAccept == acceptId) || (friend.ProfileAccept == requestId && friend.ProfileRequest == acceptId))
                                    && friend.DeletedAt == null) ?? new Friendship();
            if (friendObj.FriendshipId == 0)
            {
                friendObj.ProfileAccept = acceptId;
                friendObj.ProfileRequest = requestId;
                friendObj.Status = (int)FrienshipEnum.Request_Pending;
                this.db.Friendships.Add(friendObj);
            }
            else
            {
                if (friendObj.Status == (int)FrienshipEnum.Request_Accepted || friendObj.Status == (int)FrienshipEnum.Request_Pending)
                {
                    errors.Add(new ValidationsModel((int)HttpStatusCode.Conflict, "Already Friend Request has been Sent."));
                    throw new AggregateValidationException { Validations = errors };
                }
                else
                {
                    friendObj.ProfileAccept = acceptId;
                    friendObj.ProfileRequest = requestId;
                    friendObj.Status = (int)FrienshipEnum.Request_Pending;
                    this.db.Friendships.Update(friendObj);
                }
            }

            await this.db.SaveChangesAsync();
            return await this.GetFriendRequestById(friendObj.FriendshipId);
        }

        /// <summary>
        /// Validations the send freind request.
        /// </summary>
        /// <param name="requestUserId">The request user identifier.</param>
        /// <param name="acceptUserId">The accept user identifier.</param>
        /// <returns>
        /// List Of validations if occur in sending friend request.
        /// </returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">throw an exception.</exception>
        public async Task ValidationSendFreindRequest(long requestUserId, long acceptUserId)
        {
            List<ValidationsModel> errors = new();

            if ((acceptUserId == 0 || requestUserId == 0) || acceptUserId == requestUserId)
                errors.Add(new ValidationsModel((int)HttpStatusCode.Conflict, "You Enter Wrong Accepted and Requested UserId or same Ids"));

            bool isValidAcceptedUserId = await this.ValidateUserById(acceptUserId);
            if (!isValidAcceptedUserId)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Accepted UserId Not Exist"));

            bool isValidRequestedUserId = await this.ValidateUserById(requestUserId);
            if (!isValidRequestedUserId)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Requested UserId Not Exist"));

            // check already both are friends
            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };
        }

        /// <summary>
        /// Approves the or reject request.
        /// </summary>
        /// <param name="friendshipId">The friendship identifier.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>return true friend request accepted otherwise return false.</returns>
        public async Task<FriendShipModel> ApproveOrRejectRequest(long friendshipId, int identity)
        {
            Friendship? friendship = await this.db.Friendships.FirstOrDefaultAsync(friendship => friendship.FriendshipId == friendshipId) ?? new Friendship();
            List<ValidationsModel> validationsMessage = new();
            bool isValidFriendship = await this.ValidateFriendRequestById(friendshipId);

            if (friendship.FriendshipId == 0)
                validationsMessage.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "This Friend Request Is Not Found."));

            if (identity != (int)FrienshipEnum.Request_Accepted && identity != (int)FrienshipEnum.Request_Rejected)
                validationsMessage.Add(new ValidationsModel((int)HttpStatusCode.Unauthorized, "please enter correct identity"));

            if (validationsMessage.Any())
                throw new AggregateValidationException { Validations = validationsMessage };

            friendship.Status = identity == (int)FrienshipEnum.Request_Accepted ? (int)FrienshipEnum.Request_Accepted : (int)FrienshipEnum.Request_Rejected;
            friendship.IsFriend = identity == (int)FrienshipEnum.Request_Accepted;
            this.db.Update(friendship);
            await this.db.SaveChangesAsync();
            return await this.GetFriendRequestById(friendship.FriendshipId);
        }

        /// <summary>
        /// Gets the requested users asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="requestStatus">the requestStatus identifier.</param>
        /// <param name="requestType">requestStatus using user sent or receive.</param>
        /// <returns>
        /// return userlist according pending,accepted and rejected.
        /// </returns>
        public async Task<List<RequestedUsersModel>> GetRequestedUsersAsync(long userId, int requestStatus, int requestType)
        {
            await this.ValidationGetRequestedUsers(userId, requestStatus, requestType);
            List<RequestedUsersModel> requestedUsers = await this.db.Friendships
           .Where(friendship => friendship.DeletedAt == null
            && ((requestStatus == (int)FilterViaRequestEnum.Is_Pending && friendship.Status == (int)FilterViaRequestEnum.Is_Pending && friendship.IsFriend == false)
               || (requestStatus == (int)FilterViaRequestEnum.Is_Rejected && friendship.Status == (int)FilterViaRequestEnum.Is_Rejected && friendship.IsFriend == false)
               || (requestStatus == (int)FrienshipEnum.Request_Accepted && friendship.Status == (int)FrienshipEnum.Request_Accepted && friendship.IsFriend == true))
            && ((requestType == (int)RequestTypeEnum.Is_Sent && friendship.ProfileRequest == userId)
               || (requestType == (int)RequestTypeEnum.Is_Received && friendship.ProfileAccept == userId)
               || (requestType != (int)RequestTypeEnum.Is_Received && requestType != (int)RequestTypeEnum.Is_Sent && (friendship.ProfileAccept == userId || friendship.ProfileRequest == userId))))
           .Select(friendship => new RequestedUsersModel
           {
               FriendshipId = friendship.FriendshipId,
               RequestType = userId == friendship.ProfileAccept ? "Request Received" : userId == friendship.ProfileRequest ? "Request Sent" : "Both The Requests",
               RequestStatus = requestStatus == (int)FilterViaRequestEnum.Is_Pending ? "Your Request Is Pending" : requestStatus == (int)FilterViaRequestEnum.Is_Rejected ? "You Reject the Request" : "Your Accepted Request",
           }).ToListAsync();

            return requestedUsers ?? new List<RequestedUsersModel>();
        }

        /// <summary>
        /// Validations the get requested users.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="requestStatus">The request status.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <returns>
        /// List of validatoins if occur in getting list of requested users.
        /// </returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">if any errors occurs then exception will be throw.</exception>
        public async Task ValidationGetRequestedUsers(long userId, int requestStatus, int requestType)
        {
            List<ValidationsModel> errors = new();
            bool isValidUser = await this.ValidateUserById(userId);
            if (!isValidUser)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found"));

            if (requestStatus != (int)FrienshipEnum.Request_Accepted && requestStatus != (int)FrienshipEnum.Request_Rejected && requestStatus != (int)FrienshipEnum.Request_Pending)
                errors.Add(new ValidationsModel((int)HttpStatusCode.Unauthorized, $"please enter {(int)FrienshipEnum.Request_Pending} for check pending request and {(int)FrienshipEnum.Request_Accepted} for your accepted request  and {(int)FrienshipEnum.Request_Pending} for your requested request"));

            if (requestType != 0 && requestType != (int)RequestTypeEnum.Is_Received && requestType != (int)RequestTypeEnum.Is_Sent)
                errors.Add(new ValidationsModel((int)HttpStatusCode.Unauthorized, $"please enter {(int)RequestTypeEnum.Is_Received} for check received request and {RequestTypeEnum.Is_Sent} for your sent request"));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };
        }

        /// <summary>Validates the friend request by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>validation for get the friend request id.<returns>
        public async Task<bool> ValidateFriendRequestById(long id)
        {
            bool isFriendRequestExist = await Task.Run(() => this.db.Friendships.AnyAsync(check => id > 0 && check.FriendshipId == id && check.DeletedAt == null));
            return isFriendRequestExist;
        }

        /// <summary>
        /// Validates the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// return true if two ids are match otherwise false.
        /// </returns>
        public async Task<bool> ValidateUserById(long userId)
        {
            bool isValidUserId = await Task.Run(() => this.db.Users.AnyAsync(checkUser => checkUser.UserId == userId));
            return isValidUserId;
        }
    }
}
