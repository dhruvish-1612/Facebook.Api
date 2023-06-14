// <copyright file="UserRequestController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Controllers
{
    using System.Net;
    using Facebook.Constant;
    using Facebook.CustomException;
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
    [Route("[controller]/[action]")]
    [ApiController]
    public class UserRequestController : ControllerBase
    {
        /// <summary>
        /// The user request repository.
        /// </summary>
        private readonly IUserRequestRepository userRequestRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRequestController"/> class.
        /// </summary>
        /// <param name="userRequestRepository">The user request repository.</param>
        public UserRequestController(IUserRequestRepository userRequestRepository) => this.userRequestRepository = userRequestRepository;

        /// <summary>Gets the friend request by identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        ///   get FriendShip object.
        /// </returns>
        [HttpGet]
        [AllowAnonymous]
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
        /// <param name="requestId">The request identifier.</param>
        /// <param name="acceptId">The accept identifier.</param>
        /// <returns>return requested freindship object.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendFreindRequest(long requestId, long acceptId)
        {
            try
            {
                return this.Ok(await this.userRequestRepository.SendFreindRequest(requestId, acceptId));
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
        [HttpPut]
        [AllowAnonymous]
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
        /// Gets the reques dted users.
        /// </summary>
        /// <param name="getRequestedUserParam">The get requested user parameter.</param>
        /// <returns>
        /// GetRequestedUsers.
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetRequestedUsers(GetRequestedUserParam getRequestedUserParam)
        {
            try
            {
                return this.Ok(await this.userRequestRepository.GetRequestedUsersAsync(getRequestedUserParam.UserId, getRequestedUserParam.Filter, getRequestedUserParam.RequestType));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
