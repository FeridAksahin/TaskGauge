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
    }
}
