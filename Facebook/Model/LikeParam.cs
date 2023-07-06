// <copyright file="LikeParam.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// LikeParam.
    /// </summary>
    public class LikeParam
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long PostUserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the like user identifier.
        /// </summary>
        /// <value>
        /// The like user identifier.
        /// </value>
        public long LikeUserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is liked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is liked; otherwise, <c>false</c>.
        /// </value>
        public bool? IsLiked { get; set; }
    }
}
