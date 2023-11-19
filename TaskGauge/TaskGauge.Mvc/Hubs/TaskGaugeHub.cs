using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace TaskGauge.Mvc.Hubs
{
    public class TaskGaugeHub : Hub
    {
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.OthersInGroup(roomName).SendAsync("userJoined", Context.ConnectionId);
        }

    }
}
