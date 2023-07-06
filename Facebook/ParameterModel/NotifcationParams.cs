// <copyright file="NotifcationParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.ParameterModel
{
    /// <summary>
    /// NotifcationParams.
    /// </summary>
    public class NotifcationParams
    {
        /// <summary>
        /// Gets or sets The user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets The pagination parameters.
        /// </summary>
        /// <value>
        /// The pagination parameters.
        /// </value>
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }
}
