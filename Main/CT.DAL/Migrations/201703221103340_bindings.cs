namespace CT.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bindings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Checkpoints", "Control", c => c.String(nullable: false));
            AddColumn("dbo.Flights", "CheckpointControl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Flights", "CheckpointControl");
            DropColumn("dbo.Checkpoints", "Control");
        }
    }
}
