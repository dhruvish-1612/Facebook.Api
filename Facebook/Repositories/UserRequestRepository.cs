// <copyright file="UserRequestRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.Net;
    using Facebook.CustomException;
    using Facebook.Enums;
    using Facebook.Helpers;
    using Facebook.Hubs;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Facebook.ParameterModel;
    using Microsoft.AspNetCore.SignalR;
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
        private readonly INotificationRepository notificationRepository;
        private readonly IHubContext<FacebookHub> facebookHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRequestRepository" /> class.
        /// </summary>
        /// <param name="facebookContext">The facebook context.</param>
        /// <param name="notificationRepository">The notification repository.</param>
        /// <param name="facebookHub">The facebook hub.</param>
        public UserRequestRepository(FacebookContext facebookContext, INotificationRepository notificationRepository, IHubContext<FacebookHub> facebookHub)
        {
            this.db = facebookContext;
            this.notificationRepository = notificationRepository;
            this.facebookHub = facebookHub;
        }

        /// <summary>
        /// Gets the friendship by identifier.
        /// </summary>
        /// <param name="friendshipId">The friendship identifier.</param>
        /// <returns> get freiendship by it's id object.</returns>
        public async Task<FriendShipModel> GetFriendRequestById(long friendshipId)
        {
            Type friendshipEnumType = typeof(FrienshipEnum);
            FriendShipModel? friendshipModel = await this.db.Friendships.Where(friendship => friendship.FriendshipId.Equals(friendshipId) && friendship.DeletedAt == null).Select(
               friendships => new FriendShipModel
               {
                   FriendshipId = friendshipId,
                   AcceptedUserId = friendships.ProfileAccept,
                   RequestedUserId = friendships.ProfileRequest,
                   AcceptedUserName = $"{friendships.ProfileAcceptNavigation.FirstName} {friendships.ProfileAcceptNavigation.LastName}",
                   RequestedUserName = $"{friendships.ProfileRequestNavigation.FirstName} {friendships.ProfileRequestNavigation.LastName}",
                   AcceptedUserAvtar = friendships.ProfileAcceptNavigation.Avatar ?? string.Empty,
                   RequestedUserAvtar = friendships.ProfileRequestNavigation.Avatar ?? string.Empty,
                   Status = GetStringFromEnum.GetEnumString(friendships.Status ?? 0, friendshipEnumType),
               }).FirstOrDefaultAsync();

            if (friendshipModel == null)
            {
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "friend request not found") } };
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
                    throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.Conflict, "Already Friend Request has been Sent.") } };
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
            string userId = await this.notificationRepository.AddFriedShipNotification(acceptId, friendObj.FriendshipId, (int)FrienshipEnum.Request_Pending);
            FriendShipModel friendShip = await this.GetFriendRequestById(friendObj.FriendshipId);
            await this.facebookHub.Clients.Group(userId).SendAsync("SendNotification", friendShip);
            return friendShip;
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
            Friendship? friendship = await this.db.Friendships.FirstOrDefaultAsync(friendship => friendship.FriendshipId == friendshipId && friendship.DeletedAt == null) ?? new Friendship();
            List<ValidationsModel> validationsMessage = new();
            bool isValidFriendship = await this.ValidateFriendRequestById(friendshipId);

            if (friendship.FriendshipId == 0)
                validationsMessage.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "This Friend Request Is Not Found."));

            if (friendship.IsFriend == true && identity == 1)
                validationsMessage.Add(new ValidationsModel((int)HttpStatusCode.Conflict, "Already You Are Friends."));

            if (friendship.Status == (int)FrienshipEnum.Request_Rejected && identity == 2)
                validationsMessage.Add(new ValidationsModel((int)HttpStatusCode.Conflict, "You Already Reject The Friend Request."));

            if (identity != (int)FrienshipEnum.Request_Accepted && identity != (int)FrienshipEnum.Request_Rejected)
                validationsMessage.Add(new ValidationsModel((int)HttpStatusCode.Unauthorized, "please enter correct identity"));

            if (validationsMessage.Any())
                throw new AggregateValidationException { Validations = validationsMessage };

            friendship.Status = identity == (int)FrienshipEnum.Request_Accepted ? (int)FrienshipEnum.Request_Accepted : (int)FrienshipEnum.Request_Rejected;
            friendship.IsFriend = identity == (int)FrienshipEnum.Request_Accepted;
            this.db.Update(friendship);
            await this.db.SaveChangesAsync();
            string userId = await this.notificationRepository.AddFriedShipNotification(friendship.ProfileRequest, friendship.FriendshipId, friendship.Status ?? 0);
            FriendShipModel friendShip = await this.GetFriendRequestById(friendship.FriendshipId);
            await this.facebookHub.Clients.Group(userId).SendAsync("SendNotification", friendShip);
            return friendShip;
        }

        /// <summary>
        /// Unfollows the friends.
        /// </summary>
        /// <param name="rejectedUserId">The rejected user identifier.</param>
        /// <param name="requestRejectedUserId">The request rejected user identifier.</param>
        /// <returns>Unfollow the friend.</returns>
        /// <exception cref="Facebook.CustomException.AggregateValidationException">for validation.</exception>
        public async Task<bool> UnfollowFriends(long rejectedUserId, long requestRejectedUserId)
        {
            Friendship friendship = await this.db.Friendships.FirstOrDefaultAsync(friendship => ((friendship.ProfileAccept == rejectedUserId && friendship.ProfileRequest == requestRejectedUserId)
            || (friendship.ProfileAccept == requestRejectedUserId && friendship.ProfileRequest == rejectedUserId)) && friendship.IsFriend == true && friendship.DeletedAt == null) ?? new Friendship();

            List<ValidationsModel> errors = new();
            bool isValidAcceptedUserId = await this.ValidateUserById(rejectedUserId);
            if (!isValidAcceptedUserId)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Accepted UserId Not Exist"));

            bool isValidRequestedUserId = await this.ValidateUserById(requestRejectedUserId);
            if (!isValidRequestedUserId)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "Requested UserId Not Exist"));

            if (friendship.FriendshipId == 0)
                errors.Add(new ValidationsModel((int)HttpStatusCode.NotFound, "This Friend Request Is Not Found."));

            if (errors.Any())
                throw new AggregateValidationException { Validations = errors };

            friendship.DeletedAt = DateTime.Now;
            this.db.Update(friendship);
            await this.db.SaveChangesAsync();
            await this.notificationRepository.DeleteFriendShipNotification(friendship.FriendshipId);
            return true;
        }

        /// <summary>
        /// Gets the requested users asynchronous.
        /// </summary>
        /// <param name="userId">userid.</param>
        /// <param name="getRequestedUserParam">The get requested user parameter.</param>
        /// <returns>
        /// return userlist according pending,accepted and rejected.
        /// </returns>
        public async Task<Pagination<RequestedUsersModel>> GetRequestedUsersAsync(long userId, GetRequestedUserParam getRequestedUserParam)
        {
            await this.ValidationGetRequestedUsers(userId, getRequestedUserParam.Filter, getRequestedUserParam.RequestType);

            Type filterViaRequest = typeof(FilterViaRequestEnum);
            List<RequestedUsersModel> requestedUsers = await this.db.Friendships
           .Where(friendship => friendship.DeletedAt == null
            && ((getRequestedUserParam.Filter == (int)FilterViaRequestEnum.Is_Pending && friendship.Status == (int)FilterViaRequestEnum.Is_Pending && friendship.IsFriend == false)
               || (getRequestedUserParam.Filter == (int)FilterViaRequestEnum.Is_Rejected && friendship.Status == (int)FilterViaRequestEnum.Is_Rejected && friendship.IsFriend == false)
               || (getRequestedUserParam.Filter == (int)FrienshipEnum.Request_Accepted && friendship.Status == (int)FrienshipEnum.Request_Accepted && friendship.IsFriend == true))
            && ((getRequestedUserParam.RequestType == (int)RequestTypeEnum.Is_Sent && friendship.ProfileRequest == userId)
               || (getRequestedUserParam.RequestType == (int)RequestTypeEnum.Is_Received && friendship.ProfileAccept == userId)
               || (getRequestedUserParam.RequestType != (int)RequestTypeEnum.Is_Received && getRequestedUserParam.RequestType != (int)RequestTypeEnum.Is_Sent && (friendship.ProfileAccept == userId || friendship.ProfileRequest == userId))))
           .Select(friendship => new RequestedUsersModel
           {
               FriendshipId = friendship.FriendshipId,
               RequestType = userId == friendship.ProfileAccept ? "Request Received" : userId == friendship.ProfileRequest ? "Request Sent" : "Both The Requests",
               RequestStatus = GetStringFromEnum.GetEnumString(friendship.Status ?? 0, filterViaRequest),
           }).ToListAsync();

            List<RequestedUsersModel> paginatedRequestedUsers = requestedUsers.Skip((getRequestedUserParam.PaginationParams.PageNumber - 1) * getRequestedUserParam.PaginationParams.PageSize)
                                                                .Take(getRequestedUserParam.PaginationParams.PageSize).ToList();

            return new Pagination<RequestedUsersModel>(paginatedRequestedUsers, paginatedRequestedUsers.Count, requestedUsers.Count);
        }

        /// <summary>
        /// Gets the suggessted friend.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// resturn suggested friend.
        /// </returns>
        public async Task<Pagination<GetFriendsModel>> GetSuggestedFriend(long userId, PaginationParams paginationParams)
        {
            List<GetFriendsModel> users = await this.db.Users.Where(user => user.UserId != userId)
                                          .Select(user => new GetFriendsModel
                                          {
                                              UserId = user.UserId,
                                              UserName = $"{user.FirstName} {user.LastName}",
                                              Avatar = user.Avatar,
                                          }).ToListAsync();

            List<long> suggestedFriend = await this.db.Friendships
                        .Where(friend => friend.DeletedAt == null && (friend.ProfileAccept == userId || friend.ProfileRequest == userId) && friend.IsFriend == true)
                        .Select(x => x.ProfileRequest == userId ? x.ProfileAcceptNavigation.UserId : x.ProfileRequestNavigation.UserId).Where(id => id != userId).ToListAsync();

            List<long> friends = await this.db.Friendships
                        .Where(friend => friend.DeletedAt == null && (suggestedFriend.Contains(friend.ProfileAccept) || suggestedFriend.Contains(friend.ProfileRequest)) && friend.IsFriend == true)
                        .Select(x => suggestedFriend.Contains(x.ProfileRequest) ? x.ProfileAcceptNavigation.UserId : x.ProfileRequestNavigation.UserId)
                        .ToListAsync();

            friends = friends.Where(x => x != userId && !suggestedFriend.Contains(x)).ToList();

            users = users.Where(x => !suggestedFriend.Contains(x.UserId)).OrderByDescending(sequence => friends.IndexOf(sequence.UserId)).ToList();
            List<GetFriendsModel> paginatedUsers = users.Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).ToList();
            return new Pagination<GetFriendsModel>(paginatedUsers, paginatedUsers.Count, users.Count);
        }

        /// <summary>
        /// Gets the mutual friends.
        /// </summary>
        /// <param name="loginUserId">The login user identifier.</param>
        /// <param name="friendParam">The friend parameter.</param>
        /// <returns>
        /// GetMutualFriends.
        /// </returns>
        public async Task<Pagination<GetFriendsModel>> GetMutualFriends(long loginUserId, GetNotificationParam friendParam)
        {
            List<GetFriendsModel> getLoginUserFriend = await this.db.Friendships.Where(friend => friend.DeletedAt == null && friend.IsFriend == true
                                                      && (friend.ProfileRequest == loginUserId || friend.ProfileAccept == loginUserId))
                                                       .Select(x => x.ProfileRequest == loginUserId ?
                                                       new GetFriendsModel
                                                       {
                                                           UserId = x.ProfileAcceptNavigation.UserId,
                                                           UserName = $"{x.ProfileAcceptNavigation.FirstName} {x.ProfileAcceptNavigation.LastName}",
                                                           Avatar = x.ProfileAcceptNavigation.Avatar,
                                                       }
                                                       : new GetFriendsModel
                                                       {
                                                           UserId = x.ProfileRequestNavigation.UserId,
                                                           UserName = $"{x.ProfileRequestNavigation.FirstName} {x.ProfileRequestNavigation.LastName}",
                                                           Avatar = x.ProfileRequestNavigation.Avatar,
                                                       }).ToListAsync();

            List<long> getFriendUserFriends = await this.db.Friendships.Where(friend => friend.DeletedAt == null && friend.IsFriend == true
                                                      && (friend.ProfileRequest == friendParam.UserId || friend.ProfileAccept == friendParam.UserId))
                                                       .Select(x => x.ProfileRequest == friendParam.UserId ? x.ProfileAccept : x.ProfileRequest).ToListAsync();
            List<GetFriendsModel> getMutualFriends = getLoginUserFriend.Where(x => getFriendUserFriends.Contains(x.UserId)).ToList();
            List<GetFriendsModel> getPaginatedMutualFriends = getMutualFriends.Skip((friendParam.PaginationParams.PageNumber - 1) * friendParam.PaginationParams.PageSize)
                                                                              .Take(friendParam.PaginationParams.PageSize).ToList();

            return new Pagination<GetFriendsModel>(getPaginatedMutualFriends, getPaginatedMutualFriends.Count, getMutualFriends.Count);
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
            bool isFriendRequestExist = await this.db.Friendships.AnyAsync(check => id > 0 && check.FriendshipId == id && check.DeletedAt == null);
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
            bool isValidUserId = await this.db.Users.AnyAsync(checkUser => checkUser.UserId == userId && checkUser.DeletedAt == null);
            return isValidUserId;
        }
    }
}
