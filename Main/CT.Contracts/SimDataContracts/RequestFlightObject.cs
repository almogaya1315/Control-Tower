using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using CT.Common.DTO_Models;

namespace CT.Contracts.SimDataContracts
{
    [DataContract]
    public class RequestFlightObject
    {
        [DataMember]
        public ICollection<FlightDTO> CurrentFlights { get; set; }
    }
}
