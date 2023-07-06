// <copyright file="FacebookHub.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Facebook.Hubs
{
    using Microsoft.AspNetCore.SignalR;

    /// <summary>
    /// FacebookHub.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.SignalR.Hub" />
    public class FacebookHub : Hub
    {
        /// <summary>
        /// Adds the user to group.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// nothing.
        /// </returns>
        public async Task AddUserToGroup(string userId)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, userId);
            await this.Clients.Group(userId).SendAsync("AddUser", "Added to group");
        }
    }
}
