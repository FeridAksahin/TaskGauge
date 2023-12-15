using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.DataTransferObject
{
    public class TaskDto
    { 
        public string TaskName { get; set; } 
        public DateTime RecordDate { get; set; } = DateTime.Now;  
        public string RecordByName { get; set; }   
        public string RoomName { get; set; }
    }
}
