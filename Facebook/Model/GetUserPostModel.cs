// <copyright file="GetUserPostModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// GetUserPost.
    /// </summary>
    public class GetUserPostModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the post text.
        /// </summary>
        /// <value>
        /// The post text.
        /// </value>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the post media with types.
        /// </summary>
        /// <value>
        /// The post media with types.
        /// </value>
        public List<PostsWithTypes> PostMediaWithTypes { get; set; } = new List<PostsWithTypes>();
    }
}
