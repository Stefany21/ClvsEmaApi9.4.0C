namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class printerConfigurationOnCompanyModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "PrinterConfiguration", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "PrinterConfiguration");
        }
    }
}
