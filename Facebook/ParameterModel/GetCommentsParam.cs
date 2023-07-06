// <copyright file="GetCommentsParam.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.ParameterModel
{
    /// <summary>
    /// GetCommentsModel.
    /// </summary>
    public class GetCommentsParam
    {
        /// <summary>
        /// Gets or sets the post identifier.
        /// </summary>
        /// <value>
        /// The post identifier.
        /// </value>
        public long PostId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the pagination parameters.
        /// </summary>
        /// <value>
        /// The pagination parameters.
        /// </value>
        public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    }
}
