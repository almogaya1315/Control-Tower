using CT.Common.Abstracts;
using CT.Common.Commands;
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="flight"></param>
        void SimProxy_OnPromotionEvaluationEvent(object sender, FlightDTO flight)
        {
            RequestFlightPosition reqPosition = new RequestFlightPosition()
            {
                TxtblckNameFlightNumberHash = SetTxtblckHash(txtblckCheckpoints),
                LstvwNameFlightsListHash = SetLstvwHash(lstvwsCheckpoints),
                FlightSerial = flight.FlightSerial.ToString(),
                IsBoarding = EvaluateTerminalState(flight)
            };

            ResponseFlightPosition resPosition = simProxy.GetFlightPosition(reqPosition);

            KeyValuePair<FlightDTO, Timer> keyToRemove = new KeyValuePair<FlightDTO, Timer>(flight, simProxy.flightsTimers[flight]);
            FlightDTO previousFlightObject = flight; // object with values in 'FlightSerial' property only
            flight = simProxy.GetFlight(flight.FlightSerial);
            KeyValuePair<FlightDTO, Timer> keyToAdd = new KeyValuePair<FlightDTO, Timer>(flight, simProxy.flightsTimers[previousFlightObject]);
            simProxy.UpdateflightsTimersHash(flight, keyToRemove, keyToAdd);

            if (resPosition.IsSuccess)
            {
                if (flight.Checkpoint != null && resPosition.NextCheckpointName != "Departed!")
                {
                    double duration = flight.Checkpoint.Duration;
                    simProxy.flightsTimers[flight].Interval = duration;
                }
                else if (resPosition.CheckpointSerial != -1 && resPosition.CheckpointType != null)
                {
                    RequestCheckpointDuration reqDur = new RequestCheckpointDuration()
                    { CheckpointSerial = resPosition.CheckpointSerial.ToString() };
                    ResponseCheckpointDuration resDur = simProxy.GetCheckpointDuration(reqDur);
                    if (resDur.IsSuccess)
                    { simProxy.flightsTimers[flight].Interval = resDur.CheckpointDuration; }
                }

                if (resPosition.LastCheckpointPosition == "txtblckFlightTerminal1" || resPosition.LastCheckpointPosition == "txtblckFlightTerminal2")
                {
                    SwitchOnCheckpointSerial(airportUserControl.Dispatcher, resPosition.CheckpointSerial, resPosition.CheckpointType,
                        resPosition.NextCheckpointName, resPosition.LastCheckpointPosition, flight);
                    return;
                }
                bool? isFound = SwitchOnNextCheckpointName(airportUserControl.Dispatcher, resPosition.NextCheckpointName, flight);
                if (isFound == null)
                {
                    if (resPosition.NextCheckpointName == "Departed!" || resPosition.NextCheckpointName == "No access to field!")
                    {
                        simProxy.OnDispose(flight.FlightSerial);
                        KeyValuePair<FlightDTO, Timer> keyToDispose = new KeyValuePair<FlightDTO, Timer>(flight, simProxy.flightsTimers[flight]);
                        simProxy.UpdateflightsTimersHash(null, keyToDispose, new KeyValuePair<FlightDTO, Timer>());
                    }
                    return;
                }
                if (isFound == false)
                {
                    SwitchOnCheckpointSerial(airportUserControl.Dispatcher, resPosition.CheckpointSerial, resPosition.CheckpointType,
                        resPosition.NextCheckpointName, resPosition.LastCheckpointPosition, flight);
                }
            }
        }
        void SimProxy_OnDisposeEvent(object sender, int flightSerial)
        {
            RequestDisposeFlight reqDis = new RequestDisposeFlight() { FlightSerial = flightSerial };
            ResponseDisposeFlight resDis = simProxy.DisposeFlight(reqDis);
            if (resDis.IsSuccess)
            {
                FlightInDeparted = InitializeFlightBindingObject();

                //airportUserControl.txtblckFlightDepart.Text = "---";
                //airportUserControl.imgPlanDepart.Source = PlaneImageSource.NoPlane;
                return;
            }
            else throw new Exception("[UI] Service was unable to dispose the flight.");
        }
        #endregion

        #region public commands
        public ICommand AddFlightCommand { get; set; }

        public void AddFlight()
        {

        }
        #endregion

        #region private methods
        ICollection<TextBlock> InitializeTxtblckCheckpoints(ICollection<TextBlock> txtblckCheckpoints) //UIElementCollection children, 
        {
            //foreach (UIElement element in children)
            //    if (element.GetType() != typeof(TextBlock))
            //        if ((element as TextBlock).Name.Contains("txtblckFlight"))
            //            txtblckCheckpoints.Add(element as TextBlock);
            //return txtblckCheckpoints;

            return txtblckCheckpoints = new List<TextBlock>()
            {
                airportUserControl.txtblckFlightArr1, airportUserControl.txtblckFlightArr2, airportUserControl.txtblckFlightArr3,
                airportUserControl.txtblckFlightRunway, airportUserControl.txtblckFlightTerminal1,
                airportUserControl.txtblckFlightTerminal2, airportUserControl.txtblckFlightDepart
            };
        }
        ICollection<ListView> InitializeLstvwsCheckpoints(ICollection<ListView> lstvwsCheckpoints) //UIElementCollection children,
        {
            //foreach (UIElement element in children)
            //    if (element is ListView)
            //        if ((element as ListView).Name.Contains("lstvwPark"))
            //            lstvwsCheckpoints.Add(element as ListView);
            //return lstvwsCheckpoints;

            return lstvwsCheckpoints = new List<ListView>()
            {
                airportUserControl.lstvwParkUnload, airportUserControl.lstvwParkDepart
            };
        }
        ICollection<Image> InitializeImgPlanes(ICollection<Image> imgPlanes) //UIElementCollection children, 
        {
            //foreach (UIElement element in children)
            //    if (element is Image)
            //        if ((element as Image).Name.Contains("imgPlane"))
            //            imgPlanes.Add(element as Image);
            //return imgPlanes;

            return imgPlanes = new List<Image>()
            {
                airportUserControl.imgPlaneArr1, airportUserControl.imgPlaneArr2, airportUserControl.imgPlaneArr3,
                airportUserControl.imgPlaneRunway, airportUserControl.imgPlaneTerminal1,
                airportUserControl.imgPlaneTerminal2, airportUserControl.imgPlanDepart
            };
        }

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
