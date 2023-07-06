// <copyright file="UserPostsModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// User Posts Model.
    /// </summary>
    public class UserPostsModel
    {
        /// <summary>
        /// Gets or sets the post text.
        /// </summary>
        /// <value>
        /// The post text.
        /// </value>
        public string? PostText { get; set; }

        /// <summary>
        /// Gets or sets the posts.
        /// </summary>
        /// <value>
        /// The posts.
        /// </value>
        public List<IFormFile> Posts { get; set; } = new List<IFormFile>();
    }
}
