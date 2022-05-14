namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Camposcedula : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "FEIdType", c => c.String(maxLength: 2));
            AddColumn("dbo.Companys", "FEIdNumber", c => c.String(maxLength: 12));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "FEIdNumber");
            DropColumn("dbo.Companys", "FEIdType");
        }
    }
}
