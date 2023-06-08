// <copyright file="AggregateValidationException.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.CustomException
{
    using Facebook.Model;

    /// <summary>
    /// AggregateValidationException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AggregateValidationException : Exception
    {
        /// <summary>
        /// Gets or sets the validations.
        /// </summary>
        /// <value>
        /// The validations.
        /// </value>
        public List<ValidationsModel> Validations { get; set; } = new List<ValidationsModel>();
    }
}
