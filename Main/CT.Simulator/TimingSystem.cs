using CT.Common.DTO_Models;
using CT.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CT.Simulator
{
    /// <summary>
    /// The system that determines the current flight's next position
    /// </summary>
    public class TimingSystem
    {
        /// <summary>
        /// Public method exposed to the service that retrieves the flight's next position
        /// </summary>
        /// <param name="txtblckNameFlightNumber">the viewmodel's TextBlock name & current flight serial hash</param>
        /// <param name="lstvwNameFlightList">the viewmodel's ListView name & current flight serial list hash</param>
        /// <param name="flight">the current flight</param>
        /// <param name="isBoarding">indicates if the flight is in landing or departure process</param>
        /// <param name="newCheckpointSerial">[out] the new checkpoint serial</param>
        /// <returns>next checkpoint TextBlock name or specified action & [out] next checkpoint serial</returns>
        public string GetFlightPosition(Dictionary<string, string> txtblckNameFlightNumber, Dictionary<string, List<string>> lstvwNameFlightList, FlightDTO flight, bool isBoarding, out int newCheckpointSerial)
        {
            //when the flight's checkpoint is null, it means the flight is not alive (with serial parameter only)
            if (flight.Checkpoint == null)
            {
                //set first checkpoint serial as defualt
                newCheckpointSerial = 1;
                //retrieves it's TextBlock name
                return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightArr1", flight.IsAlive, isBoarding);
            }
            switch (flight.Checkpoint.Serial)
            {
                //case 1 & 2 => adds 1 to the current checkpoint serial & retrieves it's TextBlock name
                case 1:
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightArr2", flight.IsAlive, isBoarding);
                case 2:
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightArr3", flight.IsAlive, isBoarding);
                //sets the new checkpoint serial to FlightRunwayLanded checkpoint type's serial
                case 3:
                    newCheckpointSerial = 41;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightRunway", flight.IsAlive, isBoarding);
                //sets the new checkpoint serial to StandbyForUnloading checkpoint type's serial
                case 41:
                    newCheckpointSerial = 5;
                    return "lstvwParkUnload";
                //sets the new checkpoint serial to Depart checkpoint type's serial
                case 42:
                    newCheckpointSerial = 9;
                    return "txtblckFlightDepart";
                //checks which terminal is idil and returns its serial
                case 5:
                    if (txtblckNameFlightNumber["txtblckFlightTerminal1"] == "0")
                    {
                        newCheckpointSerial = 61;
                        return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightTerminal1", flight.IsAlive, isBoarding);
                    }
                    else if (txtblckNameFlightNumber["txtblckFlightTerminal2"] == "0")
                    {
                        newCheckpointSerial = 62;
                        return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightTerminal2", flight.IsAlive, isBoarding);
                    }
                    else
                    {
                        //if non returns 'stay in checkpoint' action
                        newCheckpointSerial = 5;
                        return "Stay in checkpoint!";
                    }
                //case 61 & 62 => changes the terminal checkpoint from unloading to boarding serial according to the current terminal
                case 61:
                    newCheckpointSerial = 71;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 62:
                    newCheckpointSerial = 72;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                //case 71 & 72 => changes the next checkpoint serial to StandbyForDeparting type's serial
                case 71:
                    newCheckpointSerial = 8;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 72:
                    newCheckpointSerial = 8;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                //sets the new checkpoint serial to FlightRunwayDepart checkpoint type's serial
                case 8:
                    newCheckpointSerial = 42;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightRunway", flight.IsAlive, isBoarding);
                //sets the new checkpoint serial to no defualt value & 'Departed' action
                case 9:
                    newCheckpointSerial = -1;
                    return "Departed!";
                default:
                    throw new Exception("Checkpoint serial not found!");
            }
        }

        /// <summary>
        /// Private method that retrieves the actual string of the TextBlock name property
        /// </summary>
        /// <param name="txtblckNameFlightNumber">the viewmodel's TextBlock name & current flight serial hash</param>
        /// <param name="lstvwNameFlightList">the viewmodel's ListView name & current flight serial list hash</param>
        /// <param name="controlName">the hard coded control name</param>
        /// <param name="isAlive">indicates if the flight has all of it's values & is in a process</param>
        /// <param name="isBoarding">indicates if the flight is in landing or departure process</param>
        /// <returns></returns>
        string RetrieveCheckpointName(Dictionary<string, string> txtblckNameFlightNumber, Dictionary<string, List<string>> lstvwNameFlightList, string controlName, bool isAlive, bool isBoarding)
        {
            if (txtblckNameFlightNumber != null)
            {
                if (txtblckNameFlightNumber[$"txtblck{controlName}"] == "0") return controlName;
                else
                {
                    if (isAlive) return "Stay in checkpoint!";
                    else return "No access to field!";
                }
            }
            else if (lstvwNameFlightList != null)
            {
                if (isBoarding) return controlName;
                else return "Stay in checkpoint!";
            }
            else throw new Exception("No dictionary data passed!");
        }
    }
}
