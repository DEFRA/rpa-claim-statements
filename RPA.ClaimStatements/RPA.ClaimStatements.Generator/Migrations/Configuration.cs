namespace RPA.ClaimStatements.Generator.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RPA.ClaimStatements.Generator.Context.ClaimStatementsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RPA.ClaimStatements.Generator.Context.ClaimStatementsContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { [REDACTED_NAME] },
            //      new Person { [REDACTED_NAME] },
            //      new Person { [REDACTED_NAME] }
            //    );
            //
        }
    }
}
