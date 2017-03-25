using CT.Common.DTO_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CT.Common.IRepositories
{
    /// <summary>
    /// The interface for the CT app repository
    /// </summary>
    public interface IControlTowerRepository
    {
        ICollection<ProcessDTO> ProcessesDTOs { get; set; }
        ICollection<CheckpointDTO> CheckPointsDTOs { get; set; }
        ICollection<FlightDTO> FlightsDTOs { get; set; }

        FlightDTO CreateFlightObject(int flightSerial);
        FlightDTO GetFlightObject(int flightSerial);
        ICollection<FlightDTO> GetFlightsCollection();

        CheckpointDTO GetCheckpoint(string checkpointSerial, string checkpointType);

        string UpdateCheckpoints(int newCheckpointSerial, string lastCheckpointPosition, int lastCheckpointSerial, FlightDTO flight);
        string GetFlightCheckpoint(Dictionary<string, string> txtblckNameFlightNumber, Dictionary<string, List<string>> lstvwsNameFlightsList, string flightSerial, bool isBoarding, out int lastCheckpointSerial);
        void UpdateFlightObject(FlightDTO flight, int newCheckpointSerial, int lastCheckpointSerial, bool isNew);
        void InitializeDatabase();

        bool DisposeFlight(int flightSerial);
    }
}
