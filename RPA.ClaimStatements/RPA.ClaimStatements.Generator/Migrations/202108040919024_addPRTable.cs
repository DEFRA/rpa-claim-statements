namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPRTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "SITI.PR",
                c => new
                    {
                        PRID = c.Guid(nullable: false),
                        ClaimID = c.Guid(nullable: false),
                        PRB1MaxBand = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB1Percent = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB1Result = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB2MaxBand = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB2Percent = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB2Result = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB3MaxBand = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB3Percent = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB3Result = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB4MaxBand = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB4Percent = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRB4Result = c.Decimal(nullable: false, precision: 18, scale: 4),
                        PRBTotalResult = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.PRID);
            
            AddColumn("SITI.Claims", "PRID", c => c.Guid());
            CreateIndex("SITI.Claims", "PRID");
            AddForeignKey("SITI.Claims", "PRID", "SITI.PR", "PRID");
        }
        
        public override void Down()
        {
            DropForeignKey("SITI.Claims", "PRID", "SITI.PR");
            DropIndex("SITI.Claims", new[] { "PRID" });
            DropColumn("SITI.Claims", "PRID");
            DropTable("SITI.PR");
        }
    }
}
