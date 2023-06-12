// <copyright file="StoryModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// Story Model.
    /// </summary>
    public class StoryModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string? WrittenText { get; set; }

        /// <summary>
        /// Gets or sets the media path.
        /// </summary>
        /// <value>
        /// The media path.
        /// </value>
        public string MediaPath { get; set; } = null!;

        /// <summary>
        /// Gets or sets the type of the media.
        /// </summary>
        /// <value>
        /// The type of the media.
        /// </value>
        public string MediaType { get; set; } = null!;
    }
}
