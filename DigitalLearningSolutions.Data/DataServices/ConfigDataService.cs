namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface IConfigDataService
    {
        string? GetConfigValue(string key);
        DateTime GetConfigLastUpdated(string key);

        bool GetCentreBetaTesting(int centreId);

        string GetConfigValueMissingExceptionMessage(string missingConfigValue);
    }

    public class ConfigDataService : IConfigDataService
    {
        private readonly IDbConnection connection;

        public ConfigDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public bool GetCentreBetaTesting(int centreId)
        {
            return connection.Query<bool>(
                @"SELECT BetaTesting FROM Centres WHERE CentreID = @centreId",
                new { centreId }
            ).FirstOrDefault();
        }

        public string? GetConfigValue(string key)
        {
            return connection.Query<string>(
                @"SELECT ConfigText FROM Config WHERE ConfigName = @key",
                new { key }
            ).FirstOrDefault();
        }

        public DateTime GetConfigLastUpdated(string key)
        {
            return connection.Query<DateTime>(
                @"SELECT UpdatedDate FROM Config WHERE ConfigName = @key",
                new { key }
            ).FirstOrDefault();
        }

        public string GetConfigValueMissingExceptionMessage(string missingConfigValue)
        {
            return $"Encountered an error while trying to send an email: The value of {missingConfigValue} is null";
        }
    }

    public class ConfigValueMissingException : Exception
    {
        public ConfigValueMissingException(string message)
            : base(message) { }
    }
}
