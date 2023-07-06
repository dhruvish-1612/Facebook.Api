// <copyright file="FrienshipEnum.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Enums
{
    using Facebook.Helpers;

    /// <summary>
    /// Enum for User Frenship Request.
    /// </summary>
    public enum FrienshipEnum
    {
        /// <summary>
        /// The request pending.
        /// </summary>
        [StringValue("Your Request Is Pending")]
        Request_Pending,

        /// <summary>
        /// The request accepted.
        /// </summary>
        [StringValue("You Accept The Request")]
        Request_Accepted,

        /// <summary>
        /// The request rejected.
        /// </summary>
        [StringValue("You Reject The Request")]
        Request_Rejected,
    }
}
