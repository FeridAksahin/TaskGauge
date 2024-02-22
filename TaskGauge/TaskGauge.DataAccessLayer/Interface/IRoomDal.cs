using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.DataAccessLayer.Interface
{
    public interface IRoomDal
    {
        public bool IsRoomExist(string roomName);
        public bool AddRoom(string roomName);
        public bool IsTheLoggedInUserTheRoomAdministrator(string roomName);
        public void GetAllRoomIntoStaticList();
        public void GetAllTaskIntoStaticList();
        public bool IsExistRoomName(string roomName);
        public void SaveToDatabase(string roomName);
    }
}
