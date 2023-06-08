// <copyright file="IUserRequestRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Model;

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
        /// <param name="filter">The filter.</param>
        /// <param name="requestType">Type of the request.</param>
        /// <returns>
        /// sdf.
        /// </returns>
        Task<List<RequestedUsersModel>> GetRequestedUsersAsync(long userId, int filter, int requestType);

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
        /// <returns></returns>
        Task<FriendShipModel> SendFreindRequest(long requestId, long acceptId);
        Task<bool> ValidateUserById(long userId);
    }
}
