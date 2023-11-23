using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace TaskGauge.Mvc.Hubs
{
    public class TaskGaugeHub : Hub
    {
        public async Task JoinRoom(string roomName)
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Session.GetString("Username");
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);


        }

    }
}
