namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteFieldOnCompanysModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Companys", "DecimalAmountTotalDocuent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companys", "DecimalAmountTotalDocuent", c => c.Int(nullable: false));
        }
    }
}
