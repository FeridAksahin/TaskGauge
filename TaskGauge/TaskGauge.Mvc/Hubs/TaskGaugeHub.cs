﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TaskGauge.DataTransferObject;
using TaskGauge.ViewModel;

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
            roomMember.ConnectionId = Context.ConnectionId;
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
                roomUserStatic.roomUser.ForEach(user => user.ConnectionId = user.Username == username ? roomMember.ConnectionId : user.ConnectionId);
                var roomUserList = GetTheNameOfTheUsersInTheRoom(roomUserStatic.roomUser, roomName);
                await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
                await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);
                await Clients.Caller.SendAsync("userList", roomUserList);
            }
        }

        public async Task AddTask(string taskName)
        {
            var user = roomUserStatic.roomUser.Where(x => x.ConnectionId.Equals(Context.ConnectionId)).FirstOrDefault();
            var roomName = user.RoomName;

            var isExistTaskName = roomUserStatic.allRoomTask.Exists(x => x.TaskName.Equals(taskName));

            AddTaskViewModel addTaskModel = new AddTaskViewModel()
            {
                TaskName = taskName
            };

            if (isExistTaskName)
            {
                addTaskModel = new AddTaskViewModel()
                {
                    IsSuccess = false,
                    Message = "Task name is exist."
                };
            }
            else
            {
                addTaskModel = new AddTaskViewModel()
                {
                    TaskName = taskName,
                    IsSuccess = true
                };
                roomUserStatic.allRoomTask.Add(new TaskDto
                {
                    RecordByName = user.Username,
                    RecordDate = DateTime.Now,
                    RoomName = roomName,
                    TaskName = taskName
                });
            }

            await Clients.Caller.SendAsync("addedTaskByAdmin", addTaskModel);

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
            foreach (var item in userList)
            {
                if (item.RoomName == roomName && item.IsItInTheRoom)
                {
                    requestRoomUserList.Add(item.Username);
                }
            }
            return requestRoomUserList;
        }
    }
}
