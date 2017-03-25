using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CT.DAL.Entities;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using CT.Common.Enums;

namespace CT.DAL
{
    /// <summary>
    /// The control tower application's database context class 
    /// </summary>
    public partial class CTContext : DbContext
    {
        #region base data
        public CTContext()
            : base("name=CT_DB") { }

        public virtual DbSet<Checkpoint> Checkpoints { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<Process> Processes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
        #endregion

        #region service repository related operations
        public Flight CreateFlight(Flight flight)
        {
            try
            {
                Flights.Add(flight);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return flight;
        }

        public Flight GetFlight(int flightSerial)
        {
            return Flights.FirstOrDefault(f => f.FlightSerial == flightSerial);
        }

        public Checkpoint GetCheckpoint(int serial)
        {
            return Checkpoints.FirstOrDefault(cp => cp.Serial == serial);
        }

        public Process GetProcess(string processType)
        {
            return Processes.FirstOrDefault(p => p.ProcessType == processType);
        }

        /// <summary>
        /// Updates the checkpoints the current flight passes between
        /// </summary>
        /// <param name="newCheckpointSerial">the next checkpoint's serial</param>
        /// <param name="lastCheckpointSerial">the last checkpoint's serial</param>
        /// <param name="flight">the current flight</param>
        public void UpdateCheckpoint(int newCheckpointSerial, int lastCheckpointSerial, Flight flight)
        {
            //if the last checkpoint's serial is 8, it's the depart checkpoint
            if (lastCheckpointSerial == 8)
                Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint =
                    Checkpoints.FirstOrDefault(cp => cp.CheckpointType == CheckpointType.RunwayDeparting.ToString());
            //all other checkpoints will be updated by the new checkpoint serial
            else Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint =
                    Checkpoints.FirstOrDefault(cp => cp.Serial == newCheckpointSerial);
            SaveChanges();
        }

        /// <summary>
        /// Updates the current flight' checkpoint
        /// </summary>
        /// <param name="flight">the current flight</param>
        /// <param name="newCheckpointSerial">the next checkpoint's serial</param>
        /// <param name="lastCheckpointSerial">the last checkpoint's serial</param>
        /// <param name="isNew">indicates if the flight is not alive</param>
        public void UpdateFlight(Flight flight, int newCheckpointSerial, int lastCheckpointSerial, bool isNew)
        {
            //if the flight is new, it's alive state is modified
            if (isNew)
                Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).IsAlive = true;

            //f the last checkpoint's serial is 8, it's the depart checkpoint
            if (lastCheckpointSerial == 8)
                Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint =
                    Checkpoints.FirstOrDefault(cp => cp.CheckpointType == CheckpointType.RunwayDeparting.ToString());
            //all other checkpoints will be updated by the new checkpoint serial
            else Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint =
                    Checkpoints.FirstOrDefault(cp => cp.Serial == newCheckpointSerial);
            SaveChanges();
        }

        public bool DisposeFlight(int flightSerial)
        {
            Flight toRemove = Flights.FirstOrDefault(f => f.FlightSerial == flightSerial);
            try
            {
                Flights.Remove(toRemove);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return true;
        }
        #endregion

        #region database initialization
        public void InitializeDatabase()
        {
            ClearDatabase();
            //InitializeLandingProcess();
            //InitializeDepartureProcess();
        }
        void ClearDatabase()
        {
            IEnumerable<Flight> flightEntities = Flights.ToList();
            if (flightEntities.Count() > 0)
            {
                Flights.RemoveRange(flightEntities);
                SaveChanges();
            }
        }
        void InitializeLandingProcess()
        {
            Process landing = new Process()
            {
                ProcessType = ProcessType.LandingProcess.ToString()
            };
            Processes.Add(landing);
            SaveChanges();

            Checkpoint arrival1 = new Checkpoint()
            {
                Serial = 1,
                Control = "txtblckFlightArr1",
                CheckpointType = CheckpointType.Landing.ToString(),
                Duration = 2000,
                Process = landing,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(arrival1);
            landing.Checkpoints.Add(arrival1);
            SaveChanges();

            Checkpoint arrival2 = new Checkpoint()
            {
                Serial = 2,
                Control = "txtblckFlightArr2",
                CheckpointType = CheckpointType.Landing.ToString(),
                Duration = 2000,
                Process = landing,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(arrival2);
            landing.Checkpoints.Add(arrival2);
            SaveChanges();

            Checkpoint arrival3 = new Checkpoint()
            {
                Serial = 3,
                Control = "txtblckFlightArr3",
                CheckpointType = CheckpointType.Landing.ToString(),
                Duration = 2000,
                Process = landing,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(arrival3);
            landing.Checkpoints.Add(arrival3);
            SaveChanges();

            Checkpoint runwayLanded = new Checkpoint()
            {
                Serial = 41,
                Control = "txtblckFlightRunway",
                CheckpointType = CheckpointType.RunwayLanded.ToString(),
                Duration = 3000,
                Process = landing,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(runwayLanded);
            landing.Checkpoints.Add(runwayLanded);
            SaveChanges();

            Checkpoint standbyLanded = new Checkpoint()
            {
                Serial = 5,
                Control = "lstvwParkUnload",
                CheckpointType = CheckpointType.StandbyLanded.ToString(),
                Duration = 2000,
                Process = landing,
                Flights = new List<Flight>(100)
            };
            Checkpoints.Add(standbyLanded);
            landing.Checkpoints.Add(standbyLanded);
            SaveChanges();

            Checkpoint parkingUnloading_terminal1 = new Checkpoint()
            {
                Serial = 61,
                Control = "txtblckFlightTerminal1",
                CheckpointType = CheckpointType.ParkingUnloading_terminal1.ToString(),
                Duration = 5000,
                Process = landing,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(parkingUnloading_terminal1);
            landing.Checkpoints.Add(parkingUnloading_terminal1);
            SaveChanges();

            Checkpoint parkingUnloading_terminal2 = new Checkpoint()
            {
                Serial = 62,
                Control = "txtblckFlightTerminal2",
                CheckpointType = CheckpointType.ParkingUnloading_terminal2.ToString(),
                Duration = 5000,
                Process = landing,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(parkingUnloading_terminal2);
            landing.Checkpoints.Add(parkingUnloading_terminal2);
            SaveChanges();
        }
        void InitializeDepartureProcess()
        {
            Process departure = new Process()
            {
                ProcessType = ProcessType.DepartingProcess.ToString()
            };
            Processes.Add(departure);
            SaveChanges();

            Checkpoint parkingBoarding_terminal1 = new Checkpoint()
            {
                Serial = 71,
                Control = "txtblckFlightTerminal1",
                CheckpointType = CheckpointType.ParkingBoarding_terminal1.ToString(),
                Duration = 5000,
                Process = departure,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(parkingBoarding_terminal1);
            departure.Checkpoints.Add(parkingBoarding_terminal1);
            SaveChanges();

            Checkpoint parkingBoarding_terminal2 = new Checkpoint()
            {
                Serial = 72,
                Control = "txtblckFlightTerminal2",
                CheckpointType = CheckpointType.ParkingBoarding_terminal2.ToString(),
                Duration = 5000,
                Process = departure,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(parkingBoarding_terminal2);
            departure.Checkpoints.Add(parkingBoarding_terminal2);
            SaveChanges();

            Checkpoint standbyDeparting = new Checkpoint()
            {
                Serial = 8,
                Control = "lstvwParkDepart",
                CheckpointType = CheckpointType.StandbyDeparting.ToString(),
                Duration = 2000,
                Process = departure,
                Flights = new List<Flight>(100)
            };
            Checkpoints.Add(standbyDeparting);
            departure.Checkpoints.Add(standbyDeparting);
            SaveChanges();

            Checkpoint runwayDeparting = new Checkpoint()
            {
                Serial = 42,
                Control = "txtblckFlightRunway",
                CheckpointType = CheckpointType.RunwayDeparting.ToString(),
                Duration = 3000,
                Process = departure,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(runwayDeparting);
            departure.Checkpoints.Add(runwayDeparting);
            SaveChanges();

            Checkpoint departed = new Checkpoint()
            {
                Serial = 9,
                Control = "txtblckFlightDepart",
                CheckpointType = CheckpointType.Departed.ToString(),
                Duration = 2000,
                Process = departure,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(departed);
            departure.Checkpoints.Add(departed);
            SaveChanges();
        }
        #endregion
    }
}
