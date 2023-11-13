using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.Entity.Entity
{
    [Table("UserEstimationLog")]
    public class UserEstimationLog
    {
        [Key]
        public int Id { get; set; } 
        public int UserId { get; set; }  
        public int TaskId { get; set; }
        public string EstimationTime { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }
    }
}
