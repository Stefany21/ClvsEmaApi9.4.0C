namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addColumn_CardsPinpad_table_PaydeskBalances : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaydeskBalances", "CardsPinpad", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaydeskBalances", "CardsPinpad");
        }
    }
}
