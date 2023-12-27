using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.DataTransferObject
{
    public class RoomUserDto
    {
        public string Username { get; set; }
        public string RoomName { get; set; }
        public bool IsItInTheRoom { get; set; }
        public string ConnectionId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public int UserId { get; set; }
    }
}
