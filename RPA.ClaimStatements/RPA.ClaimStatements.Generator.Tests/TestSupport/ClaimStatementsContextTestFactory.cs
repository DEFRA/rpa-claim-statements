// Moq builds a mocked ClaimStatementsContext with a real base constructor. Without arguments it would pick
// the production constructor, which reads ConnectionStrings__ClaimStatementsContext and throws when it is
// unset. Passing synthetic options selects the options constructor instead, which does no I/O; the mocked
// contexts never open a connection.
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;

namespace RPA.ClaimStatements.Generator.Tests.TestSupport
{
    internal static class ClaimStatementsContextTestFactory
    {
        /// <summary>
        /// Builds a <see cref="DbContextOptions{TContext}"/> for <see cref="ClaimStatementsContext"/>
        /// against a deliberately unresolvable, obviously-synthetic server name with a 1-second
        /// connect timeout. Constructing a <see cref="DbContextOptions{TContext}"/> and passing it to
        /// a <see cref="DbContext"/> constructor performs zero I/O by itself -- EF Core only touches
        /// the network when a query/SaveChanges/Database.* call actually executes, and no caller of
        /// this factory ever makes such a call against the resulting context/mock.
        /// </summary>
        public static DbContextOptions<ClaimStatementsContext> CreateSyntheticOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ClaimStatementsContext>();
            optionsBuilder.UseSqlServer(
                "Server=intentionally-unresolvable.invalid;Database=ClaimStatementsContext;Integrated Security=true;TrustServerCertificate=true;Connect Timeout=1;");
            return optionsBuilder.Options;
        }
    }
}