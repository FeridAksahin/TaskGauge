﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.Entity.Entity
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Password { get; set; }    
        public DateTime RecordTime { get; set; } = DateTime.Now;
        public int SecurityQuestionId { get; set; }
        public string SecurityQuestionAnswer { get; set; }
        [ForeignKey("SecurityQuestionId")]
        public virtual SecurityQuestion SecurityQuestion { get; set; }
    }
}
