namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HasOfflineModeOnCompanys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "HasOfflineMode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "HasOfflineMode");
        }
    }
}
