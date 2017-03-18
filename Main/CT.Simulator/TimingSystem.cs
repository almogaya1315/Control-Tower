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
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightRunway", flight.IsAlive, isBoarding);
                case 4:
                    if (flight.Checkpoint.CheckpointType == CheckpointType.RunwayLanded.ToString())
                    {
                        newCheckpointSerial = flight.Checkpoint.Serial + 1;
                        return "lstvwParkUnload";
                    }
                    else
                    {
                        newCheckpointSerial = 9;
                        return "txtblckFlightDepart";
                    }
                case 5:
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    if (txtblckNameFlightNumber["txtblckFlightTerminal1"] == "---")
                        return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightTerminal1", flight.IsAlive, isBoarding);
                    if (txtblckNameFlightNumber["txtblckFlightTerminal2"] == "---")
                        return RetrieveCheckpointName(txtblckNameFlightNumber, null, "FlightTerminal2", flight.IsAlive, isBoarding);
                    return "Stay in checkpoint!";
                case 6:
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 7:
                    newCheckpointSerial = flight.Checkpoint.Serial + 1;
                    return RetrieveCheckpointName(null, lstvwNameFlightList, "lstvwParkDepart", flight.IsAlive, isBoarding);
                case 8:
                    newCheckpointSerial = 4;
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
                if (txtblckNameFlightNumber[$"txtblck{controlName}"] == "---") return controlName;
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
            else throw new Exception("No dictionery data passed!");
        }
    }
}
