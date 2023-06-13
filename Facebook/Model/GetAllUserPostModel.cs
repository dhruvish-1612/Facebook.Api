// <copyright file="GetAllStoriesForUserModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// get all stories for that user.
    /// </summary>
    public class GetAllUserPostModel
    {
        /// <summary>
        /// Gets or sets the story identifier.
        /// </summary>
        /// <value>
        /// The story identifier.
        /// </value>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user avtar.
        /// </summary>
        /// <value>
        /// The user avtar.
        /// </value>
        public string UserAvtar { get; set; } = string.Empty;
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
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        public DateTime CreatedAt { get; set; }
    }
}
