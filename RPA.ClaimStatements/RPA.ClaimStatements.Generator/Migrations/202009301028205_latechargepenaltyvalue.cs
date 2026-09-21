namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class latechargepenaltyvalue : DbMigration
    {
        public override void Up()
        {
            AddColumn("SITI.BPSPEN", "LateChargePenaltyValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("SITI.GRPEN", "LateChargePenaltyValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("SITI.YF", "LateChargePenaltyValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("SITI.YF", "LateChargePenaltyValue");
            DropColumn("SITI.GRPEN", "LateChargePenaltyValue");
            DropColumn("SITI.BPSPEN", "LateChargePenaltyValue");
        }
    }
}
