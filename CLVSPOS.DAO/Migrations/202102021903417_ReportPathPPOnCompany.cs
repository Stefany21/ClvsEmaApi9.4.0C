namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportPathPPOnCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "ReportPathPP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "ReportPathPP");
        }
    }
}
