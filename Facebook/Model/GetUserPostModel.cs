﻿// <copyright file="GetUserPostModel.cs" company="PlaceholderCompany">
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
        public string? WrittenText { get; set; }

        /// <summary>
        /// Gets or sets the media path.
        /// </summary>
        /// <value>
        /// The media path.
        /// </value>
        public string MediaPath { get; set; } = null!;
    }
}
