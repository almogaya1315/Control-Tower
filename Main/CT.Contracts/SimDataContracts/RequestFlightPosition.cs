using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CT.Contracts.SimDataContracts
{
    [DataContract]
    public class RequestFlightPosition
    {
        [DataMember]
        public Dictionary<string, string> TxtblckNameFlightNumberHash { get; set; }

        [DataMember]
        public Dictionary<string, List<string>> LstvwNameFlightsListHash { get; set; }

        //[DataMember]
        //public ICollection<object> TextBlocksCheckpoints { get; set; }

        //[DataMember]
        //public ICollection<object> ListViewsCheckpoints { get; set; }

        [DataMember]
        public string FlightSerial { get; set; }

        [DataMember]
        public bool IsBoarding { get; set; }
    }
}
