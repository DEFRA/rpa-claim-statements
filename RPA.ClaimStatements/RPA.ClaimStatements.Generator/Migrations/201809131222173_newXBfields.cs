namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newXBfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("XB.XBData", "EnglandGreeningCropDiversificationReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("XB.XBData", "ScotlandGreeningCropDiversificationReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("XB.XBData", "WalesGreeningCropDiversificationReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("XB.XBData", "NIGreeningCropDiversificationReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("XB.XBData", "NIGreeningCropDiversificationReductionAdditional");
            DropColumn("XB.XBData", "WalesGreeningCropDiversificationReductionAdditional");
            DropColumn("XB.XBData", "ScotlandGreeningCropDiversificationReductionAdditional");
            DropColumn("XB.XBData", "EnglandGreeningCropDiversificationReductionAdditional");
        }
    }
}
