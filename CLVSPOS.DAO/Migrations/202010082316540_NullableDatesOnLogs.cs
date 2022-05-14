namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableDatesOnLogs : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Logs", "TypeDocument", c => c.Int(nullable: false));
            AlterColumn("dbo.Logs", "StartTimeDocument", c => c.DateTime());
            AlterColumn("dbo.Logs", "EndTimeDocument", c => c.DateTime());
            AlterColumn("dbo.Logs", "StartTimeCompany", c => c.DateTime());
            AlterColumn("dbo.Logs", "EndTimeCompany", c => c.DateTime());
            AlterColumn("dbo.Logs", "StartTimeSapDocument", c => c.DateTime());
            AlterColumn("dbo.Logs", "EndTimeSapDocument", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Logs", "EndTimeSapDocument", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Logs", "StartTimeSapDocument", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Logs", "EndTimeCompany", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Logs", "StartTimeCompany", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Logs", "EndTimeDocument", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Logs", "StartTimeDocument", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Logs", "TypeDocument", c => c.String());
        }
    }
}
