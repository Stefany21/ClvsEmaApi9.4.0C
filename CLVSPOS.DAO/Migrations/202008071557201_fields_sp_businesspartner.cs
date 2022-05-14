namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fields_sp_businesspartner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companys", "SP_GetCustomer", c => c.String());
            AddColumn("dbo.Companys", "SP_GetCustomerbyCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companys", "SP_GetCustomerbyCode");
            DropColumn("dbo.Companys", "SP_GetCustomer");
        }
    }
}
