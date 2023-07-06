// <copyright file="PaginationParams.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.ParameterModel
{
    /// <summary>
    /// PaginationParams.
    /// </summary>
    public class PaginationParams
    {
        private int pageSize;
        private int pageNumber;

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        /// <value>
        /// The page number.
        /// </value>
        public int PageNumber { get => this.pageNumber; set => this.pageNumber = value > 0 ? value : 1; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get => this.pageSize; set => this.pageSize = value > 0 ? value : 10; }
    }
}
