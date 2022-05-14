namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportRecivedPaidPath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "ReportRecivedPaid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "ReportRecivedPaid");
        }
    }
}
