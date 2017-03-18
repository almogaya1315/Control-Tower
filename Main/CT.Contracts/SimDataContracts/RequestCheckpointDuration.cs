using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CT.Contracts.SimDataContracts
{
    [DataContract]
    public class RequestCheckpointDuration
    {
        [DataMember]
        public string CheckpointSerial { get; set; }

        [DataMember]
        public string CheckpointType { get; set; }
    }
}
