namespace ProjectInternShip.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _030822 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.data_table", "sensor_value_1", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.data_table", "sensor_value_1");
        }
    }
}
