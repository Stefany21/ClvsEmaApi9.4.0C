namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationSps_WebConfig_To_DB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "SP_GetItemByCodeBar", c => c.String());
            AddColumn("dbo.Companys", "SP_GetItemPriceList", c => c.String());
            AddColumn("dbo.Companys", "SP_GetBarcodesByItem", c => c.String());
            AddColumn("dbo.Companys", "V_GetSuppliers", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "V_GetSuppliers");
            DropColumn("dbo.Companys", "SP_GetBarcodesByItem");
            DropColumn("dbo.Companys", "SP_GetItemPriceList");
            DropColumn("dbo.Companys", "SP_GetItemByCodeBar");
        }
    }
}
