using Isoqube.Orchestration.Core.ServiceBus.Models;
using Microsoft.AspNetCore.SignalR;

namespace Isoqube.Endpoint.SignalRHubs
{
    public class NotifyClientsHub : Hub
    {
        public async Task SendMessage(NotifyClient notification)
        {
            await Clients.All.SendAsync("listentonotifications", notification);
        }
    }
}
