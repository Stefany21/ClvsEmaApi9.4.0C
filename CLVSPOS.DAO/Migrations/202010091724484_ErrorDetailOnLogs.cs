namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ErrorDetailOnLogs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "ErrorDetail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "ErrorDetail");
        }
    }
}
