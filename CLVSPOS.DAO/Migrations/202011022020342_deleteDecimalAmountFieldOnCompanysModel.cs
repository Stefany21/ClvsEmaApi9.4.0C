namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteDecimalAmountFieldOnCompanysModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Companys", "DecimalAmount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companys", "DecimalAmount", c => c.Int(nullable: false));
        }
    }
}
