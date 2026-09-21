namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnglandLatePenaltyChargeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("XB.XBData", "EnglandGreeningLateChangePenalty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("XB.XBData", "EnglandGreeningLateChangePenalty");
        }
    }
}
