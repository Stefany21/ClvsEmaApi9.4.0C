namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfieldLineMode_Companys : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "LineMode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "LineMode");
        }
    }
}
