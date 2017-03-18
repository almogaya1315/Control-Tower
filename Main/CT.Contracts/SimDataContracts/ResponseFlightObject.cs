using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using CT.Common.Abstracts;
using CT.Common.DTO_Models;
using System.Threading;

namespace CT.Contracts.SimDataContracts
{
    [DataContract]
    public class ResponseFlightObject : ResponseBase
    {
        [DataMember]
        public FlightDTO Flight { get; set; }
    }
}
