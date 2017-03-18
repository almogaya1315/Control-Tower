using CT.Common.DTO_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CT.Contracts.SimCallbacks
{
    [ServiceContract]
    public interface ISimCallback
    {
        [OperationContract]
        void OnLoad(bool isLoaded);

        [OperationContract]
        void OnPromotion(object flightSerial);

        [OperationContract]
        void OnDispose(int flightSerial);
    }
}
