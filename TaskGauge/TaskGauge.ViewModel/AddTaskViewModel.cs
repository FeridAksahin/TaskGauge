using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.ViewModel
{
    public class AddTaskViewModel
    {
        public string TaskName { get; set; }
        public string Message { get; set; } 
        public bool IsSuccess { get; set; }
    }
}
