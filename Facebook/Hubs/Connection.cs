using Microsoft.AspNetCore.SignalR.Client;

namespace Facebook.Hubs
{
    /// <summary>
    /// dfd.
    /// </summary>
    public class Connection
    {
        private readonly HubConnection hubConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="hubConnection">The hub connection.</param>
        public Connection(HubConnection hubConnection)
        {
            this.hubConnection = hubConnection;
            this.hubConnection = new HubConnectionBuilder().WithUrl("").Build();
        }

        public async Task<HubConnection> GetConnection()
        {
            if(this.hubConnection.State == HubConnectionState.Disconnected)
            {
                await this.hubConnection.StartAsync();
            }

            return this.hubConnection;
        }
    }
}
