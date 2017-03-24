using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ComponentModel;
using CT.UI.SimulatorServiceReference;
using CT.Common.DTO_Models;
using System.Timers;

namespace CT.UI.Proxy
{
    /// <summary>
    /// The UI's proxy connection to simulator service
    /// </summary>
    public class SimServiceProxy : ISimService, ISimServiceCallback
    {
        #region proxy data
        ISimService simService { get; set; }

        public Dictionary<FlightDTO, Timer> flightsTimers { get; set; }

        public event EventHandler<bool> OnLoadEvent;
        public event EventHandler<FlightDTO> OnPromotionEvaluationEvent;
        public event EventHandler<int> OnDisposeEvent;

        /// <summary>
        /// Create the simulator service & initializes database
        /// </summary>
        public SimServiceProxy()
        {
            var serviceFactory = new DuplexChannelFactory<ISimService>(this,
                                 new WSDualHttpBinding(),
                                 new EndpointAddress("http://localhost:4767/Services/SimService.svc"));
            serviceFactory.Open();
            simService = serviceFactory.CreateChannel();

            try
            {
                InitializeDatabase();
            }
            catch (Exception e)
            {
                throw new Exception($"Database was not initialized. {e.Message}");
            }

            flightsTimers = new Dictionary<FlightDTO, Timer>();
        }

        /// <summary>
        /// Updates the proxy's hash of current flights & their timers
        /// </summary>
        /// <param name="flight">the current flight</param>
        /// <param name="toRemove">KeyValuePair to remove</param>
        /// <param name="toAdd">KeyValuePair to add</param>
        public void UpdateflightsTimersHash(FlightDTO flight, KeyValuePair<FlightDTO, Timer> toRemove, KeyValuePair<FlightDTO, Timer> toAdd)
        {
            if (flight != null)
            {
                flightsTimers.Remove(toRemove.Key);
                flightsTimers.Add(toAdd.Key, toAdd.Value);
            }
            else
            {
                flightsTimers[toRemove.Key].Dispose();
                flightsTimers.Remove(toRemove.Key);
                return;
            }
        }
        #endregion

        #region service methods
        public ResponseInitializeSimulator InitializeSimulator(RequestInitializeSimulator req)
        {
            return simService.InitializeSimulator(req);
        }

        public ResponseFlightsCollection GetFlightsCollection()
        {
            return simService.GetFlightsCollection();
        }

        public ResponseFlightObject CreateFlightObject(RequestFlightObject req)
        {
            return simService.CreateFlightObject(req);
        }

        public ResponseFlightPosition GetFlightPosition(RequestFlightPosition req)
        {
            return simService.GetFlightPosition(req);
        }

        public ResponseCheckpointDuration GetCheckpointDuration(RequestCheckpointDuration req)
        {
            return simService.GetCheckpointDuration(req);
        }

        public FlightDTO GetFlight(int flightSerial)
        {
            return simService.GetFlight(flightSerial);
        }

        public ResponseDisposeFlight DisposeFlight(RequestDisposeFlight req)
        {
            return simService.DisposeFlight(req);
        }

        public void InitializeDatabase()
        {
            simService.InitializeDatabase();
        }
        #endregion

        #region callback methods
        public void OnLoad(bool isLoaded)
        {
            OnLoadEvent?.Invoke(this, isLoaded);
        }

        public void OnPromotion(object flight)
        {
            if (flight is FlightDTO)
                OnPromotionEvaluationEvent?.Invoke(this, flight as FlightDTO);
        }

        public void OnDispose(int flightSerial)
        {
            OnDisposeEvent?.Invoke(this, flightSerial);
        }
        #endregion
    }
}
