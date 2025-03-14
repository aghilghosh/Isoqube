using Isoqube.Orchestration.Core.ServiceBus.Models;
using Microsoft.AspNetCore.SignalR;

namespace Isoqube.Orchestration.Core.SignalR
{
    public class NotifyClientsHub : Hub
    {
        public async Task SendMessage(NotifyClient notification)
        {
            await Clients.All.SendAsync("ReceiveMessage", notification);
        }
    }
}
