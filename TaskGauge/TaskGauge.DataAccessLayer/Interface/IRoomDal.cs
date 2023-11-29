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
    }
}
