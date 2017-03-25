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
    /// <summary>
    /// An abstract class that holds all bindings data for the CT app view model
    /// </summary>
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
        /// <summary>
        /// Creates a flight model with defualt values
        /// </summary>
        /// <returns>the initialized flight model</returns>
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

        /// <summary>
        /// Creates a flight model by a specific model
        /// </summary>
        /// <param name="flight">the source model</param>
        /// <returns>the initialized model</returns>
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

        /// <summary>
        /// Determines the terminal checkpoints's state
        /// </summary>
        /// <param name="flight">the current flight in check</param>
        /// <returns>the terminal state represented in bool value</returns>
        protected bool EvaluateTerminalState(FlightDTO flight)
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
            return isBoarding;
        }
        #endregion

        #region switchers
        /// <summary>
        /// The method that updates the binding objects of the UI by the checkpoint's control name retrieved from the timing system
        /// </summary>
        /// <param name="dispatcher">the UI's user control's thread dispatcher</param>
        /// <param name="nextCheckpointName">the next checkpoint's control name</param>
        /// <param name="flight">the current flight passing between checkpoints</param>
        /// <returns></returns>
        public bool? SwitchOnNextCheckpointName(Dispatcher dispatcher, string nextCheckpointName, FlightDTO flight)
        {
            bool? isFound = default(bool);
            switch (nextCheckpointName)
            {
                //if the next checkpoint is the standbyForUbloading
                case "lstvwParkUnload":
                    //no need for plane image
                    flight.PlaneImgPath = PlaneImageSource.NoPlane.ToString();
                    dispatcher.Invoke(() =>
                    {
                        //add the current flight to the standby list
                        FlightsInStandbyForUnloading.Add(flight.FlightSerial.ToString());
                    });
                    //resets the last checkpoint's flight binding to default
                    FlightInRunway = InitializeFlightBindingObject();
                    return isFound = true;
                //if the next checkpoint is the last checkpoint
                case "txtblckFlightDepart":
                    //set the plane image accordingly
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    //resets the current checkpoint's flight binding by the cuurent flight
                    FlightInDeparted = InitializeFlightBindingObject(flight);
                    //resets the last checkpoint's flight binding to default
                    FlightInRunway = InitializeFlightBindingObject();
                    return isFound = true;
                //if the following actions are specified, the method returns null bool, handled by view model 
                case "Stay in checkpoint!":
                    return isFound = null;
                case "Departed!":
                    return isFound = null;
                case "No access to field!":
                    return isFound = null;
            }
            //if the next checkpoint is non of the above, the binding updates are handled in next switch
            return isFound = false;
        }

        /// <summary>
        /// The method that updates the binding objects of the UI by the checkpoint's serial retrieved from the timing system
        /// </summary>
        /// <param name="dispatcher">the UI's user control's thread dispatcher</param>
        /// <param name="checkpointSerial">the next checkpoint's serial</param>
        /// <param name="checkpointType">the next checkpoint's type</param>
        /// <param name="nextCheckpointName">the next checkpoint's control name</param>
        /// <param name="lastCheckpointPosition">the previous checkpoint's control name</param>
        /// <param name="flight">the current flight passing between checkpoints</param>
        public void SwitchOnCheckpointSerial(Dispatcher dispatcher, int checkpointSerial, string checkpointType,
            string nextCheckpointName, string lastCheckpointPosition, FlightDTO flight)
        {
            switch (checkpointSerial)
            {
                //for the first checkpoint
                case 1:
                    //set the plane image accordingly
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    //resets the current checkpoint's flight binding by the cuurent flight
                    FlightInLanding1 = InitializeFlightBindingObject(flight);
                    break;
                //for the second checkpoint
                case 2:
                    //set the plane image accordingly
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    //resets the last checkpoint's flight binding to default
                    FlightInLanding1 = InitializeFlightBindingObject();
                    //resets the current checkpoint's flight binding by the cuurent flight
                    FlightInLanding2 = InitializeFlightBindingObject(flight);
                    break;
                //for the third checkpoint, same as case above
                case 3:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    FlightInLanding2 = InitializeFlightBindingObject();
                    FlightInLanding3 = InitializeFlightBindingObject(flight);
                    break;
                //for the fourth checkpoint, same as case above
                case 41:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    FlightInLanding3 = InitializeFlightBindingObject();
                    FlightInRunway = InitializeFlightBindingObject(flight);
                    break;
                //for the nineth checkpoint
                case 42:
                    flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                    dispatcher.Invoke(() =>
                    {
                        //removes the current binding flight object from the boarding standby list
                        FlightsInStandbyForBoarding.Remove(flight.FlightSerial.ToString());
                    });
                    FlightInRunway = InitializeFlightBindingObject(flight);
                    break;
                //for the sixth checkpoint (fifth checkpoint handled in previous switch)
                case 61:
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    dispatcher.Invoke(() =>
                    {
                        //removes the current binding flight object from the unloading standby list
                        FlightsInStandbyForUnloading.Remove(flight.FlightSerial.ToString());
                    });
                    //sets the UI's terminal state accordingly
                    Terminal1State = $"{TerminalState.Unloading}...";
                    FlightInTerminal1 = InitializeFlightBindingObject(flight);
                    break;
                //same as case above
                case 62:
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    dispatcher.Invoke(() =>
                    {
                        FlightsInStandbyForUnloading.Remove(flight.FlightSerial.ToString());
                    });
                    Terminal2State = $"{TerminalState.Unloading}...";
                    FlightInTerminal2 = InitializeFlightBindingObject(flight);
                    break;
                //for the seventh checkpoint
                case 71:
                    //set the plane image accordingly
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    //resets the current checkpoint's flight binding by the cuurent flight
                    FlightInTerminal1 = InitializeFlightBindingObject(flight);
                    //sets the UI's terminal state accordingly
                    Terminal1State = $"...{TerminalState.Boarding}";
                    break;
                //same as case above
                case 72:
                    flight.PlaneImgPath = PlaneImageSource.PlaneDown.ToString();
                    FlightInTerminal2 = InitializeFlightBindingObject(flight);
                    Terminal2State = $"...{TerminalState.Boarding}";
                    break;
                //for eighth checkpoint
                case 8:
                    //tedermines the terminal the flight left
                    if (lastCheckpointPosition == "txtblckFlightTerminal1")
                    {
                        //resets the last checkpoint's flight binding to default
                        FlightInTerminal1 = InitializeFlightBindingObject();
                        //sets the UI's terminal state accordingly
                        Terminal1State = TerminalState.Idil.ToString();
                        //set the plane image accordingly
                        flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                        dispatcher.Invoke(() =>
                        {
                            //adds the current binding flight object to the boarding standby list
                            FlightsInStandbyForBoarding.Add(flight.FlightSerial.ToString());
                        });
                    }
                    //same as above 'if' performed on terminal 2
                    if (lastCheckpointPosition == "txtblckFlightTerminal2")
                    {
                        FlightInTerminal2 = InitializeFlightBindingObject();
                        Terminal2State = TerminalState.Idil.ToString();
                        flight.PlaneImgPath = PlaneImageSource.PlaneLeft.ToString();
                        dispatcher.Invoke(() =>
                        {
                            FlightsInStandbyForBoarding.Add(flight.FlightSerial.ToString());
                        });
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
