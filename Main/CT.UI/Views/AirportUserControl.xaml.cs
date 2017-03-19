using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CT.UI.Proxy;
using System.Timers;
using System.ComponentModel;
using CT.UI.SimulatorServiceReference;
using CT.Common.Utilities;
using System.Threading;
using CT.Common.DTO_Models;

namespace CT.UI.Views
{
    public partial class AirportUserControl : UserControl
    {
        MainWindow coreWindow { get; set; }
        SimServiceProxy SimProxy { get; set; }
        ICollection<TextBlock> txtblckCheckpoints { get; set; }
        ICollection<ListView> lstvwsCheckpoints { get; set; }
        ICollection<Image> imgPlanes { get; set; }

        public AirportUserControl(MainWindow core)
        {
            try
            {
                InitializeComponent();
                coreWindow = core;
            }
            catch (Exception e)
            {
                throw new Exception($"Airport UC did not initialize. {e.Message}");
            }
            finally
            {
                SimProxy = new SimServiceProxy();
                SimProxy.OnLoadEvent += SimProxy_OnLoadEvent;
                SimProxy.OnPromotionEvaluationEvent += SimProxy_OnPromotionEvaluationEvent;
                SimProxy.OnDisposeEvent += SimProxy_OnDisposeEvent;

                txtblckCheckpoints = InitializeTxtblckCheckpoints();
                lstvwsCheckpoints = InitializeLstvwsCheckpoints();
                imgPlanes = InitializeImgPlanes();
            }
        }



        #region service events
        void SimProxy_OnLoadEvent(object sender, bool isLoaded)
        {
            RequestInitializeSimulator reqInitSim =
                new RequestInitializeSimulator() { IsWindowLoaded = isLoaded };
            ResponseInitializeSimulator resInitSim = SimProxy.InitializeSimulator(reqInitSim);
            if (resInitSim.IsSuccess)
            {
                System.Timers.Timer arrivalTimer = new System.Timers.Timer(resInitSim.TimerInterval);
                arrivalTimer.Elapsed += CreateFlight_ArrivalTimerElapsed;
                arrivalTimer.Start();
                //CreateFlight_ArrivalTimerElapsed(this, null);
            }
        }
        void CreateFlight_ArrivalTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (sender is System.Timers.Timer) (sender as System.Timers.Timer).Stop();

            ResponseFlightObject resFlight = null;
            try
            {
                RequestFlightObject reqFlight = new RequestFlightObject()
                {
                    CurrentFlights = SimProxy.GetFlightsCollection().Flights
                };
                resFlight = SimProxy.CreateFlightObject(reqFlight);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not retrieve collection OR create flight object. {ex.Message}");
            }

            if (resFlight.IsSuccess)
            {
                double initialDuration = 2000;
                SimProxy.flightsTimers[resFlight.Flight] = new System.Timers.Timer(initialDuration);
                SimProxy.flightsTimers[resFlight.Flight].Elapsed += PromotionTimer_Elapsed;

                SimProxy.flightsTimers[resFlight.Flight].Start();
            }
            else throw new Exception("No success retrieving flight response.");

            (sender as System.Timers.Timer).Start();
        }

        public void PromotionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SimProxy.flightsTimers.Values.FirstOrDefault(t => t == sender as System.Timers.Timer).Stop();

            FlightDTO flight = null;
            KeyValuePair<FlightDTO, System.Timers.Timer> keyToRemove = new KeyValuePair<FlightDTO, System.Timers.Timer>();
            KeyValuePair<FlightDTO, System.Timers.Timer> keyToAdd = new KeyValuePair<FlightDTO, System.Timers.Timer>();

            foreach (FlightDTO fdto in SimProxy.flightsTimers.Keys)
            {
                if (SimProxy.flightsTimers[fdto].GetHashCode() == sender.GetHashCode())
                {
                    SimProxy.OnPromotion(fdto);
                    flight = SimProxy.GetFlight(fdto.FlightSerial);
                    keyToRemove = new KeyValuePair<FlightDTO, System.Timers.Timer>(fdto, SimProxy.flightsTimers[fdto]);
                    if (flight != null)
                        keyToAdd = new KeyValuePair<FlightDTO, System.Timers.Timer>(flight, SimProxy.flightsTimers[fdto]);
                    break;
                }
            }


            if (flight != null)
            {
                SimProxy.flightsTimers.Remove(keyToRemove.Key);
                SimProxy.flightsTimers.Add(keyToAdd.Key, keyToAdd.Value);
            }
            else
            {
                SimProxy.flightsTimers[keyToRemove.Key].Dispose();
                SimProxy.flightsTimers.Remove(keyToRemove.Key);
                return;
            }

