namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateNameTableObjects : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Objects", newName: "DBObjectNames");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DBObjectNames", newName: "Objects");
        }
    }
}
