namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Column_Description_Table_DBObjectNames : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DBObjectNames", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DBObjectNames", "Description");
        }
    }
}
