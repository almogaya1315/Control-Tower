using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CT.DAL.Entities
{
    [Table("Checkpoints")]
    public partial class Checkpoint
    {
        [Key]
        public int CheckpointId { get; set; }

        [Required]
        [StringLength(40)]
        public string CheckpointType { get; set; }

        [Required]
        public string Control { get; set; }

        [Required]
        public int Serial { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        [ForeignKey("Process")]
        public int ProcessId { get; set; }

        public virtual Process Process { get; set; }

        public virtual ICollection<Flight> Flights { get; set; }
    }
}