            SimProxy.flightsTimers.Values.FirstOrDefault(t => t == sender as System.Timers.Timer).Start();
        }
        void SimProxy_OnPromotionEvaluationEvent(object sender, FlightDTO flight)
        {
            Dispatcher.Invoke(() =>
            {
                bool isBoarding = default(bool);
                if (txtblckFlightTerminal1.Text == flight.FlightSerial.ToString())
                {
                    if (txtblckTerminal1Message.Text == "Unloading...") isBoarding = false;
                    else if (txtblckTerminal1Message.Text == "...Boarding") isBoarding = true;
                }
                else if (txtblckFlightTerminal2.Text == flight.FlightSerial.ToString())
                {
                    if (txtblckTerminal2Message.Text == "Unloading...") isBoarding = false;
                    else if (txtblckTerminal2Message.Text == "...Boarding") isBoarding = true;
                }
                RequestFlightPosition reqPosition = new RequestFlightPosition()
                {
                    TxtblckNameFlightNumberHash = SetTxtblckHash(),
                    LstvwNameFlightsListHash = SetLstvwHash(),
                    FlightSerial = flight.FlightSerial.ToString(),
                    IsBoarding = isBoarding
                };
                ResponseFlightPosition resPosition = SimProxy.GetFlightPosition(reqPosition);
                if (resPosition.IsSuccess)
                {
                    if (flight.Checkpoint != null && resPosition.NextCheckpointName != "Departed!")
                    {
                        //FlightDTO entityMoq = SimProxy.GetFlight(flight.FlightSerial);
                        double duration = flight.Checkpoint.Duration; //entityMoq.Checkpoint.Duration;
                        SimProxy.flightsTimers[flight].Interval = duration;
                    }
                    else if (resPosition.CheckpointSerial != -1 && resPosition.CheckpointType != null)
                    {
                        RequestCheckpointDuration reqDur = new RequestCheckpointDuration()
                        { CheckpointSerial = resPosition.CheckpointSerial.ToString() };
                        ResponseCheckpointDuration resDur = SimProxy.GetCheckpointDuration(reqDur);
                        if (resDur.IsSuccess)
                        { SimProxy.flightsTimers[flight].Interval = resDur.CheckpointDuration; }
                    }

                    if (resPosition.LastCheckpointPosition == "txtblckFlightTerminal1" || resPosition.LastCheckpointPosition == "txtblckFlightTerminal2")
                    {
                        SwitchOnCheckpointSerial(resPosition.CheckpointSerial, resPosition.CheckpointType,
                            resPosition.NextCheckpointName, resPosition.LastCheckpointPosition, flight);
                        return;
                    }
                    bool? isFound = SwitchOnNextCheckpointName(resPosition.NextCheckpointName, flight);
                    if (isFound == null) return;
                    if (isFound == false)
                    {
                        SwitchOnCheckpointSerial(resPosition.CheckpointSerial, resPosition.CheckpointType,
                            resPosition.NextCheckpointName, resPosition.LastCheckpointPosition, flight);
                    }
                }
            });
        }

        void SimProxy_OnDisposeEvent(object sender, int flightSerial)
        {
            RequestDisposeFlight reqDis = new RequestDisposeFlight() { FlightSerial = flightSerial };
            ResponseDisposeFlight resDis = SimProxy.DisposeFlight(reqDis);
            if (resDis.IsSuccess)
            {
                txtblckFlightDepart.Text = "---";
                imgPlanDepart.Source = PlaneImageSource.NoPlane;
                return;
            }
            else throw new Exception("[UI] Service was unable to dispose the flight.");
        }
        #endregion

        #region ui events
        void AirportUC_Loaded(object sender, RoutedEventArgs e)
        {
            SimProxy.OnLoad(IsLoaded);
        }
        #endregion

        #region private methods
        ICollection<TextBlock> InitializeTxtblckCheckpoints()
        {
            return txtblckCheckpoints = new List<TextBlock>()
            {
                txtblckFlightArr1, txtblckFlightArr2, txtblckFlightArr3,
                txtblckFlightRunway, txtblckFlightTerminal1,
                txtblckFlightTerminal2, txtblckFlightDepart
            };
        }
        ICollection<ListView> InitializeLstvwsCheckpoints()
        {
            return lstvwsCheckpoints = new List<ListView>()
            {
                lstvwParkUnload, lstvwParkDepart
            };
        }
        ICollection<Image> InitializeImgPlanes()
        {
            return imgPlanes = new List<Image>()
            {
                imgPlaneArr1, imgPlaneArr2, imgPlaneArr3,
                imgPlaneRunway, imgPlaneTerminal1,
                imgPlaneTerminal2, imgPlanDepart
            };
        }

        bool? SwitchOnNextCheckpointName(string nextCheckpointName, FlightDTO flight)
        {
            bool? isFound = default(bool);
            switch (nextCheckpointName)
            {
                case "lstvwParkUnload":
                    lstvwParkUnload.Items.Add(flight.FlightSerial.ToString());
                    txtblckFlightRunway.Text = "---";
                    imgPlaneRunway.Source = PlaneImageSource.NoPlane;
                    return isFound = true;
                case "txtblckFlightDepart":
                    txtblckFlightDepart.Text = flight.FlightSerial.ToString();
                    imgPlanDepart.Source = PlaneImageSource.PlaneLeft;
                    txtblckFlightRunway.Text = "---";
                    imgPlaneRunway.Source = PlaneImageSource.NoPlane;
                    return isFound = true;
                case "Stay in checkpoint!":
                    return isFound = null;
                case "Departed!":
                    SimProxy.OnDispose(flight.FlightSerial);
                    return isFound = null;
                case "No access to field!":
                    SimProxy.OnDispose(flight.FlightSerial);
                    return isFound = null;
            }
            return isFound = false;
        }
        void SwitchOnCheckpointSerial(int checkpointSerial, string checkpointType,
            string nextCheckpointName, string lastCheckpointPosition, FlightDTO flight)
        {
            switch (checkpointSerial)
            {
                case 1:
                    imgPlaneArr1.Source = PlaneImageSource.PlaneLeft;
                    txtblckFlightArr1.Text = flight.FlightSerial.ToString();
                    break;
                case 2:
                    imgPlaneArr1.Source = PlaneImageSource.NoPlane;
                    txtblckFlightArr1.Text = "---";
                    imgPlaneArr2.Source = PlaneImageSource.PlaneLeft;
                    txtblckFlightArr2.Text = flight.FlightSerial.ToString();
                    break;
                case 3:
                    imgPlaneArr2.Source = PlaneImageSource.NoPlane;
                    txtblckFlightArr2.Text = "---";
                    imgPlaneArr3.Source = PlaneImageSource.PlaneLeft;
                    txtblckFlightArr3.Text = flight.FlightSerial.ToString();
                    break;
                case 4:
                    if (checkpointType == "RunwayLanded")
                    {
                        imgPlaneArr3.Source = PlaneImageSource.NoPlane;
                        txtblckFlightArr3.Text = "---";
                        imgPlaneRunway.Source = PlaneImageSource.PlaneLeft;
                        txtblckFlightRunway.Text = flight.FlightSerial.ToString();
                    }
                    else if (checkpointType == "RunwayDeparting")
                    {
                        lstvwParkDepart.Items.Remove(flight.FlightSerial.ToString());
                        imgPlaneRunway.Source = PlaneImageSource.PlaneLeft;
                        txtblckFlightRunway.Text = flight.FlightSerial.ToString();
                    }
                    break;
                case 6:
                    if (nextCheckpointName == "FlightTerminal1")
                    {
                        lstvwParkUnload.Items.Remove(flight.FlightSerial.ToString());
                        txtblckFlightTerminal1.Text = flight.FlightSerial.ToString();
                        imgPlaneTerminal1.Source = PlaneImageSource.PlaneDown;
                        txtblckTerminal1Message.Text = "Unloading...";
                    }
                    if (nextCheckpointName == "FlightTerminal2")
                    {
                        lstvwParkUnload.Items.Remove(flight.FlightSerial.ToString());
                        txtblckFlightTerminal2.Text = flight.FlightSerial.ToString();
                        imgPlaneTerminal2.Source = PlaneImageSource.PlaneDown;
                        txtblckTerminal2Message.Text = "Unloading...";
                    }
                    break;
                case 7:
                    if (nextCheckpointName == "txtblckFlightTerminal1")
                        txtblckTerminal1Message.Text = "...Boarding";
                    if (nextCheckpointName == "txtblckFlightTerminal2")
                        txtblckTerminal2Message.Text = "...Boarding";
                    break;
                case 8:
                    if (lastCheckpointPosition == "txtblckFlightTerminal1")
                    {
                        txtblckFlightTerminal1.Text = "---";
                        imgPlaneTerminal1.Source = PlaneImageSource.NoPlane;
                        txtblckTerminal1Message.Text = string.Empty;
                        lstvwParkDepart.Items.Add(flight.FlightSerial.ToString());
                    }
                    if (lastCheckpointPosition == "txtblckFlightTerminal2")
                    {
                        txtblckFlightTerminal2.Text = "---";
                        imgPlaneTerminal2.Source = PlaneImageSource.NoPlane;
                        txtblckTerminal2Message.Text = string.Empty;
                        lstvwParkDepart.Items.Add(flight.FlightSerial.ToString());
                    }
                    break;
            }
        }

        Dictionary<string, string> SetTxtblckHash()
        {
            Dictionary<string, string> txtblckNameFlightNumberHash = new Dictionary<string, string>();
            foreach (TextBlock txtblck in txtblckCheckpoints)
                txtblckNameFlightNumberHash[txtblck.Name] = txtblck.Text;
            return txtblckNameFlightNumberHash;
        }
        Dictionary<string, string[]> SetLstvwHash()
        {
            Dictionary<string, string[]> lstvwNameFlightsListHash = new Dictionary<string, string[]>();
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
            return lstvwNameFlightsListHash;
        }
        #endregion

        void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Timers.Timer timer = SimProxy.flightsTimers.Values.FirstOrDefault();
            PromotionTimer_Elapsed(timer, null);
        }
    }
}
