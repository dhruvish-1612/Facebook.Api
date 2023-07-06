// <copyright file="ValidationsModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    ///   <para>
    ///     validations model.
    ///   </para>
    /// </summary>
    public class ValidationsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationsModel" /> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="errorMessage">The error message.</param>
        public ValidationsModel(int statusCode, string errorMessage)
        {
            this.StatusCode = statusCode;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationsModel"/> class.
        /// </summary>
        public ValidationsModel()
        {
        }

        /// <summary>Gets or sets the status code.</summary>
        /// <value>The status code.</value>
        public int StatusCode { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        /// <value>The error message.</value>
        public string? ErrorMessage { get; set; }
    }
}
