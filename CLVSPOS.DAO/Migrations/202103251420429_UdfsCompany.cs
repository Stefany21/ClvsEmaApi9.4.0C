namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UdfsCompany : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyUdfs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        TableId = c.String(),
                        Udfs = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CompanyUdfs");
        }
    }
}
