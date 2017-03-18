using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CT.Common.DTO_Models
{
    public class FlightDTO
    {
        public int FlightId { get; set; }
        public int FlightSerial { get; set; }
        public bool IsAlive { get; set; }
        public int ProcessId { get; set; }
        public int CheckpointId { get; set; }
        public virtual CheckpointDTO Checkpoint { get; set; }
        public virtual ProcessDTO Process { get; set; }
    }
}
