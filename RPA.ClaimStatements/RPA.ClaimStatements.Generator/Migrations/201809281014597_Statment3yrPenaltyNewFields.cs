namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Statment3yrPenaltyNewFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("SITI.GRPEN", "CropDiversificationNumberAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("SITI.GRPEN", "EFANumberAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("SITI.GRPEN", "EFANumberAdditional");
            DropColumn("SITI.GRPEN", "CropDiversificationNumberAdditional");
        }
    }
}
