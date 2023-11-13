using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.Entity.Entity
{
    [Table("Room")]
    public class Room
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoomAdminId { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.Now;
        [ForeignKey("RoomAdminId")]
        public virtual User RoomAdmin { get; set; }
    }
}
