using CT.Common.Abstracts;
using CT.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CT.Contracts.SimDataContracts
{
    [DataContract]
    public class ResponseFlightPosition : ResponseBase
    {
        [DataMember]
        public string NextCheckpointName;

        [DataMember]
        public int CheckpointSerial { get; set; }

        [DataMember]
        public string CheckpointType { get; set; }

        [DataMember]
        public string LastCheckpointPosition { get; set; }
    }
}
