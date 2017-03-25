using CT.Common.Abstracts;
using CT.Common.DTO_Models;
using CT.Common.Enums;
using CT.Common.Utilities;
using CT.UI.Proxy;
using CT.UI.SimulatorServiceReference;
using CT.UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace CT.UI.ViewModels
{
    /// <summary>
    /// The binding data tier for the UI, inherits from data abstract class
    /// </summary>
    public class AirportViewModel : CTBindingData
    {
        #region private props & ctor
        /// <summary>
        /// The simulator service proxy for the UI
        /// </summary>
        SimServiceProxy simProxy;
        /// <summary>
        /// The user control that connects to the view model
        /// </summary>
        AirportUserControl airportUserControl;

        /// <summary>
        /// A list of the checkpoints represented by 'TextBlock' controls
        /// </summary>
        ICollection<TextBlock> txtblckCheckpoints { get; set; }
        /// <summary>
        /// A list of the checkpoints represented by 'ListView' controls
        /// </summary>
        ICollection<ListView> lstvwsCheckpoints { get; set; }
        /// <summary>
        /// A list of the checkpoints images controls
        /// </summary>
        ICollection<Image> imgPlanes { get; set; }

        /// <summary>
        /// The constructor of the view model. called from the user control by a locator.
        /// </summary>
        /// <param name="control">the user control that calls the view model</param>
        /// <param name="proxy">the UI service proxy</param>
        public AirportViewModel(AirportUserControl control, SimServiceProxy proxy) : base()
        {
            airportUserControl = control;
            //the view model listens to the loaded event of the user control to start the flight object generator after load.
            airportUserControl.Loaded += airportUserControl_Loaded;

            simProxy = proxy;
            //the method that runs after the control loaded event
            simProxy.OnLoadEvent += SimProxy_OnLoadEvent;
            //the method that runs each flight object checkpoint promotion event 
            simProxy.OnPromotionEvaluationEvent += SimProxy_OnPromotionEvaluationEvent;
            //the method that runs when a flight object leaves the airport
            simProxy.OnDisposeEvent += SimProxy_OnDisposeEvent;

            txtblckCheckpoints = InitializeTxtblckCheckpoints(txtblckCheckpoints);
            lstvwsCheckpoints = InitializeLstvwsCheckpoints(lstvwsCheckpoints);
            imgPlanes = InitializeImgPlanes(imgPlanes);
        }
        #endregion

        #region ui events
        /// <summary>
        /// The user control loaded event method
        /// </summary>
        /// <param name="sender">the caller of the event</param>
        /// <param name="e">the event arguments of the event</param>
        void airportUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //after the page loads successfully, the proxy starts the flow
            if (sender is UserControl)
                simProxy.OnLoad((sender as UserControl).IsLoaded);
            else throw new Exception($"sender object is not a type of 'UserControl'.");
        }
        #endregion

        #region service events
        /// <summary>
        /// the proxy's onload event method
        /// </summary>
        /// <param name="sender">the caller of the event</param>
        /// <param name="isLoaded">a bool value of the control loaded event</param>
        void SimProxy_OnLoadEvent(object sender, bool isLoaded)
        {
            //a request is being made to the service to initalite the flights simulator
            RequestInitializeSimulator reqInitSim =
                new RequestInitializeSimulator() { IsWindowLoaded = isLoaded };
            ResponseInitializeSimulator resInitSim = simProxy.InitializeSimulator(reqInitSim);
            //if the simulator is initialized successfully, a timer starts with the simulator data 
            if (resInitSim.IsSuccess)
            {
                //resInitSim.TimerInterval => simulator data
                Timer arrivalTimer = new Timer(resInitSim.TimerInterval);
                arrivalTimer.Elapsed += CreateFlight_ArrivalTimerElapsed;
                arrivalTimer.Start();
            }
        }

        /// <summary>
        /// the simulator timer's elapsed event
        /// </summary>
        /// <param name="sender">the timer caller</param>
        /// <param name="e">the event arguments of the event</param>
        void CreateFlight_ArrivalTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //the timer pauses until the current flight is created fully
            if (sender is Timer) (sender as Timer).Stop();

            ResponseFlightObject resFlight = null;
            try
            {
                //a request is being made to the service to create a flight objest
                RequestFlightObject reqFlight = new RequestFlightObject()
                {
                    CurrentFlights = simProxy.GetFlightsCollection().Flights
                };
                resFlight = simProxy.CreateFlightObject(reqFlight);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not retrieve collection OR create flight object. {ex.Message}");
            }

            //if the fligts has been created successfully
            if (resFlight.IsSuccess)
            {
                //the first interval is retreived by the proxy from the first checkpoint
                double initialDuration = simProxy.GetCheckpointDuration(new RequestCheckpointDuration()
                { CheckpointSerial = "1", CheckpointType = CheckpointType.Landing.ToString() }).CheckpointDuration;
                //the current flight & a new timer, for checkpoint promotion, are being saved into the simproxy hash 
                simProxy.flightsTimers[resFlight.Flight] = new Timer(initialDuration);
                simProxy.flightsTimers[resFlight.Flight].Elapsed += PromotionTimer_Elapsed;

                simProxy.flightsTimers[resFlight.Flight].Start();
            }
            else throw new Exception("No success retrieving flight response.");

            //the object creation timer unpauses
            (sender as Timer).Start();
        }

        /// <summary>
        /// the current flight's promotion timer event
        /// </summary>
        /// <param name="sender">the timer caller</param>
        /// <param name="e">the event arguments of the event</param>
        void PromotionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //the promotion timer pauses until the current promotion evaluation finishes
            simProxy.flightsTimers.Values.FirstOrDefault(t => t == sender as Timer).Stop();

            FlightDTO flight = null;
            //the flight that the timer belongs to is retreived from the simproxy flight_timer
            foreach (FlightDTO fdto in simProxy.flightsTimers.Keys)
            {
                //the sender timer's hash code is compared
                if (simProxy.flightsTimers[fdto].GetHashCode() == sender.GetHashCode())
                {
                    //the proxy raises the onpromotion event
                    simProxy.OnPromotion(fdto);
                    //all additional data is retreived to the flight object
                    flight = simProxy.GetFlight(fdto.FlightSerial);
                    break;
                }
            }
            //after the last checkpoint, the flight & timer are disposed, so no need to unpause the promotion timer
            if (flight != null)
                simProxy.flightsTimers.FirstOrDefault(pair => pair.Key.FlightSerial == flight.FlightSerial).Value.Start();
        }

        /// <summary>
        /// The flight's checkpoint promotion evaluation method 
        /// </summary>
        /// <param name="sender">the proxy caller</param>
        /// <param name="flight">the current flight object</param>
        void SimProxy_OnPromotionEvaluationEvent(object sender, FlightDTO flight)
        {
            //a request is being made to the service in order to calculate the flight's next checkpoint
            RequestFlightPosition reqPosition = new RequestFlightPosition()
            {
                //a hash of all the checkpoints flight serial
                TxtblckNameFlightNumberHash = SetTxtblckHash(txtblckCheckpoints),
                //a hash of the the stand-by flight serial list
                LstvwNameFlightsListHash = SetLstvwHash(lstvwsCheckpoints),
                //the cuurent flight's serial
                FlightSerial = flight.FlightSerial.ToString(),
                //bool value for is flight boarding
                IsBoarding = EvaluateTerminalState(flight)
            };

            //the service retrieves all the data for the flight's next checkpoint & updates database
            ResponseFlightPosition resPosition = simProxy.GetFlightPosition(reqPosition);

            /**simproxy flight_timer hash update**/
            //values to remove from the simproxy flight_timer hash
            KeyValuePair<FlightDTO, Timer> keyToRemove = new KeyValuePair<FlightDTO, Timer>(flight, simProxy.flightsTimers[flight]);
            //flight object with values in 'FlightSerial' property only
            FlightDTO previousFlightObject = flight; 
            //flight object with full properties 
            flight = simProxy.GetFlight(flight.FlightSerial);
            //values to replace the current flight_timer values in the simproxy hash
            KeyValuePair<FlightDTO, Timer> keyToAdd = new KeyValuePair<FlightDTO, Timer>(flight, simProxy.flightsTimers[previousFlightObject]);
            simProxy.UpdateflightsTimersHash(flight, keyToRemove, keyToAdd);

            if (resPosition.IsSuccess)
            {
                /**current flight's timer's interval set to next checkpoint's duration**/
                //if the next checkpoint in not to land or depart, 
                if (flight.Checkpoint != null && resPosition.NextCheckpointName != "Departed!")
                {
                    //set timer's interval to updated flight's checkpoint's duration
                    double duration = flight.Checkpoint.Duration;
                    simProxy.flightsTimers[flight].Interval = duration;
                }
                //if the next checkpoint serial & type has values,
                else if (resPosition.CheckpointSerial != -1 && resPosition.CheckpointType != null)
                {
                    //request is made to service to retrieve the next checkpoint object 
                    RequestCheckpointDuration reqDur = new RequestCheckpointDuration()
                    { CheckpointSerial = resPosition.CheckpointSerial.ToString() };
                    ResponseCheckpointDuration resDur = simProxy.GetCheckpointDuration(reqDur);
                    //and set timer's interval to next checkpoint duration
                    if (resDur.IsSuccess)
                    { simProxy.flightsTimers[flight].Interval = resDur.CheckpointDuration; }
                }

                /**binding data update**/
                //if the last flight's position was in terminal 
                if (resPosition.LastCheckpointPosition == "txtblckFlightTerminal1" || resPosition.LastCheckpointPosition == "txtblckFlightTerminal2")
                {
                    //update the binding data by next checkpoint serial
                    SwitchOnCheckpointSerial(airportUserControl.Dispatcher, resPosition.CheckpointSerial, resPosition.CheckpointType,
                        resPosition.NextCheckpointName, resPosition.LastCheckpointPosition, flight);
                    return;
                }
                //update binding data by next checkpoint's textblock or listview name property
                bool? isFound = SwitchOnNextCheckpointName(airportUserControl.Dispatcher, resPosition.NextCheckpointName, flight);
                //if last switch returns null, 
                if (isFound == null)
                {
                    //check if the flight's next checkpoint is to cercle around or depart
                    if (resPosition.NextCheckpointName == "Departed!" || resPosition.NextCheckpointName == "No access to field!")
                    {
                        //dispose flight
                        simProxy.OnDispose(flight.FlightSerial);
                        KeyValuePair<FlightDTO, Timer> keyToDispose = new KeyValuePair<FlightDTO, Timer>(flight, simProxy.flightsTimers[flight]);
                        //dispose timer
                        simProxy.UpdateflightsTimersHash(null, keyToDispose, new KeyValuePair<FlightDTO, Timer>());
                    }
                    return;
                }
                //if the last switch returns false,
                if (isFound == false)
                {
                    //update the binding data by next checkpoint serial for all other checkpoints
                    SwitchOnCheckpointSerial(airportUserControl.Dispatcher, resPosition.CheckpointSerial, resPosition.CheckpointType,
                        resPosition.NextCheckpointName, resPosition.LastCheckpointPosition, flight);
                }
            }
        }

        /// <summary>
        /// The simproxy flight object dispose event mothed
        /// </summary>
        /// <param name="sender">the sender object</param>
        /// <param name="flightSerial">the flight serial to dispose</param>
        void SimProxy_OnDisposeEvent(object sender, int flightSerial)
        {
            //a request is made to service to delete flight from DB
            RequestDisposeFlight reqDis = new RequestDisposeFlight() { FlightSerial = flightSerial };
            ResponseDisposeFlight resDis = simProxy.DisposeFlight(reqDis);
            if (resDis.IsSuccess)
            {
                //last checkpoint binding data cleared
                FlightInDeparted = InitializeFlightBindingObject();
                return;
            }
            else throw new Exception("[UI] Service was unable to dispose the flight.");
        }
        #endregion

        #region private methods
        /// <summary>
        /// Finds all checkpoints TextBlock controls
        /// </summary>
        /// <param name="txtblckCheckpoints">the view model's list</param>
        /// <returns>the initialized list</returns>
        ICollection<TextBlock> InitializeTxtblckCheckpoints(ICollection<TextBlock> txtblckCheckpoints) //UIElementCollection children, 
        {
            return txtblckCheckpoints = new List<TextBlock>()
            {
                airportUserControl.txtblckFlightArr1, airportUserControl.txtblckFlightArr2, airportUserControl.txtblckFlightArr3,
                airportUserControl.txtblckFlightRunway, airportUserControl.txtblckFlightTerminal1,
                airportUserControl.txtblckFlightTerminal2, airportUserControl.txtblckFlightDepart
            };
        }

        /// <summary>
        /// Finds all checkpoints ListView controls
        /// </summary>
        /// <param name="lstvwsCheckpoints">the view model's list</param>
        /// <returns>the initialized list</returns>
        ICollection<ListView> InitializeLstvwsCheckpoints(ICollection<ListView> lstvwsCheckpoints) //UIElementCollection children,
        {
            return lstvwsCheckpoints = new List<ListView>()
            {
                airportUserControl.lstvwParkUnload, airportUserControl.lstvwParkDepart
            };
        }

        /// <summary>
        /// Finds all Image controls
        /// </summary>
        /// <param name="imgPlanes">the view model's list</param>
        /// <returns>the initialized list</returns>
        ICollection<Image> InitializeImgPlanes(ICollection<Image> imgPlanes) //UIElementCollection children, 
        {
            return imgPlanes = new List<Image>()
            {
                airportUserControl.imgPlaneArr1, airportUserControl.imgPlaneArr2, airportUserControl.imgPlaneArr3,
                airportUserControl.imgPlaneRunway, airportUserControl.imgPlaneTerminal1,
                airportUserControl.imgPlaneTerminal2, airportUserControl.imgPlanDepart
            };
        }

        /// <summary>
        /// Sets the checkpoint's flight serial data per promotion
        /// </summary>
        /// <param name="txtblckCheckpoints">the view model's initialized list</param>
        /// <returns>hash of TextBlock name property & flight serial property</returns>
        Dictionary<string, string> SetTxtblckHash(ICollection<TextBlock> txtblckCheckpoints)
        {
            Dictionary<string, string> txtblckNameFlightNumberHash = new Dictionary<string, string>();
            airportUserControl.Dispatcher.Invoke(() =>
            {
                foreach (TextBlock txtblck in txtblckCheckpoints)
                    txtblckNameFlightNumberHash[txtblck.Name] = txtblck.Text;
            });
            return txtblckNameFlightNumberHash;
        }

        /// <summary>
        /// Sets the stand-by checkpoint's flight serial list data per promotion
        /// </summary>
        /// <param name="lstvwsCheckpoints">the view model's initialized list</param>
        /// <returns>hash of ListView name property & list of flight serial property</returns>
        Dictionary<string, string[]> SetLstvwHash(ICollection<ListView> lstvwsCheckpoints)
        {
            Dictionary<string, string[]> lstvwNameFlightsListHash = new Dictionary<string, string[]>();

            airportUserControl.Dispatcher.Invoke(() =>
            {
                foreach (ListView lstvw in lstvwsCheckpoints)
                {
                    lstvwNameFlightsListHash[lstvw.Name] = new string[100];
                    if (lstvw.Items.Count > 0)
                    {
                        foreach (string lvi in lstvw.Items)
                        {
                            List<string> list = lstvwNameFlightsListHash[lstvw.Name].ToList();
                            list.RemoveAll(i => i == null);
                            list.Add(lvi);
                            lstvwNameFlightsListHash[lstvw.Name] = list.ToArray();
                        }
                    }
                }
            });

            return lstvwNameFlightsListHash;
        }
        #endregion
    }
}
