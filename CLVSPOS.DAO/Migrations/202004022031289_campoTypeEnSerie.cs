namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class campoTypeEnSerie : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Series", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Series", "Type");
        }
    }
}
