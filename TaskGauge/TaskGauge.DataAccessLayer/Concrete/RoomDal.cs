using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskGauge.Common;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.DataTransferObject;
using TaskGauge.Entity.Context;
using TaskGauge.Entity.Entity;
using TaskGauge.Services;

namespace TaskGauge.DataAccessLayer.Concrete
{
    public class RoomDal : IRoomDal
    {
        private IDatabase _redisService;
        RoomStatic roomStatic = RoomStatic.Instance;
        private TaskGaugeContext _taskGaugeContext;
        private UserInformation _userInformation;
        public RoomDal(TaskGaugeContext taskGaugeContext, UserInformation userInformation, RedisService redisService)
        {
            _redisService = redisService.Connect(0);
            _userInformation = userInformation;
            _taskGaugeContext = taskGaugeContext;
        }

        public bool IsRoomExist(string roomName)
        {
            return _taskGaugeContext.Room.ToList().Exists(x => x.Name.Trim().Equals(roomName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public bool AddRoom(string roomName)
        {
            _taskGaugeContext.Room.Add(new TaskGauge.Entity.Entity.Room { Name = roomName, RoomAdminId = _userInformation.GetUserIdFromCookie() });
            int result = _taskGaugeContext.SaveChanges();
            return result > 0;
        }

        public bool IsTheLoggedInUserTheRoomAdministrator(string roomName)
        {
            return (from room in _taskGaugeContext.Room
                    where room.Name.Equals(roomName) &&
                    room.RoomAdminId.Equals(_userInformation.GetUserIdFromCookie())
                    select room).Any();
        }

        public void GetAllRoomIntoStaticList()
        {
            var rooms = from room in _taskGaugeContext.Room
                        where room.isActive
                        select room;
            foreach (var room in rooms)
            {
                this.roomStatic.room.Add(new DataTransferObject.Room
                {
                    Name = room.Name,
                    isActive = room.isActive,
                    RoomAdmin = room.RoomAdmin.Name
                });
            }
        }

        public bool IsExistRoomName(string roomName)
        {
            return _taskGaugeContext.Room.ToList().Exists(x => x.Name == roomName);
        }

        public void GetAllTaskIntoRedisList()
        {
            var tasks = from task in _taskGaugeContext.Task
                        select new
                        {
                            Task = task,
                            RoomName = task.Room.Name
                        };
            _redisService.ListTrim(TextResources.RedisCacheKeys.AllRoomTasks, 1, 0);
            foreach (var task in tasks)
            {

                var serializeTaskModel = new { RecordBy = task.Task.RecordBy, RecordDate = task.Task.RecordDate, RoomName = task.RoomName, TaskName = task.Task.Name };
                _redisService.ListRightPush(TextResources.RedisCacheKeys.AllRoomTasks, JsonSerializer.Serialize(serializeTaskModel));
            }

        }

        public string SaveToDatabase(string roomName)
        {
            var roomId = _taskGaugeContext.Room.Where(x => x.Name.Equals(roomName)).FirstOrDefault().Id;
            return SaveTaskToDatabase(roomName, roomId) + SaveTaskTotalEffortInformationToDatabase(roomName, roomId) + SaveUserEffortToDatabase(roomName, roomId);
        }

        public List<TaskDto> GetAllRoomTaskFromRedis()
        {
            List<TaskDto> allRoomTask = new();
            var allRoomTasksRedisKey = TextResources.RedisCacheKeys.AllRoomTasks;
            if (_redisService.KeyExists(allRoomTasksRedisKey))
            {
                allRoomTask = _redisService.ListRange(allRoomTasksRedisKey).Select(x => JsonSerializer.Deserialize<TaskDto>(x)).ToList();
            }
            return allRoomTask;
        }

        public string SaveTaskToDatabase(string roomName, int roomId)
        {
            try
            {
                foreach (var task in GetAllRoomTaskFromRedis())
                {
                    if (task.RoomName.Equals(roomName) && !IsTaskExist(roomId, task.TaskName))
                    {
                        _taskGaugeContext.Task.Add(new Entity.Entity.Task { RoomId = roomId, Name = task.TaskName, RecordBy = _userInformation.GetUserIdFromCookie() });
                        _taskGaugeContext.SaveChanges();
                    }
                }
                return string.Empty;
            }

            catch (Exception exception)
            {
                return exception.Message;
            }

        }

        private string SaveUserEffortToDatabase(string roomName, int roomId)
        {
            try
            {
                foreach (var item in roomStatic.taskEffortList)
                {
                    var userId = GetUserId(item.Username);
                    var taskId = (from entity in _taskGaugeContext.Task
                                  where entity.Name.Equals(item.TaskName)
                                  select entity).FirstOrDefault().Id;

                    var getUserEffort = GetExistUserTaskEffortInformation(taskId, userId);

                    if (getUserEffort != null)
                    {
                        getUserEffort.EstimationTime = item.Effort.ToString();
                        _taskGaugeContext.SaveChanges();
                    }
                    else
                    {
                        _taskGaugeContext.UserEstimationLog.Add(new UserEstimationLog
                        {
                            EstimationTime = item.Effort.ToString(),
                            TaskId = taskId,
                            UserId = userId
                        });
                        _taskGaugeContext.SaveChanges();
                    }
                }
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }

        private string SaveTaskTotalEffortInformationToDatabase(string roomName, int roomId)
        {
            try
            {
                foreach (var item in roomStatic.totalTaskEffortInformation)
                {
                    var taskId = (from entity in _taskGaugeContext.Task
                                  where entity.Name.Equals(item.TaskName)
                                  select entity).FirstOrDefault().Id;

                    var getExistTotalTaskEffort = GetExistTotalTaskEffortInTheDatabase(taskId, roomId);

                    if (item.RoomName.Equals(roomName) && getExistTotalTaskEffort != null)
                    {
                        getExistTotalTaskEffort.TestEstimationTime = item.TesterTotalEffort.ToString();
                        getExistTotalTaskEffort.DevelopmentEstimationTime = item.DevTotalEffort.ToString();
                        getExistTotalTaskEffort.TotalEffort = item.TotalEffort.ToString();
                        _taskGaugeContext.SaveChanges();
                    }
                    else if (item.RoomName.Equals(roomName))
                    {
                        _taskGaugeContext.RoomTaskInformation.Add(new RoomTaskInformation
                        {
                            TestEstimationTime = item.TesterTotalEffort.ToString(),
                            DevelopmentEstimationTime = item.DevTotalEffort.ToString(),
                            RoomId = roomId,
                            TotalEffort = item.TotalEffort.ToString(),
                            TaskId = taskId,

                        });

                        _taskGaugeContext.SaveChanges();
                    }
                }
                return string.Empty;
            }

            catch (Exception exception)
            {
                return exception.Message;
            }

        }

        private int GetUserId(string username)
        {
            return _taskGaugeContext.User.FirstOrDefault(x => x.Name.Equals(username)).Id;
        }

        private RoomTaskInformation GetExistTotalTaskEffortInTheDatabase(int taskId, int roomId)
        {
            return _taskGaugeContext.RoomTaskInformation.FirstOrDefault(x => x.RoomId.Equals(roomId) && x.TaskId.Equals(taskId));
        }

        private UserEstimationLog GetExistUserTaskEffortInformation(int taskId, int userId)
        {
            return _taskGaugeContext.UserEstimationLog.FirstOrDefault(x => x.UserId.Equals(userId) && x.TaskId.Equals(taskId));
        }

        private bool IsTaskExist(int roomId, string taskName)
        {
            return _taskGaugeContext.Task.Any(x => x.RoomId.Equals(roomId) && x.Name.Equals(taskName));
        }

    }
}
