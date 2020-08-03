namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface IConfigService
    {
        public string? GetConfigValue(string key);
    }

    public class ConfigService : IConfigService
    {
        public const string MailServer = "MailServer";
        public const string MailFromAddress = "MailFromAddress";
        public const string MailUsername = "MailUsername";
        public const string MailPassword = "MailPW";
        public const string MailPort = "MailPort";
        public const string TrackingSystemBaseUrl = "TrackingSystemBaseURL";

        private readonly IDbConnection connection;

        public ConfigService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public string? GetConfigValue(string key)
        {
            return connection.Query<string>(
                @"SELECT ConfigText FROM Config WHERE ConfigName = @key",
                new { key }
            ).FirstOrDefault();
        }
    }

    public class ConfigValueMissingException : Exception
    {
        public ConfigValueMissingException(string message)
            : base(message)
        {
        }
    }
}
