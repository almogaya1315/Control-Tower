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
using System.ComponentModel;
using System.Timers;
using System.Threading;
using System.Windows.Controls;
using CT.Common.Enums;

namespace CT.SVC.Services
{
    /// <summary>
    /// The simulator service class [single automatic instance, parallel threads]
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
        /// The flight creation interval & serial simualtor class initialization request method
        /// </summary>
        /// <param name="req">the request from the user side</param>
        /// <returns>the response to the user side</returns>
        public ResponseInitializeSimulator InitializeSimulator(RequestInitializeSimulator req)
        {
            double timerInterval = default(double);
            try
            {
                //retrieves a global timer fixed value
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
        /// The list of current flight in DB request method
        /// </summary>
        /// <returns>the response to the user side</returns>
        public ResponseFlightsCollection GetFlightsCollection()
        {
            ICollection<FlightDTO> flights = null;
            try
            {
                //retrieves the list from the repository
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
        /// The flight object creation request method
        /// </summary>
        /// <param name="req">the request from the user side</param>
        /// <returns>the response to the user side</returns>
        public ResponseFlightObject CreateFlightObject(RequestFlightObject req)
        {
            int flightSerial = default(int);
            FlightDTO flight = null;
            try
            {
                //gets a random serial
                flightSerial = arrivalSim.CreateFlightSerial();
                //creates a flight object in DB with a serial property only.
                //flight 'isAlive' prperty set to false
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
        /// The current flight promotion evaluation method
        /// </summary>
        /// <param name="req">the request from the user side</param>
        /// <returns>the response to the user side</returns>
        public ResponseFlightPosition GetFlightPosition(RequestFlightPosition req)
        {
            //the next checkpoint's control name
            string newCheckpointName = default(string);
            //the current flight's checkpoint's control name
            string lastCheckpointPosition = default(string);
            //the next checkpoint's serial
            int newCheckpointSerial = default(int);
            //the current checkpoint's serial
            int lastCheckpointSerial = default(int);
            FlightDTO flight = null;
            try
            {
                //get the flight object from DB by serial
                flight = ctRepo.GetFlightObject(int.Parse(req.FlightSerial));
                if (flight == null) throw new Exception("Flight serial was not found.");
                //retrieves the current flight's serial & checkpoint's control name from DB
                lastCheckpointPosition = ctRepo.GetFlightCheckpoint(req.TxtblckNameFlightNumberHash, req.LstvwNameFlightsListHash,
                    req.FlightSerial, req.IsBoarding, out lastCheckpointSerial);
                //the simulator class calulates the next checkpoint's serial & control name 
                newCheckpointName = timingSim.GetFlightPosition(req.TxtblckNameFlightNumberHash, req.LstvwNameFlightsListHash, flight, req.IsBoarding, out newCheckpointSerial);
            }
            catch (Exception e)
            {
                throw new Exception($"Flight #{req.FlightSerial} new position was not computed. {e.Message}");
            }
            //the next checkpoint's type enum
            string checkpointType = default(string);
            //if the next checkpoint is outside the airport, no DB update accurs in this method (will accure later by the dispose request)
            if (newCheckpointName == "Departed!") { }
            //if the last checkpoint is a terminal
            else if (lastCheckpointPosition == "txtblckFlightTerminal1" || lastCheckpointPosition == "txtblckFlightTerminal2")
            {
                //the next checkpoint is the current position
                newCheckpointName = lastCheckpointPosition;
                //updates checkpoint entities in DB with the new checkpoint credencials only, retrieves the checkpoint's type enum
                checkpointType = ctRepo.UpdateCheckpoints(newCheckpointSerial, newCheckpointName, lastCheckpointSerial, flight);
                //updates the flight entity in DB with the new & last checkpoint's serial, 
                //indicates if the flight is starting the landing process
                ctRepo.UpdateFlightObject(flight, newCheckpointSerial, lastCheckpointSerial, false);
            }
            //if the new checkpoint doesn't holds one of the following actions
            else if (newCheckpointName != "Stay in checkpoint!" && newCheckpointName != "No access to field!")
            {
                //updates checkpoint entities in DB with new & last checkpoint's credencials
                checkpointType = ctRepo.UpdateCheckpoints(newCheckpointSerial, lastCheckpointPosition, lastCheckpointSerial, flight);
                //if the last checkpoint doesn't holds the following action
                if (lastCheckpointPosition != "none")
                    //updates the flight entity in DB 
                    ctRepo.UpdateFlightObject(flight, newCheckpointSerial, lastCheckpointSerial, false);
            }

            //returns all the necessary data to the user side
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
        /// The current flight's checkpoint's duration request method
        /// </summary>
        /// <param name="req">the request from the user side</param>
        /// <returns>the response to the user side</returns>
        public ResponseCheckpointDuration GetCheckpointDuration(RequestCheckpointDuration req)
        {
            double duration = default(double);
            try
            {
                //if the flight' checkpoint has values
                if (req.CheckpointSerial != "-1" || req.CheckpointType != null)
                    //set the local value using the repository 
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
        /// The current flight's dispole request method
        /// </summary>
        /// <param name="req">the request from the user side</param>
        /// <returns>the response to the user side</returns>
        public ResponseDisposeFlight DisposeFlight(RequestDisposeFlight req)
        {
            //performs a dispose action on the flight object using the repository, returns a bool value
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