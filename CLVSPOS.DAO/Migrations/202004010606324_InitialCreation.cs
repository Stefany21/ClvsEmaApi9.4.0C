namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyByUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        CompanyId = c.Int(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Active = c.Boolean(nullable: false),
                        CompanyByUsers_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompanyByUsers", t => t.CompanyByUsers_Id)
                .Index(t => t.CompanyByUsers_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(),
                        Email = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        Active = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Companys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DBName = c.String(nullable: false),
                        DBCode = c.String(nullable: false, maxLength: 450),
                        SAPConnectionId = c.Int(nullable: false),
                        LogoPath = c.String(),
                        Active = c.Boolean(nullable: false),
                        MailDataId = c.Int(),
                        ExchangeRate = c.Int(nullable: false),
                        ExchangeRateValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        HandleItem = c.Int(nullable: false),
                        BillItem = c.Int(nullable: false),
                        SP_ItemInfo = c.String(),
                        SP_InvoiceInfoPrint = c.String(),
                        V_BPS = c.String(),
                        V_Items = c.String(),
                        V_ExRate = c.String(),
                        V_Taxes = c.String(),
                        SP_WHAvailableItem = c.String(),
                        SP_SeriesByItem = c.String(),
                        SP_PayDocuments = c.String(),
                        V_GetAccounts = c.String(),
                        V_GetCards = c.String(),
                        V_GetBanks = c.String(),
                        V_GetSalesMan = c.String(),
                        SP_CancelPayment = c.String(),
                        SAPUser = c.String(),
                        SAPPass = c.String(),
                        ReportPath = c.String(),
                        ReportPathInventory = c.String(),
                        ReportPathCopy = c.String(),
                        ReportPathQuotation = c.String(),
                        ReportPathSO = c.String(),
                        ReportBalance = c.String(),
                        IsLinePriceEditable = c.Boolean(nullable: false),
                        ScaleMaxWeightToTreatAsZero = c.Double(nullable: false),
                        ScaleWeightToSubstract = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SAPConnections", t => t.SAPConnectionId, cascadeDelete: true)
                .ForeignKey("dbo.MailDatas", t => t.MailDataId)
                .Index(t => t.DBCode, unique: true)
                .Index(t => t.SAPConnectionId)
                .Index(t => t.MailDataId);
            
            CreateTable(
                "dbo.SAPConnections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Server = c.String(nullable: false),
                        LicenseServer = c.String(),
                        BoSuppLangs = c.String(),
                        DST = c.String(nullable: false),
                        DBUser = c.String(),
                        DBPass = c.String(),
                        UseTrusted = c.Boolean(nullable: false),
                        ODBCType = c.String(),
                        DBEngine = c.String(),
                        ServerType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserAssigns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false),
                        SuperUser = c.Boolean(nullable: false),
                        SAPUser = c.String(nullable: false),
                        SAPPass = c.String(nullable: false),
                        SlpCode = c.Int(nullable: false),
                        StoreId = c.Int(),
                        minDiscount = c.Decimal(precision: 18, scale: 2),
                        CenterCost = c.String(),
                        Active = c.Boolean(nullable: false),
                        PriceListDef = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companys", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.Stores", t => t.StoreId)
                .Index(t => t.StoreId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.PermsByUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserMappId = c.Int(nullable: false),
                        PermId = c.Int(nullable: false),
                        BoolValue = c.Boolean(nullable: false),
                        TextValue = c.String(),
                        IntValue = c.Int(nullable: false),
                        DoubleValue = c.Double(nullable: false),
                        DecimalValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permissions", t => t.PermId, cascadeDelete: true)
                .ForeignKey("dbo.UserAssigns", t => t.UserMappId, cascadeDelete: true)
                .Index(t => t.UserMappId)
                .Index(t => t.PermId);
            
            CreateTable(
                "dbo.SeriesByUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SerieId = c.Int(nullable: false),
                        UsrMappId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Series", t => t.SerieId, cascadeDelete: true)
                .ForeignKey("dbo.UserAssigns", t => t.UsrMappId, cascadeDelete: true)
                .Index(t => t.SerieId)
                .Index(t => t.UsrMappId);
            
            CreateTable(
                "dbo.Series",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DocType = c.Int(nullable: false),
                        Numbering = c.Int(nullable: false),
                        Serie = c.Int(nullable: false),
                        CompanyId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        WhCode = c.String(),
                        WhName = c.String(),
                        CompanyId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companys", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.MailDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        subject = c.String(),
                        from = c.String(),
                        user = c.String(),
                        pass = c.String(),
                        port = c.Int(nullable: false),
                        Host = c.String(),
                        SSL = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ParamsViewCompanies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        ViewsId = c.Int(nullable: false),
                        ParamsId = c.Int(nullable: false),
                        Descrip = c.String(nullable: false),
                        Order = c.Int(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        Display = c.Boolean(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companys", t => t.CompanyId, cascadeDelete: true)
                .ForeignKey("dbo.ViewParams", t => t.ParamsId, cascadeDelete: true)
                .Index(t => t.CompanyId)
                .Index(t => t.ParamsId);
            
            CreateTable(
                "dbo.ViewParams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ParamsViewCompanies", "ParamsId", "dbo.ViewParams");
            DropForeignKey("dbo.ParamsViewCompanies", "CompanyId", "dbo.Companys");
            DropForeignKey("dbo.Companys", "MailDataId", "dbo.MailDatas");
            DropForeignKey("dbo.UserAssigns", "StoreId", "dbo.Stores");
            DropForeignKey("dbo.Stores", "CompanyId", "dbo.Companys");
            DropForeignKey("dbo.SeriesByUsers", "UsrMappId", "dbo.UserAssigns");
            DropForeignKey("dbo.SeriesByUsers", "SerieId", "dbo.Series");
            DropForeignKey("dbo.PermsByUsers", "UserMappId", "dbo.UserAssigns");
            DropForeignKey("dbo.PermsByUsers", "PermId", "dbo.Permissions");
            DropForeignKey("dbo.UserAssigns", "CompanyId", "dbo.Companys");
            DropForeignKey("dbo.Companys", "SAPConnectionId", "dbo.SAPConnections");
            DropForeignKey("dbo.CompanyByUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Permissions", "CompanyByUsers_Id", "dbo.CompanyByUsers");
            DropIndex("dbo.ViewParams", new[] { "Name" });
            DropIndex("dbo.ParamsViewCompanies", new[] { "ParamsId" });
            DropIndex("dbo.ParamsViewCompanies", new[] { "CompanyId" });
            DropIndex("dbo.Stores", new[] { "CompanyId" });
            DropIndex("dbo.SeriesByUsers", new[] { "UsrMappId" });
            DropIndex("dbo.SeriesByUsers", new[] { "SerieId" });
            DropIndex("dbo.PermsByUsers", new[] { "PermId" });
            DropIndex("dbo.PermsByUsers", new[] { "UserMappId" });
            DropIndex("dbo.UserAssigns", new[] { "CompanyId" });
            DropIndex("dbo.UserAssigns", new[] { "StoreId" });
            DropIndex("dbo.Companys", new[] { "MailDataId" });
            DropIndex("dbo.Companys", new[] { "SAPConnectionId" });
            DropIndex("dbo.Companys", new[] { "DBCode" });
            DropIndex("dbo.Permissions", new[] { "CompanyByUsers_Id" });
            DropIndex("dbo.CompanyByUsers", new[] { "UserId" });
            DropTable("dbo.ViewParams");
            DropTable("dbo.ParamsViewCompanies");
            DropTable("dbo.MailDatas");
            DropTable("dbo.Stores");
            DropTable("dbo.Series");
            DropTable("dbo.SeriesByUsers");
            DropTable("dbo.PermsByUsers");
            DropTable("dbo.UserAssigns");
            DropTable("dbo.SAPConnections");
            DropTable("dbo.Companys");
            DropTable("dbo.Users");
            DropTable("dbo.Permissions");
            DropTable("dbo.CompanyByUsers");
        }
    }
}
