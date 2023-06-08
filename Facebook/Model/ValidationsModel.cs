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
        /// <summary>Gets or sets the status code.</summary>
        /// <value>The status code.</value>
        public int StatusCode { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        /// <value>The error message.</value>
        public string? ErrorMessage { get; set; }

    }

    /// <summary>
    ///   the validationmodel.
    /// </summary>
    public class ValidationModel
    {
        /// <summary>Returns true if ... is valid.</summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; set; }

        /// <summary>Gets or sets the status code.</summary>
        /// <value>The status code.</value>
        public int StatusCode { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        /// <value>The error message.</value>
        public string? ErrorMessage { get; set; }
    }
}
