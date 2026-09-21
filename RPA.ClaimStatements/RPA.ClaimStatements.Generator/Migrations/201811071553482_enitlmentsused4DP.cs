namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class enitlmentsused4DP : DbMigration
    {
        public override void Up()
        {
            AlterColumn("SITI.YF", "EntitlementsUsedToClaim", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("SITI.YF", "EntitlementsUsedToClaim", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
