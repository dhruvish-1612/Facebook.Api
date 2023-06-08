// <copyright file="RequestedUsersModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    /// this model is for only fetch the only requested model.
    /// </summary>
    public class RequestedUsersModel
    {
        /// <summary>
        /// Gets or sets the friendship identifier.
        /// </summary>
        /// <value>
        /// The friendship identifier.
        /// </value>
        public long FriendshipId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        /// <value>
        /// The user role.
        /// </value>
        public string RequestType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request status.
        /// </summary>
        /// <value>
        /// The request status.
        /// </value>
        public string RequestStatus { get; set; } = string.Empty;
    }
}
