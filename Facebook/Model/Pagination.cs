// <copyright file="Pagination.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// Pagination.
    /// </summary>
    /// <typeparam name="T">List of model.</typeparam>
    public class Pagination<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pagination{T}"/> class.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="recoeds">The recoeds.</param>
        /// <param name="totalRecords">The total records.</param>
        /// <param name="totalItens">The total itens.</param>
        public Pagination(List<T> recoeds, int totalRecords, int totalItens)
        {
            this.Records = recoeds;
            this.TotalRecords = totalRecords;
            this.TotalItems = totalItens;
        }

        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>
        /// The total records.
        /// </value>
        public int TotalRecords { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        /// <value>
        /// The total items.
        /// </value>
        public int TotalItems { get; set; } = 0;

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>
        /// The records.
        /// </value>
        public List<T> Records { get; set; } = new List<T>();
    }
}
