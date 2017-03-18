using CT.Common.Abstracts;
using CT.Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CT.Contracts.SimDataContracts
{
    [DataContract]
    public class ResponseInitializeSimulator : ResponseBase
    {
        [DataMember]
        public double TimerInterval { get; set; }
    }
}
