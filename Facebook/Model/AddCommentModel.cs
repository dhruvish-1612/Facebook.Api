// <copyright file="AddCommentModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// CommentModel.
    /// </summary>
    public class AddCommentModel
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
        /// Gets or sets the comment text.
        /// </summary>
        /// <value>
        /// The comment text.
        /// </value>
        public string? CommentText { get; set; }
    }
}
