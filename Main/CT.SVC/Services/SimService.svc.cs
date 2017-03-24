﻿using System;
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
using System.ComponentModel;
using System.Timers;
using System.Threading;
using System.Windows.Controls;
using CT.Common.Enums;

namespace CT.SVC.Services
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class SimService : ISimService
    {
        #region private props & ctor
        ArrivalSim arrivalSim { get; set; }
        TimingSystem timingSim { get; set; }
        IControlTowerRepository ctRepo { get; set; }

        public SimService()
        {
            arrivalSim = new ArrivalSim();
            timingSim = new TimingSystem();
            ctRepo = new ControlTowerRepository();
        }
        #endregion

        #region requset\response methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ResponseInitializeSimulator InitializeSimulator(RequestInitializeSimulator req)
        {
            double timerInterval = default(double);
            try
            {
                timerInterval = arrivalSim.ArrivalTimer.Interval;
            }
            catch (Exception e)
            {
                throw new Exception($"Could not create timer. {e.Message}");
            }
            return new ResponseInitializeSimulator()
            {
                IsSuccess = true,
                Message = $"Arrival object generator timer created with {timerInterval} interval.",
                TimerInterval = timerInterval,
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ResponseFlightsCollection GetFlightsCollection()
        {
            ICollection<FlightDTO> flights = null;
            try
            {
                flights = ctRepo.GetFlightsCollection();
            }
            catch (Exception e)
            {
                throw new Exception($"Could not retrieve flights collection from db. {e.Message}");
            }
            return new ResponseFlightsCollection()
            {
                Flights = flights,
                IsSuccess = true,
                //Message = $"Collection retrieved successfully from db with total of {flights.Count} flights."
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ResponseFlightObject CreateFlightObject(RequestFlightObject req)
        {
            int flightSerial = default(int);
            FlightDTO flight = null;
            try
            {
                flightSerial = arrivalSim.CreateFlightSerial();
                flight = ctRepo.CreateFlightObject(flightSerial);
            }
            catch (Exception e)
            {
                throw new Exception($"Flight serial OR Flight object could not bet created. {e.Message}");
            }

            return new ResponseFlightObject
            {
                Flight = flight,
                IsSuccess = true,
                Message = $"Flight #{flight.FlightSerial} has been created."
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ResponseFlightPosition GetFlightPosition(RequestFlightPosition req)
        {
            string newCheckpointName = default(string);
            string lastCheckpointPosition = default(string);
            int newCheckpointSerial = default(int);
            int lastCheckpointSerial = default(int);
            FlightDTO flight = null;
            try
            {
                flight = ctRepo.GetFlightObject(int.Parse(req.FlightSerial));
                if (flight == null) throw new Exception("Flight serial was not found.");
                lastCheckpointPosition = ctRepo.GetFlightCheckpoint(req.TxtblckNameFlightNumberHash, req.LstvwNameFlightsListHash,
                    req.FlightSerial, req.IsBoarding, out lastCheckpointSerial);
                newCheckpointName = timingSim.GetFlightPosition(req.TxtblckNameFlightNumberHash, req.LstvwNameFlightsListHash, flight, req.IsBoarding, out newCheckpointSerial);
            }
            catch (Exception e)
            {
                throw new Exception($"Flight #{req.FlightSerial} new position was not computed. {e.Message}");
            }
            string checkpointType = default(string);
            if (newCheckpointName == "Departed!") { }
            else if (lastCheckpointPosition == "txtblckFlightTerminal1" || lastCheckpointPosition == "txtblckFlightTerminal2")
            {
                newCheckpointName = lastCheckpointPosition;
                checkpointType = ctRepo.UpdateCheckpoints(newCheckpointSerial, newCheckpointName, lastCheckpointSerial, flight);
                ctRepo.UpdateFlightObject(flight, newCheckpointSerial, lastCheckpointSerial, false);
            }
            else if (newCheckpointName != "Stay in checkpoint!" && newCheckpointName != "No access to field!")
            {
                checkpointType = ctRepo.UpdateCheckpoints(newCheckpointSerial, lastCheckpointPosition, lastCheckpointSerial, flight);
                if (lastCheckpointPosition != "none")
                    ctRepo.UpdateFlightObject(flight, newCheckpointSerial, lastCheckpointSerial, false);
            }
            return new ResponseFlightPosition()
            {
                IsSuccess = true,
                Message = $"Flight #{req.FlightSerial} new position retrieved.",
                NextCheckpointName = newCheckpointName,
                CheckpointSerial = newCheckpointSerial,
                CheckpointType = checkpointType,
                LastCheckpointPosition = lastCheckpointPosition
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ResponseCheckpointDuration GetCheckpointDuration(RequestCheckpointDuration req)
        {
            double duration = default(double);
            try
            {
                if (req.CheckpointSerial != "-1" || req.CheckpointType != null)
                    duration = ctRepo.GetCheckpoint(req.CheckpointSerial, req.CheckpointType).Duration;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return new ResponseCheckpointDuration()
            {
                IsSuccess = true,
                CheckpointDuration = duration
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ResponseDisposeFlight DisposeFlight(RequestDisposeFlight req)
        {
            bool isDisposed = ctRepo.DisposeFlight(req.FlightSerial);
            if (isDisposed)
            {
                return new ResponseDisposeFlight()
                {
                    Message = $"Flight #{req.FlightSerial} was disposed.",
                    IsSuccess = isDisposed
                };
            }
            else throw new Exception("Unable to dispose flight object.");
        }
        #endregion

        #region direct methods
        public FlightDTO GetFlight(int flightSerial)
        {
            return ctRepo.GetFlightObject(flightSerial);
        }

        public void InitializeDatabase()
        {
            ctRepo.InitializeDatabase();
        }
        #endregion
    }
}