// <copyright file="GetRequestedUserParam.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.ParameterModel
{
    /// <summary>
    /// GetRequestedUser.
    /// </summary>
    public class GetRequestedUserParam
    {
        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public int Filter { get; set; }

        /// <summary>
        /// Gets or sets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public int RequestType { get; set; }

        /// <summary>
        /// Gets or sets the pagination parameters.
        /// </summary>
        /// <value>
        /// The pagination parameters.
        /// </value>
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }
}
