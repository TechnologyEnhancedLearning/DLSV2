namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface IConfigDataService
    {
        string? GetConfigValue(string key);

        bool GetCentreBetaTesting(int centreId);

        string GetConfigValueMissingExceptionMessage(string missingConfigValue);
    }

    public class ConfigDataService : IConfigDataService
    {
        public const string MailServer = "MailServer";
        public const string MailFromAddress = "MailFromAddress";
        public const string MailUsername = "MailUsername";
        public const string MailPassword = "MailPW";
        public const string MailPort = "MailPort";
        public const string TrackingSystemBaseUrl = "TrackingSystemBaseURL";
        public const string AccessibilityHelpText = "AccessibilityNotice";
        public const string TermsText = "TermsAndConditions";
        public const string ContactText = "ContactUsHtml";
        public const string AppBaseUrl = "V2AppBaseUrl";
        public const string MaxSignpostedResources = "MaxSignpostedResources";

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
