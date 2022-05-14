namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class decimalAmountPerDocumentSectionOnCompanysModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "DecimalAmountPrice", c => c.Int(nullable: false));
            AddColumn("dbo.Companys", "DecimalAmountTotalLine", c => c.Int(nullable: false));
            AddColumn("dbo.Companys", "DecimalAmountTotalDocuent", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "DecimalAmountTotalDocuent");
            DropColumn("dbo.Companys", "DecimalAmountTotalLine");
            DropColumn("dbo.Companys", "DecimalAmountPrice");
        }
    }
}
