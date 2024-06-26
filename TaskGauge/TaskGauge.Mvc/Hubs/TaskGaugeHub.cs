﻿using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using TaskGauge.Common;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.DataTransferObject;
using TaskGauge.Services;
using TaskGauge.ViewModel;

namespace TaskGauge.Mvc.Hubs
{
    public class TaskGaugeHub : Hub
    {
        RoomStatic roomUserStatic = RoomStatic.Instance;
        private static Dictionary<string, string> _openOrClosedTask = new Dictionary<string, string>();
        private IRoomDal _roomDal;
        private readonly IDatabase _redisDatabase;

        public TaskGaugeHub(IRoomDal roomDal, RedisService redisService)
        {
            _redisDatabase = redisService.Connect(0);
            _roomDal = roomDal;
        }

        public async Task JoinRoom(string roomName, string isAdmin)
        {
            List<RoomUserDto> roomUserListFromCache = _roomDal.GetAllRoomUserInformationFromRedis();
         
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Request.Cookies["Username"];
            RoomUserDto roomMember = new RoomUserDto();
            roomMember.IsItInTheRoom = true;
            roomMember.RoomName = roomName;
            roomMember.Username = username;
            roomMember.UserId = Convert.ToInt32(httpContext.Request.Cookies["UserId"]);
            roomMember.UserRole = httpContext.Request.Cookies["UserRole"] ?? string.Empty;
            bool.TryParse(isAdmin, out var isAdminBool);
            roomMember.IsAdmin = isAdminBool;
            roomMember.ConnectionId = Context.ConnectionId;
            var roomTaskList = GetRoomTaskList(_roomDal.GetAllRoomTaskFromRedis(), roomName);
            if (roomUserListFromCache.Exists(x => x.Username == username && !x.IsItInTheRoom && x.RoomName.Equals(roomName)))
            {
                foreach(var user in roomUserListFromCache)
                {
                    if(user.Username == username && user.RoomName.Equals(roomName))
                    {
                       var redisUpdateValueIndex =  _redisDatabase.ListPosition(TextResources.RedisCacheKeys.RoomUser, JsonSerializer.Serialize(user));
                        user.IsItInTheRoom = true;
                        user.ConnectionId = roomMember.ConnectionId;
                        _redisDatabase.ListSetByIndex(TextResources.RedisCacheKeys.RoomUser, redisUpdateValueIndex, JsonSerializer.Serialize(user));
                    }
                }
            }
            else if (roomUserListFromCache.Exists(x => x.Username == username && x.IsItInTheRoom && x.RoomName.Equals(roomName)))
            {
                var message = "You are already in the room.";
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
                await Clients.Caller.SendAsync("alreadyInTheJoinRoom", message);
                return;
            }
            else
            {
                _redisDatabase.ListRightPush(TextResources.RedisCacheKeys.RoomUser, JsonSerializer.Serialize(roomMember));
                _redisDatabase.KeyExpire(TextResources.RedisCacheKeys.RoomUser, TimeSpan.FromHours(2));
                roomUserListFromCache.Add(roomMember);
            }

            var roomUserList = GetTheNameOfTheUsersInTheRoom(roomUserListFromCache, roomName);
            await Clients.Caller.SendAsync("userList", roomUserList);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);
            await Clients.Caller.SendAsync("addTaskForJoinedUser", roomTaskList);
            if (Convert.ToBoolean(isAdmin))
            {
                var adminConnectionId = roomUserListFromCache.Where(x => x.RoomName.Equals(roomName) && x.IsAdmin).FirstOrDefault().ConnectionId;
                await Clients.Client(adminConnectionId).SendAsync("getEffort", roomUserStatic.taskEffortList, roomUserStatic.totalTaskEffortInformation);
            }

        }

        public async Task AddTask(string taskName)
        {
            List<RoomUserDto> roomUserListFromCache = _roomDal.GetAllRoomUserInformationFromRedis();

            var user = roomUserListFromCache.Where(x => x.ConnectionId.Equals(Context.ConnectionId)).FirstOrDefault();
            var roomName = user.RoomName;
            var allRoomTask = _roomDal.GetAllRoomTaskFromRedis();
            var isExistTaskName = allRoomTask.Exists(x => x.TaskName.Equals(taskName) && x.RoomName.Equals(roomName));

            TaskViewModel taskModel = new TaskViewModel()
            {
                TaskName = taskName
            };

            if (isExistTaskName)
            {
                taskModel = new TaskViewModel()
                {
                    IsSuccess = false,
                    Message = "Task name is exist."
                };
            }
            else
            {
                taskModel = new TaskViewModel()
                {
                    TaskName = taskName,
                    IsSuccess = true
                };

                var taskJsonModel = new TaskDto
                {
                    RecordBy = user.UserId,
                    RecordDate = DateTime.Now,
                    RoomName = roomName,
                    TaskName = taskName
                };

                _redisDatabase.ListRightPush(TextResources.RedisCacheKeys.AllRoomTasks, JsonSerializer.Serialize(taskJsonModel));
            }

            await Clients.Caller.SendAsync("addedTaskByAdmin", taskModel);
            if (taskModel.IsSuccess)
            {
                await Clients.OthersInGroup(roomName).SendAsync("newTask", taskModel);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            List<RoomUserDto> roomUserListFromCache = _roomDal.GetAllRoomUserInformationFromRedis();
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Request.Cookies["Username"];
            var roomUser = roomUserListFromCache.FirstOrDefault(x => x.Username == username && x.IsItInTheRoom);
            if (Context.ConnectionId == roomUser.ConnectionId)
            {
                foreach (var user in roomUserListFromCache)
                {
                    if (user.Username == username && user.RoomName.Equals(roomUser.RoomName))
                    {
                        var redisUpdateValueIndex = _redisDatabase.ListPosition(TextResources.RedisCacheKeys.RoomUser, JsonSerializer.Serialize(user));
                        user.IsItInTheRoom = false;
                        _redisDatabase.ListSetByIndex(TextResources.RedisCacheKeys.RoomUser, redisUpdateValueIndex, JsonSerializer.Serialize(user));
                    }
                }

                await Clients.OthersInGroup(roomUser.RoomName).SendAsync("userLeft", username);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomUser.RoomName);
                await base.OnDisconnectedAsync(exception);
            }
        }

