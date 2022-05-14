namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PP_models : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PPBalances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        XMLDocumentResponse = c.String(),
                        ResponseCode = c.String(),
                        ResponseCodeDescription = c.String(),
                        AcqNumber = c.String(),
                        CardBrand = c.String(),
                        HotTime = c.String(),
                        HostDate = c.String(),
                        RefundsAmount = c.String(),
                        RefundsTransactions = c.String(),
                        SalesTransactions = c.String(),
                        SalesAmount = c.String(),
                        SalesTax = c.String(),
                        SalesTip = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        TransactionType = c.String(),
                        TerminalCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PPTerminals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Password = c.String(),
                        TerminalId = c.String(),
                        Status = c.Boolean(nullable: false),
                        Currency = c.String(),
                        QuickPayAmount = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PPTerminalByUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        TerminalId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PPTransactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocEntry = c.Int(nullable: false),
                        DocNum = c.Int(nullable: false),
                        CardName = c.String(),
                        CardNumber = c.String(),
                        AuthorizationNumber = c.String(),
                        EntryMode = c.String(),
                        ExpirationDate = c.String(),
                        ReferenceNumber = c.String(),
                        TerminalId = c.Int(nullable: false),
                        ResponseCode = c.String(),
                        InvoiceNumber = c.String(),
                        InvoiceDocument = c.String(),
                        SystemTrace = c.String(),
                        TransactionId = c.String(),
                        CharchedStatus = c.String(),
                        ChargedResponse = c.String(),
                        CanceledStatus = c.String(),
                        CanceledResponse = c.String(),
                        ReversedStatus = c.String(),
                        ReversedResponse = c.String(),
                        Currency = c.String(),
                        BacId = c.String(),
                        UserPrefix = c.String(),
                        Amount = c.Double(nullable: false),
                        IsOnBalance = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpDate = c.DateTime(),
                        AcqPrebalance = c.Int(nullable: false),
                        AcqBalance = c.Int(nullable: false),
                        SaleAmount = c.Double(nullable: false),
                        UpdatesCounter = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PPTransactionLoggers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Attempts = c.Int(nullable: false),
                        DocumentReference = c.String(),
                        Type = c.String(),
                        XmlDocumentResponse = c.String(),
                        BACResponseCode = c.String(),
                        Status = c.Int(nullable: false),
                        StarTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PPTransactionLoggers");
            DropTable("dbo.PPTransactions");
            DropTable("dbo.PPTerminalByUsers");
            DropTable("dbo.PPTerminals");
            DropTable("dbo.PPBalances");
        }
    }
}
