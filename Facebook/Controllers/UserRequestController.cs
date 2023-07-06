// <copyright file="UserRequestController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Controllers
{
    using System.Net;
    using Facebook.Constant;
    using Facebook.CustomException;
    using Facebook.Helpers;
    using Facebook.Interface;
    using Facebook.Model;
    using Facebook.ParameterModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// controller user friendship.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Authorize(Roles = AccessRoleConstant.UserRole)]
    [Route("[controller]")]
    public class UserRequestController : ControllerBase
    {
        /// <summary>
        /// The user request repository.
        /// </summary>
        private readonly IUserRequestRepository userRequestRepository;
        private readonly GetUserId getUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRequestController" /> class.
        /// </summary>
        /// <param name="userRequestRepository">The user request repository.</param>
        /// <param name="getUserId">The get user identifier.</param>
        public UserRequestController(IUserRequestRepository userRequestRepository, GetUserId getUserId)
        {
            this.userRequestRepository = userRequestRepository;
            this.getUserId = getUserId;
        }

        /// <summary>Gets the friend request by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   get FriendShip object.
        /// </returns>
        [HttpGet("GetFriendById/{id}")]
        public async Task<IActionResult> GetFriendRequestById(long id)
        {
            try
            {
                return this.Ok(await this.userRequestRepository.GetFriendRequestById(id));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Sends the freind request.
        /// </summary>
        /// <param name="toUserId">The accept identifier.</param>
        /// <returns>
        /// return requested freindship object.
        /// </returns>
        [HttpPost("SendFriendRequest")]
        public async Task<IActionResult> SendFreindRequest(long toUserId)
        {
            try
            {
                long requestId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userRequestRepository.SendFreindRequest(requestId, toUserId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Approves the or reject.
        /// </summary>
        /// <param name="friendshipId">The friendship identifier.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>return aprroved disapproved freindship object.</returns>
        // [HttpPut]
        [HttpPost("ApproveOrRejectRequest")]
        public async Task<IActionResult> ApproveOrReject(long friendshipId, int identity)
        {
            try
            {
                return this.Ok(await this.userRequestRepository.ApproveOrRejectRequest(friendshipId, identity));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Unfollows the friend.
        /// </summary>
        /// <param name="requestRejectedUserId">The request rejected user identifier.</param>
        /// <returns>
        /// true if successfully unfollow friends.
        /// </returns>
        // [HttpPut]
        [HttpPost("UnfollowFriend")]
        public async Task<IActionResult> UnfollowFriend(long requestRejectedUserId)
        {
            try
            {
                long rejectedUserId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userRequestRepository.UnfollowFriends(rejectedUserId, requestRejectedUserId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the reques dted users.
        /// </summary>
        /// <param name="getRequestedUserParam">The get requested user parameter.</param>
        /// <returns>
        /// GetRequestedUsers.
        /// </returns>
        [HttpPost("GetRequestedUsers")]
        public async Task<IActionResult> GetRequestedUsers([FromBody] GetRequestedUserParam getRequestedUserParam)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userRequestRepository.GetRequestedUsersAsync(userId, getRequestedUserParam));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the suggested friend.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// GetSuggestedFriend.
        /// </returns>
        [HttpPost("GetSuggestedFriends")]
        public async Task<IActionResult> GetSuggestedFriend([FromBody]PaginationParams paginationParams)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userRequestRepository.GetSuggestedFriend(userId,paginationParams));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Gets the mutual friends.
        /// </summary>
        /// <param name="friendParam">The friend parameter.</param>
        /// <returns>
        /// GetMutualFriends.
        /// </returns>
        [HttpPost("GetMutualFriends")]
        public async Task<IActionResult> GetMutualFriends([FromBody]GetNotificationParam friendParam)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.userRequestRepository.GetMutualFriends(userId, friendParam));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