        public async Task TaskEffort(string taskName, string taskEffortDuration)
        {
            var roomUserListFromCache = _roomDal.GetAllRoomUserInformationFromRedis();

            var user = roomUserListFromCache.Where(x => x.ConnectionId.Equals(Context.ConnectionId)).FirstOrDefault();
            var isExistEffort = false;
            var taskEffort = Convert.ToDouble(taskEffortDuration, CultureInfo.InvariantCulture);
            foreach (var item in roomUserStatic.taskEffortList)
            {
                if (item.TaskName.Equals(taskName) && item.Username.Equals(user.Username) && item.RoomName.Equals(user.RoomName))
                {
                    isExistEffort = true;
                    item.Effort = taskEffort;
                }
            }

            if (!isExistEffort)
            {
                roomUserStatic.taskEffortList.Add(new TaskEffortViewModel
                {
                    Effort = taskEffort,
                    TaskName = taskName,
                    Username = user.Username,
                    RoomName = user.RoomName,
                    UserRole = user.UserRole
                });
            }
            if (roomUserStatic.totalTaskEffortInformation.Count > 0)
            {
                roomUserStatic.totalTaskEffortInformation.RemoveAll(x => x.RoomName.Equals(user.RoomName) && x.TaskName.Equals(taskName));
            }
            var totalEffort = GetTotalEffortInformationForAdmin(user.RoomName, taskName);
            roomUserStatic.totalTaskEffortInformation.Add(totalEffort);
            var adminConnectionId = roomUserListFromCache.Where(x => x.RoomName.Equals(user.RoomName) && x.IsAdmin).FirstOrDefault().ConnectionId;
            await Clients.Client(adminConnectionId).SendAsync("getEffort", roomUserStatic.taskEffortList, roomUserStatic.totalTaskEffortInformation);

        }

        public TotalTaskEffortInformationViewModel GetTotalEffortInformationForAdmin(string roomName, string taskName)
        {
            TotalTaskEffortInformationViewModel totalEffortInformation = new TotalTaskEffortInformationViewModel();
            foreach (var item in roomUserStatic.taskEffortList)
            {
                if (item.RoomName.Equals(roomName, StringComparison.OrdinalIgnoreCase) && item.TaskName.Equals(taskName, StringComparison.OrdinalIgnoreCase))
                {
                    if (item.UserRole.Equals("Tester", StringComparison.OrdinalIgnoreCase))
                    {
                        totalEffortInformation.TesterTotalEffort += item.Effort;
                    }
                    else
                    {
                        totalEffortInformation.DevTotalEffort += item.Effort;
                    }

                    totalEffortInformation.TotalEffort += item.Effort;
                }
            }
            totalEffortInformation.TaskName = taskName;
            totalEffortInformation.RoomName = roomName;
            return totalEffortInformation;
        }

        public async Task OpenOrCloseTask(string taskSituation, string taskName)
        {
            var roomUserListFromCache = _roomDal.GetAllRoomUserInformationFromRedis();
            if (!_openOrClosedTask.ContainsKey(taskName))
            {
                _openOrClosedTask.Add(taskName, taskSituation);
            }
            else
            {
                _openOrClosedTask[taskName] = taskSituation;
            }
            var roomName = roomUserListFromCache.FirstOrDefault(x => x.ConnectionId.Equals(Context.ConnectionId)).RoomName;
            await Clients.OthersInGroup(roomName).SendAsync("changeTaskSituation", taskSituation, taskName);
        }

        public async Task SaveDatabase(string taskName)
        {
            var errorMessageFromDatabase = _roomDal.SaveToDatabase(taskName);
            await Clients.Caller.SendAsync("saveToDatabaseNotification", errorMessageFromDatabase);
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

        private List<RoomTaskForNewJoinedUserViewModel> GetRoomTaskList(List<TaskDto> allTask, string roomName)
        {
            List<RoomTaskForNewJoinedUserViewModel> requestRoomTaskList = new List<RoomTaskForNewJoinedUserViewModel>();
            foreach (var item in allTask)
            {
                if (item.RoomName == roomName)
                {
                    requestRoomTaskList.Add(new RoomTaskForNewJoinedUserViewModel { TaskName = item.TaskName, TaskSituation = _openOrClosedTask.ContainsKey(item.TaskName) ? _openOrClosedTask[item.TaskName] : "Open" });
                }
            }
            return requestRoomTaskList;
        }
    }
}
