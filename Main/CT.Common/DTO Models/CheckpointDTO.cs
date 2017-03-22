using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.DTO_Models
{
    public class CheckpointDTO
    {
        public CheckpointDTO()
        {
            Flights = new List<FlightDTO>();
        }

        public int CheckpointId { get; set; }
        public string CheckpointType { get; set; }
        public string Control { get; set; }
        public int Serial { get; set; }
        public int Duration { get; set; }
        public int ProcessId { get; set; }
        public virtual ProcessDTO Process { get; set; }
        public virtual ICollection<FlightDTO> Flights { get; set; }
    }
}
