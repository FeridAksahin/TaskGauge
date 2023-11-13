using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskGauge.Entity.Entity
{
    [Table("Task")]
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoomId { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; } 
        public int? ModifiedBy { get; set; } 
        public int RecordBy { get; set; }
        [ForeignKey("RecordBy")]
        public virtual User RecordUser { get; set; }
        [ForeignKey("ModifiedBy")]
        public virtual User ModifiedUser { get; set; }
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }
    }
}
