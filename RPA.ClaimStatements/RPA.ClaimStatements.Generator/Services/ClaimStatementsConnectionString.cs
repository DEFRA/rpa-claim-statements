using Microsoft.Data.SqlClient;
using System;

namespace RPA.ClaimStatements.Generator.Services
{
    public static class ClaimStatementsConnectionString
    {
        public const string EnvironmentVariableName = "ConnectionStrings__ClaimStatementsContext";

        public static string Resolve()
        {
            return Resolve(Environment.GetEnvironmentVariable(EnvironmentVariableName));
        }

        public static string Resolve(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(EnvironmentVariableName + " is not set or is blank. Supply the SQL connection string.");
            }

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                // Construction validates conflicting authentication options without opening a connection.
                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                }
            }
            catch (ArgumentException)
            {
                // SqlClient errors can echo sensitive input; deliberately omit the inner exception.
                throw new InvalidOperationException("Connection string 'ClaimStatementsContext' has invalid SQL connection or authentication settings.");
            }

            return connectionString;
        }
    }
}
