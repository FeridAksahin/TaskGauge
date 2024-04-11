using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskGauge.DataTransferObject;

namespace TaskGauge.DataAccessLayer.Interface
{
    public interface IRoomDal
    {
        public bool IsRoomExist(string roomName);
        public bool AddRoom(string roomName);
        public bool IsTheLoggedInUserTheRoomAdministrator(string roomName);
        public void GetAllRoomIntoStaticList();
        public void GetAllTaskIntoRedisList();
        public bool IsExistRoomName(string roomName);
        public string SaveToDatabase(string roomName);
        public List<TaskDto> GetAllRoomTaskFromRedis();
    }
}
