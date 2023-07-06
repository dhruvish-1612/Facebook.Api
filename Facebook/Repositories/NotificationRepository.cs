// <copyright file="NotificationRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Repositories
{
    using System.Net;
    using AutoMapper;
    using Facebook.CustomException;
    using Facebook.Enums;
    using Facebook.Helpers;
    using Facebook.Hubs;
    using Facebook.Infrastructure.Infrastructure;
    using Facebook.Interface;
    using Facebook.Model;
    using Facebook.ParameterModel;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// INotificationRepository.
    /// </summary>
    /// <seealso cref="Facebook.Interface.INotificationRepository" />
    public class NotificationRepository : INotificationRepository
    {
        private readonly FacebookContext db;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRepository" /> class.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="facebookHub">The facebook hub.</param>
        public NotificationRepository(FacebookContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        /// <summary>
        /// Adds the notifications.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>nothing.</returns>
        public async Task AddNotifications(List<NotificationModel> model)
        {
            try
            {
                List<Notification> notifications = this.mapper.Map<List<Notification>>(model);
                await this.db.Notifications.AddRangeAsync(notifications);
                await this.db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.InternalServerError, "Internal Error Occured While Saving The Notification.") } };
            }
        }

        /// <summary>
        /// Posts the notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="postId">The post identifier.</param>
        /// <returns>nothing.</returns>
        public async Task<List<string>> AddPostNotification(long userId, long postId)
        {
            List<long> toUserIds = await this.db.Friendships.Where(u => (u.ProfileAccept == userId || u.ProfileRequest == userId) && u.IsFriend == true)
                .Select(friend => friend.ProfileAccept == userId ? friend.ProfileRequest : friend.ProfileAccept).ToListAsync();

            List<NotificationModel> postNotifications = toUserIds.Select(u => new NotificationModel
            {
                UserId = u,
                ActivityId = postId,
                ActivityType = (int)NotificationTypeEnum.AddPost,
            }).ToList();

            await this.AddNotifications(postNotifications);
            return toUserIds.Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// Deletes the post or story.
        /// </summary>
        /// <param name="activityId">The activity identifier.</param>
        /// <param name="activityType">Type of the activity.</param>
        /// <returns>DeletePostOrStory.</returns>
        public async Task DeletePostOrStory(long activityId, int activityType)
        {
            Notification? notification = await this.db.Notifications.FirstOrDefaultAsync(x => x.ActivityId.Equals(activityId) && x.ActivityType == activityType);
            if (notification == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "Notification NotFound.") } };

            notification.DeletedAt = DateTime.Now;
            this.db.Notifications.Update(notification);
            await this.db.SaveChangesAsync();
        }

        /// <summary>
        /// Adds the story notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="storyId">The story identifier.</param>
        /// <returns>nothing.</returns>
        public async Task<List<string>> AddStoryNotification(long userId, long storyId)
        {
            List<long> toUserIds = await this.db.Friendships.Where(u => (u.ProfileAccept == userId || u.ProfileRequest == userId) && u.IsFriend == true)
                .Select(friend => friend.ProfileAccept == userId ? friend.ProfileRequest : friend.ProfileAccept).ToListAsync();

            List<NotificationModel> storyNotifications = toUserIds.Select(u => new NotificationModel
            {
                UserId = u,
                ActivityId = storyId,
                ActivityType = (int)NotificationTypeEnum.AddStory,
            }).ToList();

            await this.AddNotifications(storyNotifications);
            return toUserIds.Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// Adds the comment notification.
        /// </summary>
        /// <param name="commentId">The comment identifier.</param>
        /// <returns>
        /// nothings.
        /// </returns>
        public async Task<string> AddCommentNotification(long commentId)
        {
            long userId = await this.db.PostComments.Where(comment => comment.UserPostCommentId == commentId && comment.UserId != comment.UserPost.UserId && comment.DeletedAt == null).Select(user => user.UserPost.UserId).FirstOrDefaultAsync();
            if (userId != 0)
            {
                List<NotificationModel> commentNotifications = new();
                commentNotifications.Add(new NotificationModel
                {
                    UserId = userId,
                    ActivityId = commentId,
                    ActivityType = (int)NotificationTypeEnum.AddComment,
                });

                await this.AddNotifications(commentNotifications);
            }

            return userId.ToString();
        }

        /// <summary>
        /// Adds the like notification.
        /// </summary>
        /// <param name="likeId">The comment identifier.</param>
        /// <returns>
        /// nothing.
        /// </returns>
        public async Task<string> AddLikeNotification(long likeId)
        {
            LikeParam like = await this.db.PostLikes.Where(likes => likes.UserPostLikeId == likeId).Select(user => new LikeParam
            {
                PostUserId = user.UserPost.UserId,
                LikeUserId = user.LikeUserId,
                IsLiked = user.LikeStatus,
            }).FirstOrDefaultAsync() ?? new LikeParam();

            if (like.LikeUserId != like.PostUserId)
            {
                if (like.IsLiked == true)
                {
                    List<NotificationModel> likeNotifications = new()
                    {
                        new NotificationModel
                        {
                            UserId = like.PostUserId,
                            ActivityId = likeId,
                            ActivityType = (int)NotificationTypeEnum.Like,
                        },
                    };
                    await this.AddNotifications(likeNotifications);
                }
                else if (like.IsLiked == false)
                {
                    Notification notification = await this.db.Notifications.FirstOrDefaultAsync(x => x.ActivityId == likeId && x.ActivityType == (int)NotificationTypeEnum.Like && x.DeletedAt == null) ?? new Notification();
                    notification.DeletedAt = DateTime.Now;
                    this.db.Notifications.Update(notification);
                    await this.db.SaveChangesAsync();
                }
            }
            return like.PostUserId.ToString();
        }

        /// <summary>
        /// Adds the fried ship notification.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="friendId">The friend identifier.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>nothing.</returns>
        public async Task<string> AddFriedShipNotification(long userId, long friendId, int identity)
        {
            List<NotificationModel> friendNotifications = new();
            switch (identity)
            {
                case (int)FrienshipEnum.Request_Pending:
                    friendNotifications.Add(new NotificationModel
                    {
                        UserId = userId,
                        ActivityId = friendId,
                        ActivityType = (int)NotificationTypeEnum.ReceiveFriendRequest,
                    });
                    break;

                case (int)FrienshipEnum.Request_Accepted:
                    friendNotifications.Add(new NotificationModel
                    {
                        UserId = userId,
                        ActivityId = friendId,
                        ActivityType = (int)NotificationTypeEnum.AcceptFriendRequest,
                    });
                    break;

                case (int)FrienshipEnum.Request_Rejected:
                    friendNotifications.Add(new NotificationModel
                    {
                        UserId = userId,
                        ActivityId = friendId,
                        ActivityType = (int)NotificationTypeEnum.RejectFriendRequest,
                    });
                    break;
            }

            await this.AddNotifications(friendNotifications);
            return userId.ToString();
        }

        /// <summary>
        /// Gets the user notifications.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="paginationParam">The notifcation parameters.</param>
        /// <returns>
        /// notications for login users.
        /// </returns>
        public async Task<Pagination<GetNotificationsModel>> GetUserNotifications(long userId, PaginationParams paginationParam)
        {
            if (!await this.db.Users.AnyAsync(checkUser => checkUser.UserId == userId))
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "User Not Found.") } };

            Type notificationtype = typeof(NotificationTypeEnum);
            List<GetNotificationsModel> notifications = await this.db.Notifications
                .Where(notification => (notification.UserId == userId && notification.DeletedAt == null))
                .Select(notification => new GetNotificationsModel
                {
                    UserId = userId,
                    NotificationId = notification.NotificationId,
                    ActivityId = notification.ActivityId,
                    ActivityType = notification.ActivityType,
                    IsRead = notification.IsRead,
                    CreatedAt = notification.CreatedAt,
                    EnumTypeMessage = GetStringFromEnum.GetEnumString(notification.ActivityType, notificationtype),
                })
                .OrderByDescending(notification => notification.CreatedAt)
                .ToListAsync();
            List<GetNotificationsModel> getPaginatedAllNotifications = notifications.Skip((paginationParam.PageNumber - 1) * paginationParam.PageSize).Take(paginationParam.PageSize).ToList();

            return new Pagination<GetNotificationsModel>(
                getPaginatedAllNotifications,
                getPaginatedAllNotifications.Count,
                notifications.Count);
        }

        /// <summary>
        /// Deletes the notifications.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// true if succesfully deleted.
        /// </returns>
        /// <exception cref="Facebook.Model.ValidationsModel">Notification Of This User Is Not Found.</exception>
        public async Task<bool> DeleteNotifications(long userId)
        {
            List<Notification>? notification = await this.db.Notifications.Where(x => x.UserId == userId && x.DeletedAt == null).ToListAsync();

            if (notification.Count == 0)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "Notification Of This User Is Not Found.") } };

            notification.ForEach(x => x.DeletedAt = DateTime.Now);
            this.db.Notifications.UpdateRange(notification);
            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes the notification.
        /// </summary>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns>true if succesfully deleted.</returns>
        /// <exception cref="Facebook.Model.ValidationsModel">Notification Of This User Is Not Found.</exception>
        public async Task<bool> DeleteNotification(long notificationId)
        {
            Notification? notification = await this.db.Notifications.FirstOrDefaultAsync(x => x.NotificationId == notificationId && x.DeletedAt == null);

            if (notification == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "Notification Of This User Is Not Found.") } };

            notification.DeletedAt = DateTime.Now;
            this.db.Notifications.Update(notification);
            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes the notifications.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="notificationId">The notification identifier.</param>
        /// <returns>
        /// true if succesfully read.
        /// </returns>
        /// <exception cref="Facebook.Model.ValidationsModel">Notification Of This User Is Not Found.</exception>
        public async Task<bool> ReadNotification(long userId, long notificationId)
        {
            Notification? notification = await this.db.Notifications.FirstOrDefaultAsync(x => x.UserId == userId && notificationId == x.NotificationId && x.DeletedAt == null);

            if (notification == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "Notification Of This User Is Not Found.") } };

            notification.IsRead = true;
            this.db.Notifications.Update(notification);
            await this.db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes the friend ship notification.
        /// </summary>
        /// <param name="friendShipid">The friend shipid.</param>
        /// <returns>nothing.</returns>
        public async Task DeleteFriendShipNotification(long friendShipid)
        {
            List<Notification> notification = await this.db.Notifications.Where(x => x.ActivityId == friendShipid
            && (x.ActivityType == (int)NotificationTypeEnum.ReceiveFriendRequest || x.ActivityType == (int)NotificationTypeEnum.RejectFriendRequest
            || x.ActivityType == (int)NotificationTypeEnum.AcceptFriendRequest)).ToListAsync();
            if (notification == null)
                throw new AggregateValidationException { Validations = new List<ValidationsModel> { new ValidationsModel((int)HttpStatusCode.NotFound, "Notification Of This User Is Not Found.") } };

            notification.ForEach(x => x.DeletedAt = DateTime.Now);
            this.db.Notifications.UpdateRange(notification);
            await this.db.SaveChangesAsync();
        }
    }
}
