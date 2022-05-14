namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_AcceptedMargin_on_Company : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "AcceptedMargins", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "AcceptedMargins");
        }
    }
}
