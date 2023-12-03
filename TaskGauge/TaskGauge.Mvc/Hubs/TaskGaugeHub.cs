using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TaskGauge.DataTransferObject;

namespace TaskGauge.Mvc.Hubs
{
    public class TaskGaugeHub : Hub
    {
        RoomStatic roomUserStatic = RoomStatic.Instance;
        public async Task JoinRoom(string roomName)
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Request.Cookies["Username"];
            RoomUserDto roomMember = new RoomUserDto();
            roomMember.IsItInTheRoom = true;
            roomMember.RoomName = roomName;
            roomMember.Username = username;
            if (!roomUserStatic.roomUser.Exists(x => x.Username == username))
            {
                roomUserStatic.roomUser.Add(roomMember);
                var roomUserList = GetTheNameOfTheUsersInTheRoom(roomUserStatic.roomUser, roomName);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);
                await Clients.Caller.SendAsync("userList", roomUserList);

            }
            else if (roomUserStatic.roomUser.Exists(x => x.Username == username && !x.IsItInTheRoom))
            {
                roomUserStatic.roomUser.ForEach(user => user.IsItInTheRoom = user.Username == username ? true : user.IsItInTheRoom);
                var roomUserList = GetTheNameOfTheUsersInTheRoom(roomUserStatic.roomUser, roomName);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);
                await Clients.Caller.SendAsync("userList", roomUserList);
            }
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var httpContext = Context.GetHttpContext();
            var username = httpContext.Request.Cookies["Username"];
            var roomName = roomUserStatic.roomUser.FirstOrDefault(x => x.Username == username).RoomName;
            roomUserStatic.roomUser.ForEach(x => x.IsItInTheRoom = x.Username == username ? false : x.IsItInTheRoom);
            await Clients.OthersInGroup(roomName).SendAsync("userLeft", username);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await base.OnDisconnectedAsync(exception);
        } 

        private List<string> GetTheNameOfTheUsersInTheRoom(List<RoomUserDto> userList, string roomName)
        {
            List<string> requestRoomUserList = new List<string>();
            foreach(var item in userList)
            {
                if(item.RoomName == roomName && item.IsItInTheRoom)
                {
                    requestRoomUserList.Add(item.Username);
                }
            }
            return requestRoomUserList;
        }
    }
}
