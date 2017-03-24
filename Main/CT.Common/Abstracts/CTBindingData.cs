using CT.Common.DTO_Models;
using CT.Common.Enums;
using CT.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CT.Common.Abstracts
{
    public abstract class CTBindingData : INotifyPropertyChanged
    {
        #region interface data
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region initialzer
        public CTBindingData()
        {
            InitializeBindings();
        }
        void InitializeBindings()
        {
            Terminal1State = TerminalState.Idil.ToString();
            Terminal2State = TerminalState.Idil.ToString();

            FlightInLanding1 = InitializeFlightBindingObject();
            FlightInLanding2 = InitializeFlightBindingObject();
            FlightInLanding3 = InitializeFlightBindingObject();
            FlightInRunway = InitializeFlightBindingObject();
            FlightsInStandbyForUnloading = new ObservableCollection<string>() { InitializeFlightBindingObject().FlightSerial.ToString() };
            FlightInTerminal1 = InitializeFlightBindingObject();
            FlightInTerminal2 = InitializeFlightBindingObject();
            FlightsInStandbyForBoarding = new ObservableCollection<string>() { InitializeFlightBindingObject().FlightSerial.ToString() };
            FlightInDeparted = InitializeFlightBindingObject();
        }
        #endregion

        #region protected methods
        protected FlightDTO InitializeFlightBindingObject()
        {
            return new FlightDTO()
            {
                Checkpoint = null,
                CheckpointControl = string.Empty,
                FlightSerial = -1,
                IsAlive = false,
                PlaneImgPath = PlaneImageSource.NoPlane.ToString(),
                Process = null
            };
        }
        protected FlightDTO InitializeFlightBindingObject(FlightDTO flight)
        {
            return new FlightDTO()
            {
                Checkpoint = flight.Checkpoint,
                CheckpointControl = flight.CheckpointControl,
                FlightSerial = flight.FlightSerial,
                IsAlive = flight.IsAlive,
                PlaneImgPath = flight.PlaneImgPath,
                Process = flight.Process
            };
        }
        #endregion

        #region switchers
        public bool? SwitchOnNextCheckpointName(Dispatcher dispatcher, string nextCheckpointName, FlightDTO flight)
        {
            bool? isFound = default(bool);
            switch (nextCheckpointName)
            {
                case "lstvwParkUnload":
                    flight.PlaneImgPath = PlaneImageSource.NoPlane.ToString();
                    dispatcher.Invoke(() =>
                    {
                        FlightsInStandbyForUnloading.Add(flight.FlightSerial.ToString());
                    });
                    FlightInRunway = InitializeFlightBindingObject();
                    //lstvwParkUnload.Items.Add(flight.FlightSerial.ToString());
                    //txtblckFlightRunway.Text = "---";
                    //imgPlaneRunway.Source = PlaneImageSource.NoPlane;
                    return isFound = true;
                case "txtblckFlightDepart":
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    FlightInDeparted = InitializeFlightBindingObject(flight);
                    FlightInRunway = InitializeFlightBindingObject();
                    //txtblckFlightDepart.Text = flight.FlightSerial.ToString();
                    //imgPlanDepart.Source = PlaneImageSource.PlaneLeft;
                    //txtblckFlightRunway.Text = "---";
                    //imgPlaneRunway.Source = PlaneImageSource.NoPlane;
                    return isFound = true;
                case "Stay in checkpoint!":
                    return isFound = null;
                case "Departed!":
                    //SimProxy.OnDispose(flight.FlightSerial);
                    return isFound = null;
                case "No access to field!":
                    //SimProxy.OnDispose(flight.FlightSerial);
                    return isFound = null;
            }
            return isFound = false;
        }
        public void SwitchOnCheckpointSerial(Dispatcher dispatcher, int checkpointSerial, string checkpointType,
            string nextCheckpointName, string lastCheckpointPosition, FlightDTO flight)
        {
            switch (checkpointSerial)
            {
                case 1:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    FlightInLanding1 = InitializeFlightBindingObject(flight);
                    //imgPlaneArr1.Source = PlaneImageSource.PlaneLeft;
                    //txtblckFlightArr1.Text = flight.FlightSerial.ToString();
                    break;
                case 2:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    FlightInLanding1 = InitializeFlightBindingObject();
                    FlightInLanding2 = InitializeFlightBindingObject(flight);
                    //imgPlaneArr1.Source = PlaneImageSource.NoPlane;
                    //txtblckFlightArr1.Text = "---";
                    //imgPlaneArr2.Source = PlaneImageSource.PlaneLeft;
                    //txtblckFlightArr2.Text = flight.FlightSerial.ToString();
                    break;
                case 3:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    FlightInLanding2 = InitializeFlightBindingObject();
                    FlightInLanding3 = InitializeFlightBindingObject(flight);
                    //imgPlaneArr2.Source = PlaneImageSource.NoPlane;
                    //txtblckFlightArr2.Text = "---";
                    //imgPlaneArr3.Source = PlaneImageSource.PlaneLeft;
                    //txtblckFlightArr3.Text = flight.FlightSerial.ToString();
                    break;
                case 41:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    FlightInLanding3 = InitializeFlightBindingObject();
                    FlightInRunway = InitializeFlightBindingObject(flight);
                    break;
                case 42:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    dispatcher.Invoke(() =>
                    {
                        FlightsInStandbyForBoarding.Remove(flight.FlightSerial.ToString());
                    });
                    FlightInRunway = InitializeFlightBindingObject(flight);
                    break;
                //case 4:
                //    if (checkpointType == "RunwayLanded")
                //    {
                //        imgPlaneArr3.Source = PlaneImageSource.NoPlane;
                //        txtblckFlightArr3.Text = "---";
                //        imgPlaneRunway.Source = PlaneImageSource.PlaneLeft;
                //        txtblckFlightRunway.Text = flight.FlightSerial.ToString();
                //    }
                //    else if (checkpointType == "RunwayDeparting")
                //    {
                //        lstvwParkDepart.Items.Remove(flight.FlightSerial.ToString());
                //        imgPlaneRunway.Source = PlaneImageSource.PlaneLeft;
                //        txtblckFlightRunway.Text = flight.FlightSerial.ToString();
                //    }
                //    break;
                case 61:
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    dispatcher.Invoke(() =>
                    {
                        FlightsInStandbyForUnloading.Remove(flight.FlightSerial.ToString());
                    });
                    FlightInTerminal1 = InitializeFlightBindingObject(flight);
                    break;
                case 62:
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    dispatcher.Invoke(() =>
                    {
                        FlightsInStandbyForUnloading.Remove(flight.FlightSerial.ToString());
                    });
                    FlightInTerminal2 = InitializeFlightBindingObject(flight);
                    break;
                //case 6:
                //    if (nextCheckpointName == "FlightTerminal1")
                //    {
                //        lstvwParkUnload.Items.Remove(flight.FlightSerial.ToString());
                //        txtblckFlightTerminal1.Text = flight.FlightSerial.ToString();
                //        imgPlaneTerminal1.Source = PlaneImageSource.PlaneDown;
                //        txtblckTerminal1Message.Text = "Unloading...";
                //    }
                //    if (nextCheckpointName == "FlightTerminal2")
                //    {
                //        lstvwParkUnload.Items.Remove(flight.FlightSerial.ToString());
                //        txtblckFlightTerminal2.Text = flight.FlightSerial.ToString();
                //        imgPlaneTerminal2.Source = PlaneImageSource.PlaneDown;
                //        txtblckTerminal2Message.Text = "Unloading...";
                //    }
                //    break;
                case 71:
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    FlightInTerminal1 = InitializeFlightBindingObject(flight);
                    Terminal1State = $"...{TerminalState.Boarding}";
                    break;
                case 72:
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    FlightInTerminal2 = InitializeFlightBindingObject(flight);
                    Terminal2State = $"...{TerminalState.Boarding}";
                    break;
                //case 7:
                //    if (nextCheckpointName == "txtblckFlightTerminal1")
                //        txtblckTerminal1Message.Text = "...Boarding";
                //    if (nextCheckpointName == "txtblckFlightTerminal2")
                //        txtblckTerminal2Message.Text = "...Boarding";
                //    break;
                case 8:
                    if (lastCheckpointPosition == "txtblckFlightTerminal1")
                    {
                        FlightInTerminal1 = InitializeFlightBindingObject();
                        Terminal1State = TerminalState.Idil.ToString();
                        flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                        dispatcher.Invoke(() =>
                        {
                            FlightsInStandbyForBoarding.Add(flight.FlightSerial.ToString());
                        });
                        //txtblckFlightTerminal1.Text = "---";
                        //imgPlaneTerminal1.Source = PlaneImageSource.NoPlane;
                        //txtblckTerminal1Message.Text = string.Empty;
                        //lstvwParkDepart.Items.Add(flight.FlightSerial.ToString());
                    }
                    if (lastCheckpointPosition == "txtblckFlightTerminal2")
                    {
                        FlightInTerminal2 = InitializeFlightBindingObject();
                        Terminal2State = TerminalState.Idil.ToString();
                        flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                        dispatcher.Invoke(() =>
                        {
                            FlightsInStandbyForBoarding.Add(flight.FlightSerial.ToString());
                        });
                        //txtblckFlightTerminal2.Text = "---";
                        //imgPlaneTerminal2.Source = PlaneImageSource.NoPlane;
                        //txtblckTerminal2Message.Text = string.Empty;
                        //lstvwParkDepart.Items.Add(flight.FlightSerial.ToString());
                    }
                    break;
            }
        }
        #endregion

        #region binding props
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

        ObservableCollection<string> flightsInStandbyForUnloading;
        public ObservableCollection<string> FlightsInStandbyForUnloading
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

        ObservableCollection<string> flightsInStandbyForBoarding;
        public ObservableCollection<string> FlightsInStandbyForBoarding
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
        #endregion
    }
}
