namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoDecimalAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "DecimalAmount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "DecimalAmount");
        }
    }
}
