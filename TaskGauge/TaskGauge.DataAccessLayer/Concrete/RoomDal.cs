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
        RoomStatic roomUser = RoomStatic.Instance;
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
            foreach(var room in rooms)
            {
                roomUser.room.Add(new DataTransferObject.Room
                {
                    Name = room.Name,
                    isActive = room.isActive
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
                        select task;
            foreach (var task in tasks)
            {
                roomUser.allRoomTask.Add(new DataTransferObject.TaskDto
                {
                    RecordBy = task.RecordBy,
                    RecordDate = task.RecordDate,
                    RoomName = task.Room.Name,
                    TaskName = task.Name
                });
            }
        }
    }
}
