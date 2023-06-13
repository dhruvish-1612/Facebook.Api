// <copyright file="GetPostCommentModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// Get Post Comment Model.
    /// </summary>
    public class GetPostCommentModel
    {
        /// <summary>
        /// Gets or sets the user post comment identifier.
        /// </summary>
        /// <value>
        /// The user post comment identifier.
        /// </value>
        public long UserPostCommentId { get; set; }

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
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the commented user.
        /// </summary>
        /// <value>
        /// The name of the commented user.
        /// </value>
        public string CommentedUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the commented user avtar.
        /// </summary>
        /// <value>
        /// The commented user avtar.
        /// </value>
        public string? CommentedUserAvtar { get; set; }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        /// <value>
        /// The comment text.
        /// </value>
        public string? CommentText { get; set; }

        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }
    }
}
