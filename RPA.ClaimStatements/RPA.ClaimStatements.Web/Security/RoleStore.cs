using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RPA.ClaimStatements.Web.Security
{
    /// <summary>
    /// Source of the roles granted to a user.
    /// </summary>
    public interface IRoleStore
    {
        IReadOnlyCollection<string> GetRolesForUser(string userName);
    }

    /// <summary>
    /// Reads roles from the Security database (ASP.NET membership schema) through
    /// <c>aspnet_UsersInRoles_GetRolesForUser</c>, which is the call the former IDT role
    /// provider made. Role inheritance is resolved by the database.
    /// </summary>
    public class SecurityDatabaseRoleStore : IRoleStore
    {
        private readonly string connectionString;
        private readonly string applicationName;

        public SecurityDatabaseRoleStore(string connectionString, string applicationName)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
        }

        public IReadOnlyCollection<string> GetRolesForUser(string userName)
        {
            var roles = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("dbo.aspnet_UsersInRoles_GetRolesForUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = applicationName;
                command.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = userName;

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        roles.Add(reader.GetString(0));
                    }
                }
            }

            return roles;
        }
    }
}
