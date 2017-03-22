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
    public class TimingSystem
    {
        public string GetFlightPosition(Dictionary<string, string> txtblckNameFlightNumber, Dictionary<string, List<string>> lstvwNameFlightList, FlightDTO flight, bool isBoarding, out int newCheckpointSerial)
        {
            if (flight.Checkpoint == null)
            {
                newCheckpointSerial = 1;
                return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightArr1", flight.IsAlive, isBoarding);
            }
            switch (flight.Checkpoint.Serial)
            {
                case 1:
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightArr2", flight.IsAlive, isBoarding);
                case 2:
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightArr3", flight.IsAlive, isBoarding);
                case 3:
                    newCheckpointSerial = 41;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightRunway", flight.IsAlive, isBoarding);
                case 41:
                    newCheckpointSerial = 5;
                    return "lstvwParkUnload";
                case 42:
                    newCheckpointSerial = 9;
                    return "txtblckFlightDepart";
                //case 4:
                //    if (flight.Checkpoint.CheckpointType == CheckpointType.RunwayLanded.ToString())
                //    {
                //        newCheckpointSerial = flight.Checkpoint.Serial + 1;
                //        return "lstvwParkUnload";
                //    }
                //    else
                //    {
                //        newCheckpointSerial = 9;
                //        return "txtblckFlightDepart";
                //    }
                case 5:
                    if (txtblckNameFlightNumber["txtblckFlightTerminal1"] == "---")
                    {
                        newCheckpointSerial = 61;
                        return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightTerminal1", flight.IsAlive, isBoarding);
                    }
                    else if (txtblckNameFlightNumber["txtblckFlightTerminal2"] == "---")
                    {
                        newCheckpointSerial = 62;
                        return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightTerminal2", flight.IsAlive, isBoarding);
                    }
                    else
                    {
                        newCheckpointSerial = 5;
                        return "Stay in checkpoint!";
                    }
                case 61:
                    newCheckpointSerial = 71;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 62:
                    newCheckpointSerial = 72;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                //case 6:
                //    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                //    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 71:
                    newCheckpointSerial = 8;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 72:
                    newCheckpointSerial = 8;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                //case 7:
                //    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                //    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 8:
                    newCheckpointSerial = 42;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightRunway", flight.IsAlive, isBoarding);
                case 9:
                    newCheckpointSerial = -1;
                    return "Departed!";
                default:
                    throw new Exception("Checkpoint serial not found!");
            }
        }

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
