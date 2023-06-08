// <copyright file="FriendShipModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Facebook.Model
{
    /// <summary>
    ///  FriendShip Model.
    /// </summary>
    public class FriendShipModel
    {
        /// <summary>
        /// Gets or sets the friendship identifier.
        /// </summary>
        /// <value>
        /// The friendship identifier.
        /// </value>
        public long FriendshipId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the profile request.
        /// </summary>
        /// <value>
        /// The profile request.
        /// </value>
        public long RequestedUserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the profile accept.
        /// </summary>
        /// <value>
        /// The profile accept.
        /// </value>
        public long AcceptedUserId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the name of the requested user.
        /// </summary>
        /// <value>
        /// The name of the requested user.
        /// </value>
        public string RequestedUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the accepted user.
        /// </summary>
        /// <value>
        /// The name of the accepted user.
        /// </value>
        public string AcceptedUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the accepted user avtar.
        /// </summary>
        /// <value>
        /// The accepted user avtar.
        /// </value>
        public string AcceptedUserAvtar { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the requested user avtar.
        /// </summary>
        /// <value>
        /// The requested user avtar.
        /// </value>
        public string RequestedUserAvtar { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string? Status { get; set; } = string.Empty;
    }
}
