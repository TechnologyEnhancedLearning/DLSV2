using DigitalLearningSolutions.Data.DataServices;
using System;

namespace DigitalLearningSolutions.Web.Services
{
    public interface IConfigService
    {
        string? GetConfigValue(string key);
        DateTime GetConfigLastUpdated(string key);

        bool GetCentreBetaTesting(int centreId);

        string GetConfigValueMissingExceptionMessage(string missingConfigValue);
    }
    public class ConfigService : IConfigService
    {
        private readonly IConfigDataService configDataService;
        public ConfigService(IConfigDataService configDataService)
        {
            this.configDataService = configDataService;
        }
        public bool GetCentreBetaTesting(int centreId)
        {
            return configDataService.GetCentreBetaTesting(centreId);
        }

        public DateTime GetConfigLastUpdated(string key)
        {
            return configDataService.GetConfigLastUpdated(key);
        }

        public string? GetConfigValue(string key)
        {
            return configDataService.GetConfigValue(key);
        }

        public string GetConfigValueMissingExceptionMessage(string missingConfigValue)
        {
            return configDataService.GetConfigValueMissingExceptionMessage(missingConfigValue);
        }
    }
}
