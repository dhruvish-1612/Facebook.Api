// <copyright file="FilterViaRequestEnum.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Enums
{
    using Facebook.Helpers;

    /// <summary>
    /// check for filter.
    /// </summary>
    public enum FilterViaRequestEnum
    {
        /// <summary>
        /// The is pending.
        /// </summary>
        [StringValue("Request Is Pending")]
        Is_Pending,

        /// <summary>
        /// The is accepted.
        /// </summary>
        [StringValue("Request Is Accepted")]
        Is_Accepted,

        /// <summary>
        /// The is rejected.
        /// </summary>
        [StringValue("Request Is Rejected")]
        Is_Rejected,
    }
}
