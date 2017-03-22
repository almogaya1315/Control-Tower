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
            //Process landing = new Process()
            //{
            //    ProcessType = ProcessType.LandingProcess.ToString()
            //};
            //context.Processes.Add(landing);
            //context.SaveChanges();

            //Checkpoint arrival1 = new Checkpoint()
            //{
            //    Serial = 1,
            //    Control = "txtblckFlightArr1",
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
            //    Control = "txtblckFlightArr2",
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
            //    Control = "txtblckFlightArr3",
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
            //    Serial = 41,
            //    Control = "txtblckFlightRunway",
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
            //    Control = "lstvwParkUnload",
            //    CheckpointType = CheckpointType.StandbyLanded.ToString(),
            //    Duration = 2000,
            //    Process = landing,
            //    Flights = new List<Flight>(100)
            //};
            //context.Checkpoints.Add(standbyLanded);
            //landing.Checkpoints.Add(standbyLanded);
            //context.SaveChanges();

            //Checkpoint parkingUnloading_terminal1 = new Checkpoint()
            //{
            //    Serial = 61,
            //    Control = "txtblckFlightTerminal1",
            //    CheckpointType = CheckpointType.ParkingUnloading_terminal1.ToString(),
            //    Duration = 5000,
            //    Process = landing,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(parkingUnloading_terminal1);
            //landing.Checkpoints.Add(parkingUnloading_terminal1);
            //context.SaveChanges();

            //Checkpoint parkingUnloading_terminal2 = new Checkpoint()
            //{
            //    Serial = 62,
            //    Control = "txtblckFlightTerminal2",
            //    CheckpointType = CheckpointType.ParkingUnloading_terminal2.ToString(),
            //    Duration = 5000,
            //    Process = landing,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(parkingUnloading_terminal2);
            //landing.Checkpoints.Add(parkingUnloading_terminal2);
            //context.SaveChanges();

            //Process departure = new Process()
            //{
            //    ProcessType = ProcessType.DepartingProcess.ToString()
            //};
            //context.Processes.Add(departure);
            //context.SaveChanges();

            //Checkpoint parkingBoarding_terminal1 = new Checkpoint()
            //{
            //    Serial = 71,
            //    Control = "txtblckFlightTerminal1",
            //    CheckpointType = CheckpointType.ParkingBoarding_terminal1.ToString(),
            //    Duration = 5000,
            //    Process = departure,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(parkingBoarding_terminal1);
            //departure.Checkpoints.Add(parkingBoarding_terminal1);
            //context.SaveChanges();

            //Checkpoint parkingBoarding_terminal2 = new Checkpoint()
            //{
            //    Serial = 72,
            //    Control = "txtblckFlightTerminal2",
            //    CheckpointType = CheckpointType.ParkingBoarding_terminal2.ToString(),
            //    Duration = 5000,
            //    Process = departure,
            //    Flights = new List<Flight>(1)
            //};
            //context.Checkpoints.Add(parkingBoarding_terminal2);
            //departure.Checkpoints.Add(parkingBoarding_terminal2);
            //context.SaveChanges();

            //Checkpoint standbyDeparting = new Checkpoint()
            //{
            //    Serial = 8,
            //    Control = "lstvwParkDepart",
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
            //    Serial = 42,
            //    Control = "txtblckFlightRunway",
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
            //    Control = "txtblckFlightDepart",
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
