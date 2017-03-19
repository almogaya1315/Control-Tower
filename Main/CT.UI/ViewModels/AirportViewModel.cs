using CT.Common.Abstracts;
using CT.Common.Commands;
using CT.Common.DTO_Models;
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

namespace CT.UI.ViewModels
{
    public class AirportViewModel : ControlInitializer, INotifyPropertyChanged
    {
        #region private props & ctor
        SimServiceProxy simProxy;
        AirportUserControl airportUserControl;

        ICollection<TextBlock> txtblckCheckpoints { get; set; }
        ICollection<ListView> lstvwsCheckpoints { get; set; }
        ICollection<Image> imgPlanes { get; set; }

        

        public AirportViewModel(AirportUserControl control, SimServiceProxy proxy)
        {
            airportUserControl = control;
            airportUserControl.Loaded += airportUserControl_Loaded;
            simProxy = proxy;

            //AddFlightCommand = new AddFlightCommand(AddFlight);

            simProxy.OnLoadEvent += SimProxy_OnLoadEvent;
            simProxy.OnPromotionEvaluationEvent += SimProxy_OnPromotionEvaluationEvent;
            simProxy.OnDisposeEvent += SimProxy_OnDisposeEvent;

            txtblckCheckpoints = InitializeTxtblckCheckpoints(airportUserControl.grdMain.Children, txtblckCheckpoints);
            lstvwsCheckpoints = InitializeLstvwsCheckpoints(airportUserControl.grdMain.Children, lstvwsCheckpoints);
            imgPlanes = InitializeImgPlanes(airportUserControl.grdMain.Children, imgPlanes);
        }
        #endregion

        #region mvvm data
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddFlightCommand { get; set; }

        ObservableCollection<FlightDTO> flights;
        public ObservableCollection<FlightDTO> Flights
        {
            get
            {
                return flights;
            }
            set
            {
                flights = value;
                RaisePropertyChanged("Flights");
            }
        }

        FlightDTO currentFlight;
        public FlightDTO CurrentFlight
        {
            get
            {
                return currentFlight;
            }
            set
            {
                currentFlight = value;
                RaisePropertyChanged("CurrentFlight");
            }
        }

        BitmapImage planeImage;
        public BitmapImage PlaneImage
        {
            get
            {
                return planeImage;
            }
            set
            {
                planeImage = value;
                RaisePropertyChanged("PlaneImage");
            }
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
                //currentFlight = resFlight.Flight;
                //flights = new ObservableCollection<FlightDTO>(simProxy.GetFlightsCollection().Flights);
                //flights = simProxy.GetFlightsCollection().Flights.ToList().ToObservableCollection();

                double initialDuration = 2000;
                simProxy.flightsTimers[resFlight.Flight] = new System.Timers.Timer(initialDuration);
                simProxy.flightsTimers[resFlight.Flight].Elapsed += PromotionTimer_Elapsed;

                simProxy.flightsTimers[resFlight.Flight].Start();
            }
            else throw new Exception("No success retrieving flight response.");

            (sender as System.Timers.Timer).Start();
        }
        #endregion

        //#region public commands
        //public void AddFlight()
        //{
            
        //}
        //#endregion
    }
}
