// <copyright file="IUserRequestRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using System.Collections.Generic;
    using Facebook.Model;
    using Facebook.ParameterModel;

    /// <summary>
    /// Interface for containing user request related methods.
    /// </summary>
    public interface IUserRequestRepository
    {
        /// <summary>
        /// Approves the or reject request.
        /// </summary>
        /// <param name="friendshipId">The friendship identifier.</param>
        /// <param name="identity">The identity.</param>
        /// <returns> method for Approve Or RejectRequest.</returns>
        Task<FriendShipModel> ApproveOrRejectRequest(long friendshipId, int identity);

        /// <summary>
        /// Gets the requested users asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="getRequestedUserParam">The get requested user parameter.</param>
        /// <returns>
        /// sdf.
        /// </returns>
        Task<Pagination<RequestedUsersModel>> GetRequestedUsersAsync(long userId, GetRequestedUserParam getRequestedUserParam);

        /// <summary>Validates the friend request by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   validation for get friend request by id.
        /// </returns>
        Task<bool> ValidateFriendRequestById(long id);

        /// <summary>
        /// Gets the friendship by identifier.
        /// </summary>
        /// <param name="friendshipId">The friendship identifier.</param>
        /// <returns> method for GetFriendshipById. </returns>
        Task<FriendShipModel> GetFriendRequestById(long friendshipId);

        /// <summary>
        /// Sends the freind request.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="acceptId">The accept identifier.</param>
        /// <returns>return friendship object.</returns>
        Task<FriendShipModel> SendFreindRequest(long requestId, long acceptId);

        /// <summary>
        /// Validates the user by identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if valid user found o/w false.</returns>
        Task<bool> ValidateUserById(long userId);

        /// <summary>
        /// Unfollows the friends.
        /// </summary>
        /// <param name="rejectedUserId">The rejected user identifier.</param>
        /// <param name="requestRejectedUserId">The request rejected user identifier.</param>
        /// <returns>true if unfollow.</returns>
        Task<bool> UnfollowFriends(long rejectedUserId, long requestRejectedUserId);

        /// <summary>
        /// Gets the suggessted friend.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// resturn suggested friend.
        /// </returns>
        Task<Pagination<GetFriendsModel>> GetSuggestedFriend(long userId, PaginationParams paginationParams);

        /// <summary>
        /// Gets the mutual friends.
        /// </summary>
        /// <param name="loginUserId">The login user identifier.</param>
        /// <param name="friendParam">The friend parameter.</param>
        /// <returns>
        /// GetMutualFriends.
        /// </returns>
        Task<Pagination<GetFriendsModel>> GetMutualFriends(long loginUserId, GetNotificationParam friendParam);
    }
}
