﻿using CT.Common.DTO_Models;
using CT.Contracts.SimCallbacks;
using CT.Contracts.SimDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace CT.Contracts.SimOperationContracts
{
    [ServiceContract(CallbackContract = (typeof(ISimCallback)))]
    public interface ISimService
    {
        [OperationContract]
        ResponseInitializeSimulator InitializeSimulator(RequestInitializeSimulator req);

        [OperationContract]
        ResponseFlightsCollection GetFlightsCollection();

        [OperationContract]
        ResponseFlightObject CreateFlightObject(RequestFlightObject req);

        [OperationContract]
        ResponseFlightPosition GetFlightPosition(RequestFlightPosition req);

        [OperationContract]
        ResponseCheckpointDuration GetCheckpointDuration(RequestCheckpointDuration req);

        [OperationContract]
        ResponseDisposeFlight DisposeFlight(RequestDisposeFlight req);

        [OperationContract]
        FlightDTO GetFlight(int flightSerial);

        [OperationContract]
        void InitializeDatabase();
    }
}
