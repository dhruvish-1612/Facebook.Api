// <copyright file="NotificationTypeEnum.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Enums
{
    using Facebook.Helpers;

    /// <summary>
    /// NotificationTypeEnum.
    /// </summary>
    public enum NotificationTypeEnum
    {
        /// <summary>
        /// The add post.
        /// </summary>
        [StringValue("Post Added")]
        AddPost = 1,

        /// <summary>
        /// The add story.
        /// </summary>
        [StringValue("Story Added")]
        AddStory,

        /// <summary>
        /// The add comment.
        /// </summary>
        [StringValue("Comment Added")]
        AddComment,

        /// <summary>
        /// The like.
        /// </summary>
        [StringValue("Give Like")]
        Like,

        /// <summary>
        /// The receive friend request.
        /// </summary>
        [StringValue("Friend Request Receive")]
        ReceiveFriendRequest,

        /// <summary>
        /// The accept friend request.
        /// </summary>
        [StringValue("Accept Friend Request")]
        AcceptFriendRequest,

        /// <summary>
        /// The reject friend request.
        /// </summary>
        [StringValue("Reject Friend Request")]
        RejectFriendRequest,
    }
}
