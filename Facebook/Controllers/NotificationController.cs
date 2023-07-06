// <copyright file="NotificationController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Controllers
{
    using Facebook.Constant;
    using Facebook.CustomException;
    using Facebook.Helpers;
    using Facebook.Interface;
    using Facebook.ParameterModel;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// NotificationController.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("[controller]")]

    [Authorize(Roles = AccessRoleConstant.UserRole)]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository notificationRepository;
        private readonly GetUserId getUserId;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationController" /> class.
        /// </summary>
        /// <param name="notificationRepository">The notification repository.</param>
        /// <param name="getUserId">The get user identifier.</param>
        public NotificationController(INotificationRepository notificationRepository, GetUserId getUserId)
        {
            this.notificationRepository = notificationRepository;
            this.getUserId = getUserId;
        }

        /// <summary>
        /// Gets the user notification.
        /// </summary>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get notification for that users.
        /// </returns>
        [HttpGet("GetUserNotifications")]
        public async Task<IActionResult> GetUserNotifications([FromQuery] PaginationParams paginationParams)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.notificationRepository.GetUserNotifications(userId, paginationParams));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Deletes the notifications.
        /// </summary>
        /// <param name="notificationIds">The notification ids.</param>
        /// <returns>
        /// true if delete notifications.
        /// </returns>
        [HttpPost("DeleteNotifications")]
        public async Task<IActionResult> DeleteNotifications()
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.notificationRepository.DeleteNotifications(userId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Deletes the notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns>true if succesfully deleted.</returns>
        [HttpPost("DeleteNotification")]
        public async Task<IActionResult> DeleteNotification(long notificationId)
        {
            try
            {
                return this.Ok(await this.notificationRepository.DeleteNotification(notificationId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }

        /// <summary>
        /// Deletes the notifications.
        /// </summary>
        /// <param name="notificationId">The notification ids.</param>
        /// <returns>
        /// true if delete notifications.
        /// </returns>
        [HttpPost("ReadNotification")]
        public async Task<IActionResult> ReadNotification([FromBody] long notificationId)
        {
            try
            {
                long userId = this.getUserId.GetLoginUserId();
                return this.Ok(await this.notificationRepository.ReadNotification(userId, notificationId));
            }
            catch (AggregateValidationException ex)
            {
                return this.BadRequest(ex.Validations);
            }
        }
    }
}
