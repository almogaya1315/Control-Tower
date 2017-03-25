using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.Abstracts
{
    /// <summary>
    /// An abstract base class for all response classes of the service's data contracts
    /// </summary>
    [DataContract]
    public abstract class ResponseBase
    {
        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string Message { get; set; }
    }
}
