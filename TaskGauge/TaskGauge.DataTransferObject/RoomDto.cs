using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.DataTransferObject
{
    public class RoomDto
    {
        public string Username { get; set; }
        public string RoomName { get; set; }
        public bool IsItInTheRoom { get; set; }
    }
}
