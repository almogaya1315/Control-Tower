using CT.Common.Abstracts;
using CT.Common.DTO_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CT.Contracts.SimDataContracts
{
    [DataContract]
    public class ResponseFlightsCollection : ResponseBase
    {
        [DataMember]
        public ICollection<FlightDTO> Flights { get; set; }
    }
}
