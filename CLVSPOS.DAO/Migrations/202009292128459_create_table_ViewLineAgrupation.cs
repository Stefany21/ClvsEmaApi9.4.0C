namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_table_ViewLineAgrupation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ViewLineAgrupations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodNum = c.Int(nullable: false),
                        NameView = c.String(),
                        isGroup = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ViewLineAgrupations");
        }
    }
}
