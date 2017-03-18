namespace CT.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Checkpoints",
                c => new
                    {
                        CheckpointId = c.Int(nullable: false, identity: true),
                        CheckpointType = c.String(nullable: false, maxLength: 40),
                        Serial = c.Int(nullable: false),
                        Duration = c.Int(nullable: false),
                        ProcessId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CheckpointId)
                .ForeignKey("dbo.Processes", t => t.ProcessId)
                .Index(t => t.ProcessId);
            
            CreateTable(
                "dbo.Flights",
                c => new
                    {
                        FlightId = c.Int(nullable: false, identity: true),
                        FlightSerial = c.Int(nullable: false),
                        IsAlive = c.Boolean(nullable: false),
                        CheckpointId = c.Int(),
                    })
                .PrimaryKey(t => t.FlightId)
                .ForeignKey("dbo.Checkpoints", t => t.CheckpointId)
                .Index(t => t.CheckpointId);
            
            CreateTable(
                "dbo.Processes",
                c => new
                    {
                        ProcessId = c.Int(nullable: false, identity: true),
                        ProcessType = c.String(nullable: false, maxLength: 30),
                    })
                .PrimaryKey(t => t.ProcessId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Checkpoints", "ProcessId", "dbo.Processes");
            DropForeignKey("dbo.Flights", "CheckpointId", "dbo.Checkpoints");
            DropIndex("dbo.Flights", new[] { "CheckpointId" });
            DropIndex("dbo.Checkpoints", new[] { "ProcessId" });
            DropTable("dbo.Processes");
            DropTable("dbo.Flights");
            DropTable("dbo.Checkpoints");
        }
    }
}
