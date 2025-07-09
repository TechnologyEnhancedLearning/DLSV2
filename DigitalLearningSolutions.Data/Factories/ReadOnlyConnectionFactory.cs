namespace DigitalLearningSolutions.Data.Factories
{
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Data;
    public interface IReadOnlyDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
    public class ReadOnlyDbConnectionFactory : IReadOnlyDbConnectionFactory
    {
        private const string ReadOnlyConnectionName = "ReadOnlyConnection";
        private readonly string connectionString;

        public ReadOnlyDbConnectionFactory(IConfiguration config)
        {
            // Ensure the connection string is not null to avoid CS8601
            connectionString = config.GetConnectionString(ReadOnlyConnectionName)
                ?? throw new InvalidOperationException($"Connection string '{ReadOnlyConnectionName}' is not configured.");
        }

        public IDbConnection CreateConnection()
        {
            // Ensure the connection is not enlisted in the trasaction scope to avoid distributed transaction errors:
            return new SqlConnection(connectionString + ";Enlist=false");
        }
    }
}
