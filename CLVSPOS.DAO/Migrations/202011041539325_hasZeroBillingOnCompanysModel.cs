namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hasZeroBillingOnCompanysModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "HasZeroBilling", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "HasZeroBilling");
        }
    }
}
