using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskGauge.Common;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.Entity.Context;
using TaskGauge.Entity.Entity;

namespace TaskGauge.DataAccessLayer.Concrete
{
    public class RoomDal : IRoomDal
    {
        RoomStatic roomStatic = RoomStatic.Instance;
        private TaskGaugeContext _taskGaugeContext;
        private UserInformation _userInformation;
        public RoomDal(TaskGaugeContext taskGaugeContext, UserInformation userInformation)
        {
            _userInformation = userInformation;
            _taskGaugeContext = taskGaugeContext;
        }

        public bool IsRoomExist(string roomName)
        {
            return _taskGaugeContext.Room.ToList().Exists(x => x.Name.Trim().Equals(roomName.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public bool AddRoom(string roomName)
        {
            _taskGaugeContext.Room.Add(new Room { Name = roomName, RoomAdminId = _userInformation.GetUserIdFromCookie() });
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

        public void GetAllTaskIntoStaticList()
        {
            var tasks = from task in _taskGaugeContext.Task
                        select new
                        {
                            Task = task,
                            RoomName = task.Room.Name
                        };
            foreach (var task in tasks)
            {
                roomStatic.allRoomTask.Add(new DataTransferObject.TaskDto
                {
                    RecordBy = task.Task.RecordBy,
                    RecordDate = task.Task.RecordDate,
                    RoomName = task.RoomName,
                    TaskName = task.Task.Name
                });
            }

        }

        public void SaveToDatabase(string roomName)
        {
            SaveTaskToDatabase(roomName);
            SaveTaskEffortInformationToDatabase(roomName);
        }

        public void SaveTaskToDatabase(string roomName)
        {
            var roomId = _taskGaugeContext.Room.ToList().Where(x => x.Name.Equals(roomName)).FirstOrDefault().Id;
            foreach (var task in roomStatic.allRoomTask)
            {
                if (task.RoomName.Equals(roomName) && !IsTaskExist(roomId, task.TaskName))
                {
                    _taskGaugeContext.Task.Add(new Entity.Entity.Task { RoomId = roomId, Name = task.TaskName, RecordBy = _userInformation.GetUserIdFromCookie() });
                    _taskGaugeContext.SaveChanges();
                }
            }
        }

        private void SaveTaskEffortInformationToDatabase(string roomName)
        {
            var roomId = _taskGaugeContext.Room.Where(x => x.Name.Equals(roomName)).FirstOrDefault().Id;
            foreach (var item in roomStatic.totalTaskEffortInformation)
            {
                var taskId = (from entity in _taskGaugeContext.Task
                              where entity.Name.Equals(item.TaskName)
                              select entity).FirstOrDefault().Id;

                var existTask = GetExistTaskEffortInTheDatabase(taskId, roomId);

                if (item.RoomName.Equals(roomName) && existTask != null)
                {
                    existTask.TestEstimationTime = item.TesterTotalEffort.ToString();
                    existTask.DevelopmentEstimationTime = item.DevTotalEffort.ToString();
                    existTask.TotalEffort = item.TotalEffort.ToString();
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
        }

        private RoomTaskInformation GetExistTaskEffortInTheDatabase(int taskId, int roomId)
        {
            return _taskGaugeContext.RoomTaskInformation.FirstOrDefault(x => x.RoomId.Equals(roomId) && x.TaskId.Equals(taskId));
        }

        private bool IsTaskExist(int roomId, string taskName)
        {
            return _taskGaugeContext.Task.Any(x => x.RoomId.Equals(roomId) && x.Name.Equals(taskName));
        }

    }
}
