// <copyright file="PostMediaModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// PostMediaModel.
    /// </summary>
    public class PostMediaModel
    {
        /// <summary>
        /// Gets or sets the user post identifier.
        /// </summary>
        /// <value>
        /// The user post identifier.
        /// </value>
        public long UserPostId { get; set; }

        /// <summary>
        /// Gets or sets the media path.
        /// </summary>
        /// <value>
        /// The media path.
        /// </value>
        public string? MediaPath { get; set; }

        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        public string? MediaType { get; set; }
    }
}
