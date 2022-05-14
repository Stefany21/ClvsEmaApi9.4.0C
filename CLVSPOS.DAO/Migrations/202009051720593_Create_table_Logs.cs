namespace CLVSPOS.DAO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_table_Logs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeDocument = c.String(),
                        Document = c.String(),
                        StartTimeDocument = c.DateTime(nullable: false),
                        EndTimeDocument = c.DateTime(nullable: false),
                        ElapsedTimeCreateDocument = c.String(),
                        StartTimeCompany = c.DateTime(nullable: false),
                        EndTimeCompany = c.DateTime(nullable: false),
                        ElapsedTimeCompany = c.String(),
                        StartTimeSapDocument = c.DateTime(nullable: false),
                        EndTimeSapDocument = c.DateTime(nullable: false),
                        ElapsedTimeSapDocument = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Logs");
        }
    }
}
