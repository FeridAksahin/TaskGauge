using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TaskGauge.DataTransferObject;

namespace TaskGauge.Mvc.Hubs
{
    public class TaskGaugeHub : Hub
    {
        RoomStatic roomUser = RoomStatic.Instance;
        public async Task JoinRoom(string roomName)
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Session.GetString("Username");
            RoomUserDto roomMember = new RoomUserDto();
            roomMember.IsItInTheRoom = true;
            roomMember.RoomName = roomName;
            roomMember.Username = username;
            if (!roomUser.roomUser.Exists(x => x.Username == username))
            {
                roomUser.roomUser.Add(roomMember);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);
            }
            else if (roomUser.roomUser.Exists(x => x.Username == username && x.IsItInTheRoom == false))
            {
                roomUser.roomUser.ForEach(user => user.IsItInTheRoom = user.Username == username ? true : user.IsItInTheRoom);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var httpContext = Context.GetHttpContext();
            var username = httpContext.Session.GetString("Username");
            var roomName = roomUser.roomUser.FirstOrDefault(x => x.Username == username).RoomName;
            roomUser.roomUser.ForEach(x => x.IsItInTheRoom = x.Username == username ? false : x.IsItInTheRoom);
            await Clients.OthersInGroup(roomName).SendAsync("userLeft", username);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
