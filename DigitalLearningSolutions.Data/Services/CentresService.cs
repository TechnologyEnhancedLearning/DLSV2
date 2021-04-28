namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using Microsoft.Extensions.Logging;

    public interface ICentresService
    {
        string? GetBannerText(int centreId);
        string? GetCentreName(int centreId);
        IEnumerable<(int, string)> GetActiveCentresAlphabetical();
    }

    public class CentresService : ICentresService
    {
        private readonly ICentresDataService centresDataService;
        private readonly ILogger<CentresService> logger;

        public CentresService(ICentresDataService centresDataService, ILogger<CentresService> logger)
        {
            this.centresDataService = centresDataService;
            this.logger = logger;
        }

        public string? GetBannerText(int centreId)
        {
            return centresDataService.GetBannerText(centreId);
        }

        public string? GetCentreName(int centreId)
        {
            var name = centresDataService.GetCentreName(centreId);
            if (name == null)
            {
                logger.LogWarning(
                    $"No centre found for centre id {centreId}"
                );
            }

            return name;
        }

        public IEnumerable<(int, string)> GetActiveCentresAlphabetical()
        {
            return centresDataService.GetActiveCentresAlphabetical();
        }
    }
}
