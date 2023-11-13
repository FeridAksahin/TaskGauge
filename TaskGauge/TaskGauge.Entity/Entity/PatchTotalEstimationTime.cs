﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.Entity.Entity
{
    [Table("PatchTotalEstimationTime")]
    public class PatchTotalEstimationTime
    {
        [Key]
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string DevelopmentEstimationTime { get; set; }
        public string TestEstimationTime { get; set; }
        public int TestBufferHour { get; set; }
        public int DevelopmentBufferHour { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
    }
}
