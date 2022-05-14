namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfieldLineMode_tableViewLineAgrupations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ViewLineAgrupations", "LineMode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ViewLineAgrupations", "LineMode");
        }
    }
}
