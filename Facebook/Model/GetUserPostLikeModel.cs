// <copyright file="GetUserPostLikeModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// Get User.
    /// </summary>
    public class GetUserPostLikeModel
    {
        /// <summary>
        /// Gets or sets the user post comment identifier.
        /// </summary>
        /// <value>
        /// The user post comment identifier.
        /// </value>
        public long UserPostLikeId { get; set; }

        /// <summary>
        /// Gets or sets the user post identifier.
        /// </summary>
        /// <value>
        /// The user post identifier.
        /// </value>
        public long UserPostId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long LikedUserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the commented user.
        /// </summary>
        /// <value>
        /// The name of the commented user.
        /// </value>
        public string LikedUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the commented user avtar.
        /// </summary>
        /// <value>
        /// The commented user avtar.
        /// </value>
        public string? LikedUserAvtar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [like status].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [like status]; otherwise, <c>false</c>.
        /// </value>
        public bool? LikeStatus { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }
    }
}
