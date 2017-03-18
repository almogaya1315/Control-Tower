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
    public partial class CTContext : DbContext
    {
        public CTContext()
            : base("name=CT_DB")
        {
        }

        public virtual DbSet<Checkpoint> Checkpoints { get; set; }
        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<Process> Processes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Flight>()
            //    .HasRequired(f => f.Process)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Flight>()
            //    .HasOptional(f => f.CheckpointId);

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

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

        public void UpdateCheckpoint(int newCheckpointSerial, int lastCheckpointSerial, Flight flight)
        {
            if (lastCheckpointSerial == 8)
                Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint =
                    Checkpoints.FirstOrDefault(cp => cp.CheckpointType == CheckpointType.RunwayDeparting.ToString());
            else Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint =
                    Checkpoints.FirstOrDefault(cp => cp.Serial == newCheckpointSerial);
            SaveChanges();
        }
        public void UpdateFlight(Flight flight, int newCheckpointSerial, int lastCheckpointSerial, bool isNew)
        {
            if (isNew)
                Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).IsAlive = true;

            if (lastCheckpointSerial == 8)
                Flights.FirstOrDefault(f => f.FlightSerial == flight.FlightSerial).Checkpoint =
                    Checkpoints.FirstOrDefault(cp => cp.CheckpointType == CheckpointType.RunwayDeparting.ToString());
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
            
            //if (Flights.Contains(toRemove) == false) return true;
            //else return false;
        }

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

            //IEnumerable<Checkpoint> checkpointEntities = Checkpoints.ToList();
            //Checkpoints.RemoveRange(checkpointEntities);
            //SaveChanges();

            //IEnumerable<Process> processEntities = Processes.ToList();
            //Processes.RemoveRange(processEntities);
            //SaveChanges();
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
                Serial = 4,
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
                CheckpointType = CheckpointType.StandbyLanded.ToString(),
                Duration = 2000,
                Process = landing,
                Flights = new List<Flight>(100)
            };
            Checkpoints.Add(standbyLanded);
            landing.Checkpoints.Add(standbyLanded);
            SaveChanges();

            Checkpoint parkingUnloading = new Checkpoint()
            {
                Serial = 6,
                CheckpointType = CheckpointType.ParkingUnloading.ToString(),
                Duration = 5000,
                Process = landing,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(parkingUnloading);
            landing.Checkpoints.Add(parkingUnloading);
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

            Checkpoint parkingBoarding = new Checkpoint()
            {
                Serial = 7,
                CheckpointType = CheckpointType.ParkingBoarding.ToString(),
                Duration = 5000,
                Process = departure,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(parkingBoarding);
            departure.Checkpoints.Add(parkingBoarding);
            SaveChanges();

            Checkpoint standbyDeparting = new Checkpoint()
            {
                Serial = 8,
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
                Serial = 4,
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
                CheckpointType = CheckpointType.Departed.ToString(),
                Duration = 2000,
                Process = departure,
                Flights = new List<Flight>(1)
            };
            Checkpoints.Add(departed);
            departure.Checkpoints.Add(departed);
            SaveChanges();
        }
    }
}
