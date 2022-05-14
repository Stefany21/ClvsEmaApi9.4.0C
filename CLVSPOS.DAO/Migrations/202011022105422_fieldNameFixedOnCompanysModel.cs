namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldNameFixedOnCompanysModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "DecimalAmountTotalDocument", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "DecimalAmountTotalDocument");
        }
    }
}
