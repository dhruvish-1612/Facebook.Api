// <copyright file="ResetPasswordParam.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.ParameterModel
{
    /// <summary>
    /// ResetPasswordParam.
    /// </summary>
    public class ResetPasswordParam
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        /// <value>
        /// The old password.
        /// </value>
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the updated password.
        /// </summary>
        /// <value>
        /// The updated password.
        /// </value>
        public string UpdatedPassword { get; set; } = null!;
    }
}
