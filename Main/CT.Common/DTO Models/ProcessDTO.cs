using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.DTO_Models
{
    public class ProcessDTO
    {
        public ProcessDTO()
        {
            Checkpoints = new List<CheckpointDTO>();
            Flights = new List<FlightDTO>();
        }

        public int ProcessId { get; set; }
        public string ProcessType { get; set; }
        public virtual ICollection<CheckpointDTO> Checkpoints { get; set; }
        public virtual ICollection<FlightDTO> Flights { get; set; }
    }
}
