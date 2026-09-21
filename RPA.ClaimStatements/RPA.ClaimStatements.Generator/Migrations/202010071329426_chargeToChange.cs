namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chargeToChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("SITI.BPSPEN", "LateChangePenaltyReduction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("SITI.GRPEN", "LateChangePenaltyReduction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("SITI.YF", "LateChangePenaltyReduction", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("SITI.BPSPEN", "LateChargePenaltyValue");
            DropColumn("SITI.GRPEN", "LateChargePenaltyValue");
            DropColumn("SITI.YF", "LateChargePenaltyValue");
        }
        
        public override void Down()
        {
            AddColumn("SITI.YF", "LateChargePenaltyValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("SITI.GRPEN", "LateChargePenaltyValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("SITI.BPSPEN", "LateChargePenaltyValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("SITI.YF", "LateChangePenaltyReduction");
            DropColumn("SITI.GRPEN", "LateChangePenaltyReduction");
            DropColumn("SITI.BPSPEN", "LateChangePenaltyReduction");
        }
    }
}
