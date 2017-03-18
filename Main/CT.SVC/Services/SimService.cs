using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CT.Simulator;
using CT.Common.IRepositories;
using CT.Common.Utilities;
using CT.Contracts.SimCallbacks;
using CT.BL.Repositories;
using CT.Contracts.SimDataContracts;
using CT.Contracts.SimOperationContracts;
using CT.Common.DTO_Models;

namespace CT.SVC.Services
{
    public class SimService : ISimService
    {
        ArrivalSim arrivalSim { get; set; }
        IControlTowerRepository CTRepo { get; set; }
        CallbackChannelFactory<ISimCallback> simChannelFactory { get; set; }

        public SimService()
        {
            arrivalSim = new ArrivalSim();
            CTRepo = new ControlTowerRepository();
            simChannelFactory = new CallbackChannelFactory<ISimCallback>();
        }

        public ResponseInitialFlightSerial GetInitialFlightSerial(RequestInitialFlightSerial req)
        {
            int flightSerial = arrivalSim.CreateFlightSerial();
            FlightDTO flight = CTRepo.CreateFlightObject(flightSerial);
            ResponseInitialFlightSerial resInitFlightSerial = 
                new ResponseInitialFlightSerial
                {
                    Flight = flight,
                    IsSuccess = true,
                    Message = $"Flight #{flight.FlightSerial} has been created."
                };
            return resInitFlightSerial;
        }
    }
}
