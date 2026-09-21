namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EFAonXB : DbMigration
    {
        public override void Up()
        {
            AddColumn("XB.XBData", "EnglandGreeningEFAReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("XB.XBData", "ScotlandGreeningEFAReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("XB.XBData", "WalesGreeningEFAReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("XB.XBData", "NIGreeningEFAReductionAdditional", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("XB.XBData", "NIGreeningEFAReductionAdditional");
            DropColumn("XB.XBData", "WalesGreeningEFAReductionAdditional");
            DropColumn("XB.XBData", "ScotlandGreeningEFAReductionAdditional");
            DropColumn("XB.XBData", "EnglandGreeningEFAReductionAdditional");
        }
    }
}
