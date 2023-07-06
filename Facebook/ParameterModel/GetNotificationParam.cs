// <copyright file="GetNotificationParam.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.ParameterModel
{
    /// <summary>
    /// GetNotificationParam.
    /// </summary>
    public class GetNotificationParam
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the pagination parameters.
        /// </summary>
        /// <value>
        /// The pagination parameters.
        /// </value>
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }
}
