using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.Entity.Entity
{
    [Table("SecurityQuestion")]
    public class SecurityQuestion
    {
        [Key]
        public int Id { get; set; }
        public string Question { get; set; }
    }
}
