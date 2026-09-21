namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedPrecision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("SITI.GRPEN", "CropDiversificationNumberAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("SITI.GRPEN", "EFANumberAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("SITI.GRPEN", "EFANumberAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("SITI.GRPEN", "CropDiversificationNumberAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
