// <copyright file="PostParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    using Facebook.ParameterModel;

    /// <summary>
    /// PostParams.
    /// </summary>
    public class PostParams
    {
        /// <summary>
        /// Gets or sets a value indicating whether.
        /// </summary>
        /// <value>
        /// The is user posts.
        /// </value>
        public bool IsUserPosts { get; set; } = false;

        /// <summary>
        /// Gets or sets the pagination parameters.
        /// </summary>
        /// <value>
        /// The pagination parameters.
        /// </value>
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }
}
