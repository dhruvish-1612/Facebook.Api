// <copyright file="INotificationRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Interface
{
    using Facebook.Model;
    using Facebook.ParameterModel;

    /// <summary>
    /// INotificationRepository.
    /// </summary>
    public interface INotificationRepository
    {
        /// <summary>
        /// Adds the comment notification.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns>
        /// Adding Notification for Comments.
        /// </returns>
        Task<string> AddCommentNotification(long commentId);

        /// <summary>
        /// Adds the fried ship notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="friedId">The fried identifier.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>nothing.</returns>
        Task<string> AddFriedShipNotification(long userId, long friedId, int identity);

        /// <summary>
        /// Adds the like notification.
        /// </summary>
        /// <param name="likeId">The like identifier.</param>
        /// <returns>nothing.</returns>
        Task<string> AddLikeNotification(long likeId);

        /// <summary>
        /// Adds the notifications.
        /// </summary>
        /// <param name="models">The model.</param>
        /// <returns>nothing.</returns>
        Task AddNotifications(List<NotificationModel> models);

        /// <summary>
        /// Adds the post notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="postId">The post identifier.</param>
        /// <returns>Adding Notification for Post.</returns>
        Task<List<string>> AddPostNotification(long userId, long postId);

        /// <summary>
        /// Adds the story notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>nothing.</returns>
        Task<List<string>> AddStoryNotification(long userId, long storyId);

        /// <summary>
        /// Deletes the friend ship notification.
        /// </summary>
        /// <param name="friendShipid">The friend shipid.</param>
        /// <returns>Delete Friend Notification.</returns>
        Task DeleteFriendShipNotification(long friendShipid);

        /// <summary>
        /// Deletes the notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns>true if succesfully deleted.</returns>
        Task<bool> DeleteNotification(long notificationId);

        /// <summary>
        /// Deletes the notifications.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>true if succesfully deleted.</returns>
        Task<bool> DeleteNotifications(long userId);

        /// <summary>
        /// Deletes the post or story.
        /// </summary>
        /// <param name="activityId">The activity identifier.</param>
        /// <param name="activityType">Type of the activity.</param>
        /// <returns>nothing.</returns>
        Task DeletePostOrStory(long activityId, int activityType);

        /// <summary>
        /// Gets the user notifications.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="paginationParams">The pagination parameters.</param>
        /// <returns>
        /// get Users Notifications.
        /// </returns>
        Task<Pagination<GetNotificationsModel>> GetUserNotifications(long userId, PaginationParams paginationParams);

        /// <summary>
        /// Reads the notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns>true if read.</returns>
        Task<bool> ReadNotification(long userId, long notificationId);
    }
}
