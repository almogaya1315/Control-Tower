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
    public class AirportViewModel : CTBindingData
    {
        #region private props & ctor
        SimServiceProxy simProxy;
        AirportUserControl airportUserControl;

        ICollection<TextBlock> txtblckCheckpoints { get; set; }
        ICollection<ListView> lstvwsCheckpoints { get; set; }
        ICollection<Image> imgPlanes { get; set; }

        public AirportViewModel(AirportUserControl control, SimServiceProxy proxy) : base()
        {
            airportUserControl = control;
            airportUserControl.Loaded += airportUserControl_Loaded;

            simProxy = proxy;
            simProxy.OnLoadEvent += SimProxy_OnLoadEvent;
            simProxy.OnPromotionEvaluationEvent += SimProxy_OnPromotionEvaluationEvent;
            simProxy.OnDisposeEvent += SimProxy_OnDisposeEvent;

            AddFlightCommand = new AddFlightCommand(AddFlight);

            txtblckCheckpoints = InitializeTxtblckCheckpoints(txtblckCheckpoints);
            lstvwsCheckpoints = InitializeLstvwsCheckpoints(lstvwsCheckpoints);
            imgPlanes = InitializeImgPlanes(imgPlanes);
        }
        #endregion

        #region ui events
        void airportUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is UserControl)
                simProxy.OnLoad((sender as UserControl).IsLoaded);
            else throw new Exception($"sender object is not a type of 'UserControl'.");
        }
        #endregion

        #region service events
        void SimProxy_OnLoadEvent(object sender, bool isLoaded)
        {
            RequestInitializeSimulator reqInitSim =
                new RequestInitializeSimulator() { IsWindowLoaded = isLoaded };
            ResponseInitializeSimulator resInitSim = simProxy.InitializeSimulator(reqInitSim);
            if (resInitSim.IsSuccess)
            {
                Timer arrivalTimer = new Timer(resInitSim.TimerInterval);
                arrivalTimer.Elapsed += CreateFlight_ArrivalTimerElapsed;
                //arrivalTimer.Start();
                CreateFlight_ArrivalTimerElapsed(null, null);
            }
        }
        void CreateFlight_ArrivalTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //if (sender is Timer) (sender as Timer).Stop();

            ResponseFlightObject resFlight = null;
            try
            {
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

            if (resFlight.IsSuccess)
            {
                double initialDuration = simProxy.GetCheckpointDuration(new RequestCheckpointDuration()
                { CheckpointSerial = "1", CheckpointType = CheckpointType.Landing.ToString() }).CheckpointDuration;
                simProxy.flightsTimers[resFlight.Flight] = new Timer(initialDuration);
                simProxy.flightsTimers[resFlight.Flight].Elapsed += PromotionTimer_Elapsed;

                simProxy.flightsTimers[resFlight.Flight].Start();
                //PromotionTimer_Elapsed(simProxy.flightsTimers.Values.FirstOrDefault(), null);
            }
            else throw new Exception("No success retrieving flight response.");

            //(sender as Timer).Start();
        }
        void PromotionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            simProxy.flightsTimers.Values.FirstOrDefault(t => t == sender as Timer).Stop();

            FlightDTO flight = null;
            //KeyValuePair<FlightDTO, Timer> keyToRemove = new KeyValuePair<FlightDTO, Timer>();
            //KeyValuePair<FlightDTO, Timer> keyToAdd = new KeyValuePair<FlightDTO, Timer>();

            foreach (FlightDTO fdto in simProxy.flightsTimers.Keys)
            {
                if (simProxy.flightsTimers[fdto].GetHashCode() == sender.GetHashCode())
                {
                    simProxy.OnPromotion(fdto);
                    flight = simProxy.GetFlight(fdto.FlightSerial);
                    //keyToRemove = new KeyValuePair<FlightDTO, Timer>(fdto, simProxy.flightsTimers[fdto]);
                    //if (flight != null)
                    //    keyToAdd = new KeyValuePair<FlightDTO, Timer>(flight, simProxy.flightsTimers[fdto]);
                    break;
                }
            }

            //simProxy.UpdateflightsTimersHash(flight, keyToRemove, keyToAdd);

            //if (flight != null)
            //{
            //    simProxy.flightsTimers.Remove(keyToRemove.Key);
            //    simProxy.flightsTimers.Add(keyToAdd.Key, keyToAdd.Value);
            //}
            //else
            //{
            //    simProxy.flightsTimers[keyToRemove.Key].Dispose();
            //    simProxy.flightsTimers.Remove(keyToRemove.Key);
            //    return;
            //}

            if (flight != null)
                simProxy.flightsTimers.FirstOrDefault(pair => pair.Key.FlightSerial == flight.FlightSerial).Value.Start();
            //simProxy.flightsTimers.Values.FirstOrDefault(t => t == sender as Timer).Start();
        }
        void SimProxy_OnPromotionEvaluationEvent(object sender, FlightDTO flight)
        {
            bool isBoarding = default(bool);

            if (FlightInTerminal1.FlightSerial == flight.FlightSerial)
            {
                if (Terminal1State == $"{TerminalState.Unloading}...") isBoarding = false;
                else if (Terminal1State == $"...{TerminalState.Boarding}") isBoarding = true;
            }
            else if (FlightInTerminal2.FlightSerial == flight.FlightSerial)
            {
                if (Terminal2State == $"{TerminalState.Unloading}...") isBoarding = false;
                else if (Terminal2State == $"...{TerminalState.Boarding}") isBoarding = true;
            }

            RequestFlightPosition reqPosition = new RequestFlightPosition()
            {
                TxtblckNameFlightNumberHash = SetTxtblckHash(txtblckCheckpoints),
                LstvwNameFlightsListHash = SetLstvwHash(lstvwsCheckpoints),
                FlightSerial = flight.FlightSerial.ToString(),
                IsBoarding = isBoarding
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
