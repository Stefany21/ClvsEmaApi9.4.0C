namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createTable_PaydeskBalance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaydeskBalances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserSignature = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Cash = c.Double(nullable: false),
                        Cards = c.Double(nullable: false),
                        Transfer = c.Double(nullable: false),
                        CashflowIncomme = c.Double(nullable: false),
                        CashflowEgress = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PaydeskBalances");
        }
    }
}
