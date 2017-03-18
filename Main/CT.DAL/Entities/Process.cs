using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Diagnostics.CodeAnalysis;

namespace CT.DAL.Entities
{
    [Table("Processes")]
    public partial class Process
    {
        public Process()
        {
            Checkpoints = new List<Checkpoint>();
        }

        [Key]
        public int ProcessId { get; set; }

        [Required]
        [StringLength(30)]
        public string ProcessType { get; set; }

        public virtual ICollection<Checkpoint> Checkpoints { get; set; }
    }
}
