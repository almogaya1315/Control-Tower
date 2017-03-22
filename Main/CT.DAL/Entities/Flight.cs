using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace CT.DAL.Entities
{
    [Table("Flights")]
    public partial class Flight
    {
        [Key]
        public int FlightId { get; set; }

        [Required]
        public int FlightSerial { get; set; }

        [Required]
        public bool IsAlive { get; set; }

        //[ForeignKey("Checkpoint")]
        public int? CheckpointId { get; set; }

        public string CheckpointControl { get; set; }

        public virtual Checkpoint Checkpoint { get; set; }
    }
}
