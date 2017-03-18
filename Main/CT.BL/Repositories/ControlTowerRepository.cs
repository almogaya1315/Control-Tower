﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CT.Common.IRepositories;
using CT.Common.DTO_Models;
using CT.Common.Utilities;
using CT.Common.Abstracts;
using CT.DAL.Entities;
using CT.DAL;
using System.Windows.Controls;
using CT.Common.Enums;

namespace CT.BL.Repositories
{
    public class ControlTowerRepository : RepositoryConvertor<Flight, FlightDTO>, IControlTowerRepository
    {
        public CTContext CTDB { get; private set; }
        public ICollection<ProcessDTO> ProcessesDTOs { get; set; }
        public ICollection<CheckpointDTO> CheckPointsDTOs { get; set; }
        public ICollection<FlightDTO> FlightsDTOs { get; set; }

        public ControlTowerRepository()
        {
            CTDB = new CTContext();
            ProcessesDTOs = new List<ProcessDTO>();
            CheckPointsDTOs = new List<CheckpointDTO>();
            FlightsDTOs = new List<FlightDTO>();
        }

        public FlightDTO CreateFlightObject(int flightSerial)
        {
            ProcessDTO process = new ProcessDTO()
            {
                ProcessType = CTDB.GetProcess(ProcessType.LandingProcess.ToString()).ProcessType,
                ProcessId = CTDB.GetProcess(ProcessType.LandingProcess.ToString()).ProcessId,
                Flights = new List<FlightDTO>()
            };
            foreach (Checkpoint cp in CTDB.GetProcess(ProcessType.LandingProcess.ToString()).Checkpoints)
            {
                process.Checkpoints.Add(new CheckpointDTO()
                {
                    CheckpointType = cp.CheckpointType,
                    CheckpointId = cp.CheckpointId,
                    Process = process,
                    ProcessId = cp.ProcessId,
                    Duration = cp.Duration,
                    Serial = cp.Serial,
                    Flights = new List<FlightDTO>()
                });
            }
            FlightDTO flight = new FlightDTO()
            {
                FlightSerial = flightSerial,
                IsAlive = false,
                Process = process,
                ProcessId = process.ProcessId
            };
            FlightsDTOs.Add(flight);
            return flight = ConvertToDTO(CTDB.CreateFlight(ConvertToEntity(flight)));
        }

        public FlightDTO GetFlightObject(int flightSerial)
        {
            return ConvertToDTO(CTDB.GetFlight(flightSerial));
        }

        public ICollection<FlightDTO> GetFlightsCollection()
        {
            ICollection<FlightDTO> flights = new List<FlightDTO>();
            foreach (Flight flight in CTDB.Flights)
            {
                flights.Add(ConvertToDTO(flight));
            }
            return flights;
        }

        /// <summary>
        /// Updates the associated checkpoint's flight list with the flight object
        /// </summary>
        /// <param name="newCheckpointSerial">The new flight's next checkpoint serial</param>
        /// <param name="checkpointPosition">The current flight's position as a TextBlock name property</param>
        /// <param name="checkpointSerial">The current fligh's checkpoint serial</param>
        /// <param name="flight">The associated flight object</param>
        /// <returns>Return a string format for the flight's next checkpoint type</returns>
        public string UpdateCheckpoints(int newCheckpointSerial, string lastCheckpointPosition, int lastCheckpointSerial, FlightDTO flight)
        {
            if (lastCheckpointPosition == "none")
            {
                UpdateFlightObject(flight, newCheckpointSerial, lastCheckpointSerial, true);
                return CheckpointType.Landing.ToString();
            }
            CTDB.UpdateCheckpoint(newCheckpointSerial, lastCheckpointSerial, ConvertToEntity(flight));
            if (lastCheckpointPosition == "txtblckFlightArr3") return CheckpointType.RunwayLanded.ToString();
            else if (lastCheckpointPosition == "lstvwParkDepart") return CheckpointType.RunwayDeparting.ToString();
            else return CTDB.Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint.CheckpointType.ToString();
        }

        public void UpdateFlightObject(FlightDTO flight, int newCheckpointSerial, int lastCheckpointSerial, bool isNew)
        {
            CTDB.UpdateFlight(ConvertToEntity(flight), newCheckpointSerial, lastCheckpointSerial, isNew);
        }

