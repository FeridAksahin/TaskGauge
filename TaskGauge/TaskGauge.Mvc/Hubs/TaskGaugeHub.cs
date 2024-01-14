using Microsoft.AspNetCore.SignalR;
using System;
using System.Globalization;
using System.Threading.Tasks;
using TaskGauge.Common;
using TaskGauge.DataTransferObject;
using TaskGauge.ViewModel;

namespace TaskGauge.Mvc.Hubs
{
    public class TaskGaugeHub : Hub
    {
        RoomStatic roomUserStatic = RoomStatic.Instance;
        private static Dictionary<string, string> _openOrClosedTask = new Dictionary<string, string>();
        public async Task JoinRoom(string roomName, string isAdmin)
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Request.Cookies["Username"];
            RoomUserDto roomMember = new RoomUserDto();
            roomMember.IsItInTheRoom = true;
            roomMember.RoomName = roomName;
            roomMember.Username = username;
            roomMember.UserId = Convert.ToInt32(httpContext.Request.Cookies["UserId"]);
            bool.TryParse(isAdmin, out var isAdminBool);
            roomMember.IsAdmin = isAdminBool;
            roomMember.ConnectionId = Context.ConnectionId;
            var roomTaskList = GetRoomTaskList(roomUserStatic.allRoomTask, roomName);
            if (roomUserStatic.roomUser.Exists(x => x.Username == username && !x.IsItInTheRoom && x.RoomName.Equals(roomName)))
            {
                roomUserStatic.roomUser.ForEach(user => user.IsItInTheRoom = user.Username == username && user.RoomName.Equals(roomName) ? true : user.IsItInTheRoom);
                roomUserStatic.roomUser.ForEach(user => user.ConnectionId = user.Username == username && user.RoomName.Equals(roomName) ? roomMember.ConnectionId : user.ConnectionId); 
            }
            else
            {
                roomUserStatic.roomUser.Add(roomMember);
            }

            var roomUserList = GetTheNameOfTheUsersInTheRoom(roomUserStatic.roomUser, roomName);
            await Clients.Caller.SendAsync("userList", roomUserList);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await Clients.OthersInGroup(roomName).SendAsync("userJoined", username);
            await Clients.Caller.SendAsync("addTaskForJoinedUser", roomTaskList);
            if (Convert.ToBoolean(isAdmin))
            {
                var adminConnectionId = roomUserStatic.roomUser.Where(x => x.RoomName.Equals(roomName) && x.IsAdmin).FirstOrDefault().ConnectionId;
                await Clients.Client(adminConnectionId).SendAsync("allTaskEffortForJoinedAdminUser", roomUserStatic.taskEffortList);
            }
        
        }

        public async Task AddTask(string taskName)
        {
            var user = roomUserStatic.roomUser.Where(x => x.ConnectionId.Equals(Context.ConnectionId)).FirstOrDefault();
            var roomName = user.RoomName;

            var isExistTaskName = roomUserStatic.allRoomTask.Exists(x => x.TaskName.Equals(taskName));

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
                roomUserStatic.allRoomTask.Add(new TaskDto
                {
                    RecordBy = user.UserId,
                    RecordDate = DateTime.Now,
                    RoomName = roomName,
                    TaskName = taskName
                });
            }

            await Clients.Caller.SendAsync("addedTaskByAdmin", taskModel);
            if (taskModel.IsSuccess)
            {
                await Clients.OthersInGroup(roomName).SendAsync("newTask", taskModel);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            var username = httpContext.Request.Cookies["Username"];
            var roomName = roomUserStatic.roomUser.FirstOrDefault(x => x.Username == username && x.IsItInTheRoom).RoomName;
            roomUserStatic.roomUser.ForEach(x => x.IsItInTheRoom = x.Username == username && x.RoomName.Equals(roomName) ? false : x.IsItInTheRoom);
            await Clients.OthersInGroup(roomName).SendAsync("userLeft", username);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task TaskEffort(string taskName, string taskEffortDuration)
        {
            var user = roomUserStatic.roomUser.Where(x => x.ConnectionId.Equals(Context.ConnectionId)).FirstOrDefault();
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
                    RoomName = user.RoomName
                });
            }
            var adminConnectionId = roomUserStatic.roomUser.Where(x => x.RoomName.Equals(user.RoomName) && x.IsAdmin).FirstOrDefault().ConnectionId;
            await Clients.Client(adminConnectionId).SendAsync("getEffort", roomUserStatic.taskEffortList);
       
        }

        public async Task OpenOrCloseTask(string taskSituation, string taskName)
        {
            if (!_openOrClosedTask.ContainsKey(taskName))
            {
                _openOrClosedTask.Add(taskName, taskSituation);
            }
            else
            {
                _openOrClosedTask[taskName] = taskSituation;    
            }
            var roomName = roomUserStatic.roomUser.FirstOrDefault(x => x.ConnectionId.Equals(Context.ConnectionId)).RoomName;
            await Clients.OthersInGroup(roomName).SendAsync("changeTaskSituation", taskSituation, taskName);
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
