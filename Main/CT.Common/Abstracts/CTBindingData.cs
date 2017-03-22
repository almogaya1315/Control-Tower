using CT.Common.DTO_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CT.Common.Abstracts
{
    public abstract class CTBindingData : ControlInitializer, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CTBindingData()
        {
            InitializeBindings();
        }
        void InitializeBindings()
        {
            Terminal1State = string.Empty;
            Terminal2State = string.Empty;

            FlightInLanding1 = InitializeFlightBindingObject();
            FlightInLanding2 = InitializeFlightBindingObject();
            FlightInLanding3 = InitializeFlightBindingObject();
            FlightInRunway = InitializeFlightBindingObject();
            FlightsInStandbyForUnloading = new ObservableCollection<FlightDTO>() { InitializeFlightBindingObject() };
            FlightInTerminal1 = InitializeFlightBindingObject();
            FlightInTerminal2 = InitializeFlightBindingObject();
            FlightsInStandbyForBoarding = new ObservableCollection<FlightDTO>() { InitializeFlightBindingObject() };
            FlightInDeparted = InitializeFlightBindingObject();
        }
        FlightDTO InitializeFlightBindingObject()
        {
            return new FlightDTO()
            {
                Checkpoint = null,
                CheckpointControl = string.Empty,
                FlightSerial = -1,
                IsAlive = false,
                PlaneImgPath = string.Empty,
                Process = null
            };
        }

        string terminal1State;
        public string Terminal1State
        {
            get
            {
                return terminal1State;
            }
            set
            {
                terminal1State = value;
                RaisePropertyChanged("Terminal1State");
            }
        }

        string terminal2State;
        public string Terminal2State
        {
            get
            {
                return terminal2State;
            }
            set
            {
                terminal2State = value;
                RaisePropertyChanged("Terminal2State");
            }
        }

        FlightDTO flightInLanding1;
        public FlightDTO FlightInLanding1
        {
            get
            {
                return flightInLanding1;
            }
            set
            {
                flightInLanding1 = value;
                RaisePropertyChanged("FlightInLanding1");
            }
        }

        FlightDTO flightInLanding2;
        public FlightDTO FlightInLanding2
        {
            get
            {
                return flightInLanding2;
            }
            set
            {
                flightInLanding2 = value;
                RaisePropertyChanged("FlightInLanding2");
            }
        }

        FlightDTO flightInLanding3;
        public FlightDTO FlightInLanding3
        {
            get
            {
                return flightInLanding3;
            }
            set
            {
                flightInLanding3 = value;
                RaisePropertyChanged("FlightInLanding3");
            }
        }

        FlightDTO flightInRunway;
        public FlightDTO FlightInRunway
        {
            get
            {
                return flightInRunway;
            }
            set
            {
                flightInRunway = value;
                RaisePropertyChanged("FlightInRunway");
            }
        }

        ObservableCollection<FlightDTO> flightsInStandbyForUnloading;
        public ObservableCollection<FlightDTO> FlightsInStandbyForUnloading
        {
            get
            {
                return flightsInStandbyForUnloading;
            }
            set
            {
                flightsInStandbyForUnloading = value;
                RaisePropertyChanged("FlightsInStandbyForUnloading");
            }
        }

        FlightDTO flightInTerminal1;
        public FlightDTO FlightInTerminal1
        {
            get
            {
                return flightInTerminal1;
            }
            set
            {
                flightInTerminal1 = value;
                RaisePropertyChanged("FlightInTerminal1");
            }
        }

        FlightDTO flightInTerminal2;
        public FlightDTO FlightInTerminal2
        {
            get
            {
                return flightInTerminal2;
            }
            set
            {
                flightInTerminal2 = value;
                RaisePropertyChanged("FlightInTerminal2");
            }
        }

        ObservableCollection<FlightDTO> flightsInStandbyForBoarding;
        public ObservableCollection<FlightDTO> FlightsInStandbyForBoarding
        {
            get
            {
                return flightsInStandbyForBoarding;
            }
            set
            {
                flightsInStandbyForBoarding = value;
                RaisePropertyChanged("FlightsInStandbyForBoarding");
            }
        }

        FlightDTO flightInDeparted;
        public FlightDTO FlightInDeparted
        {
            get
            {
                return flightInDeparted;
            }
            set
            {
                flightInDeparted = value;
                RaisePropertyChanged("FlightInDeparted");
            }
        }
    }
}