        /// <summary>
        /// Retrieves a checkpoint's TextBlock control name property by a flight object
        /// </summary>
        /// <param name="textBlocks">The UI list of TextBlock controls</param>
        /// <param name="flightSerial">The flight serial to retrieve it's current checkpoint</param>
        /// <param name="checkpointSerial">out ref of the checkpoint serial</param>
        /// <returns>Returns a string format for the flight's checkpoint position represented as the TextBlock control name property)</returns>
        public string GetFlightCheckpoint(Dictionary<string, string> txtblckNameFlightNumber, Dictionary<string, List<string>> lstvwNameFlightsList,
            string flightSerial, bool isBoarding, out int checkpointSerial)
        {
            string txtblckName = default(string);
            try { txtblckName = txtblckNameFlightNumber.Keys.FirstOrDefault(blockName => txtblckNameFlightNumber[blockName] == flightSerial); }
            catch { }

            checkpointSerial = -1;
            if (txtblckName == null)
            {
                string lstvwName = lstvwNameFlightsList.Keys.FirstOrDefault(listName => lstvwNameFlightsList[listName].Contains(flightSerial));
                if (lstvwName == "lstvwParkUnload") checkpointSerial = 5;
                else if (lstvwName == "lstvwParkDepart") checkpointSerial = 8;
                else if (lstvwName == null) return "none";
                return lstvwName;
            }
            switch (txtblckName)
            {
                case "txtblckFlightArr1":
                    checkpointSerial = 1;
                    return txtblckName;
                case "txtblckFlightArr2":
                    checkpointSerial = 2;
                    return txtblckName;
                case "txtblckFlightArr3":
                    checkpointSerial = 3;
                    return txtblckName;
                case "txtblckFlightRunway":
                    checkpointSerial = 4;
                    return txtblckName;
                case "txtblckFlightTerminal1":
                    if (isBoarding) checkpointSerial = 7;
                    else checkpointSerial = 6;
                    return txtblckName;
                case "txtblckFlightTerminal2":
                    if (isBoarding) checkpointSerial = 7;
                    else checkpointSerial = 6;
                    return txtblckName;
                case "txtblckFlightDepart":
                    checkpointSerial = 9;
                    return txtblckName;
                default: return txtblckName;
            }
        }

        public CheckpointDTO GetCheckpoint(string checkpointSerial, string checkpointType)
        {
            int serial = default(int);
            int.TryParse(checkpointSerial, out serial);
            return new CheckpointDTO()
            { Duration = CTDB.GetCheckpoint(serial).Duration };
        }


        protected override FlightDTO ConvertToDTO(Flight entity)
        {
            if (entity == null) return null;
            FlightDTO flight = new FlightDTO()
            {
                FlightSerial = entity.FlightSerial,
                IsAlive = entity.IsAlive
            };
            if (entity.Checkpoint != null)
            {
                flight.Checkpoint = new CheckpointDTO()
                {
                    CheckpointType = entity.Checkpoint.CheckpointType,
                    CheckpointId = entity.Checkpoint.CheckpointId,
                    Duration = entity.Checkpoint.Duration,
                    Serial = entity.Checkpoint.Serial,
                    Process = new ProcessDTO() { ProcessType = entity.Checkpoint.Process.ProcessType, ProcessId = entity.Checkpoint.Process.ProcessId },
                    ProcessId = entity.Checkpoint.Process.ProcessId
                };
                flight.CheckpointId = entity.Checkpoint.CheckpointId;
                flight.Process = flight.Checkpoint.Process;
                flight.ProcessId = flight.Checkpoint.ProcessId;
            }
            return flight;
        }
        protected override Flight ConvertToEntity(FlightDTO dto)
        {
            Flight flight = new Flight()
            {
                FlightSerial = dto.FlightSerial,
                IsAlive = dto.IsAlive
            };
            if (dto.Checkpoint != null)
            {
                flight.Checkpoint = CTDB.GetCheckpoint(dto.Checkpoint.Serial);
                flight.CheckpointId = CTDB.GetCheckpoint(dto.Checkpoint.Serial).CheckpointId;
            }
            return flight;
        }

        public bool DisposeFlight(int flightSerial)
        {
            return CTDB.DisposeFlight(flightSerial);
        }

        public void InitializeDatabase()
        {
            try
            {
                CTDB.InitializeDatabase();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            FlightsDTOs.Clear();
        }
    }
}
