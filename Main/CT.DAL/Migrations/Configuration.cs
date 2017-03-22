namespace CT.DAL.Migrations
{
    using Common.Enums;
    using Entities;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CTContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CTContext context)
        {
            // bindings setup
            Checkpoint landing1 = context.Checkpoints.FirstOrDefault(cp => cp.Serial == 1);
            landing1.Control = "txtblckFlightArr1";
            context.SaveChanges();
            Checkpoint landing2 = context.Checkpoints.FirstOrDefault(cp => cp.Serial == 2);
            landing2.Control = "txtblckFlightArr2";
            context.SaveChanges();
            Checkpoint landing3 = context.Checkpoints.FirstOrDefault(cp => cp.Serial == 3);
            landing3.Control = "txtblckFlightArr3";
            context.SaveChanges();
            ICollection<Checkpoint> CheckpointRunway = context.Checkpoints.Where(cp => cp.Serial == 4).ToList();
            foreach (Checkpoint cp in CheckpointRunway) cp.Control = "txtblckFlightRunway";
            context.SaveChanges();
            Checkpoint preUnloading = context.Checkpoints.FirstOrDefault(cp => cp.Serial == 5);
            preUnloading.Control = "lstvwParkUnload";
            context.SaveChanges();
            Checkpoint unloading = context.Checkpoints.FirstOrDefault(cp => cp.Serial == 6);
            unloading.Control = "txtblckFlightTerminal1";
            context.SaveChanges();
            Checkpoint boarding = context.Checkpoints.FirstOrDefault(cp => cp.Serial == 7);
            boarding.Control = "txtblckFlightTerminal2";
            context.SaveChanges();

            // initial setup
            //Process landing = new Process()
            //{
            //    ProcessType = ProcessType.LandingProcess.ToString()
            //};
            //context.Processes.Add(landing);
            //context.SaveChanges();

            //Checkpoint arrival1 = new Checkpoint()
            //{
            //    Serial = 1,
            //    CheckpointType = CheckpointType.Landing.ToString(),
            //    Duration = 2000,
            //    Process = landing,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(arrival1);
            //landing.Checkpoints.Add(arrival1);
            //context.SaveChanges();

            //Checkpoint arrival2 = new Checkpoint()
            //{
            //    Serial = 2,
            //    CheckpointType = CheckpointType.Landing.ToString(),
            //    Duration = 2000,
            //    Process = landing,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(arrival2);
            //landing.Checkpoints.Add(arrival2);
            //context.SaveChanges();

            //Checkpoint arrival3 = new Checkpoint()
            //{
            //    Serial = 3,
            //    CheckpointType = CheckpointType.Landing.ToString(),
            //    Duration = 2000,
            //    Process = landing,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(arrival3);
            //landing.Checkpoints.Add(arrival3);
            //context.SaveChanges();

            //Checkpoint runwayLanded = new Checkpoint()
            //{
            //    Serial = 4,
            //    CheckpointType = CheckpointType.RunwayLanded.ToString(),
            //    Duration = 3000,
            //    Process = landing,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(runwayLanded);
            //landing.Checkpoints.Add(runwayLanded);
            //context.SaveChanges();

            //Checkpoint standbyLanded = new Checkpoint()
            //{
            //    Serial = 5,
            //    CheckpointType = CheckpointType.StandbyLanded.ToString(),
            //    Duration = 2000,
            //    Process = landing,
            //    Flights = new List<Flight>(100)
            //};
            //context.Checkpoints.Add(standbyLanded);
            //landing.Checkpoints.Add(standbyLanded);
            //context.SaveChanges();

            //Checkpoint parkingUnloading = new Checkpoint()
            //{
            //    Serial = 6,
            //    CheckpointType = CheckpointType.ParkingUnloading.ToString(),
            //    Duration = 5000,
            //    Process = landing,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(parkingUnloading);
            //landing.Checkpoints.Add(parkingUnloading);
            //context.SaveChanges();

            //Process departure = new Process()
            //{
            //    ProcessType = ProcessType.DepartingProcess.ToString()
            //};
            //context.Processes.Add(departure);
            //context.SaveChanges();

            //Checkpoint parkingBoarding = new Checkpoint()
            //{
            //    Serial = 7,
            //    CheckpointType = CheckpointType.ParkingBoarding.ToString(),
            //    Duration = 5000,
            //    Process = departure,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(parkingBoarding);
            //departure.Checkpoints.Add(parkingBoarding);
            //context.SaveChanges();

            //Checkpoint standbyDeparting = new Checkpoint()
            //{
            //    Serial = 8,
            //    CheckpointType = CheckpointType.StandbyDeparting.ToString(),
            //    Duration = 2000,
            //    Process = departure,
            //    Flights = new List<Flight>(100)
            //};
            //context.Checkpoints.Add(standbyDeparting);
            //departure.Checkpoints.Add(standbyDeparting);
            //context.SaveChanges();

            //Checkpoint runwayDeparting = new Checkpoint()
            //{
            //    Serial = 4,
            //    CheckpointType = CheckpointType.RunwayDeparting.ToString(),
            //    Duration = 3000,
            //    Process = departure,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(runwayDeparting);
            //departure.Checkpoints.Add(runwayDeparting);
            //context.SaveChanges();

            //Checkpoint departed = new Checkpoint()
            //{
            //    Serial = 9,
            //    CheckpointType = CheckpointType.Departed.ToString(),
            //    Duration = 2000,
            //    Process = departure,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(departed);
            //departure.Checkpoints.Add(departed);
            //context.SaveChanges();
        }
    }
}
