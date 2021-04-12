namespace DigitalLearningSolutions.Data.Services
{
    using Dapper;
    using System.Data;
    using Microsoft.Extensions.Logging;

    public interface ICentresService
    {
        string? GetBannerText(int centreId);
        string? GetCentreName(int centreId);
    }

    public class CentresService : ICentresService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CourseService> logger;

        public CentresService(IDbConnection connection, ILogger<CourseService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public string? GetBannerText(int centreId)
        {
            return connection.QueryFirstOrDefault<string?>(
                @"SELECT BannerText
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );
        }

        public string? GetCentreName(int centreId)
        {
            var name = connection.QueryFirstOrDefault<string?>(
                @"SELECT CentreName
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );
            if (name == null)
            {
                logger.LogWarning(
                    $"No centre found for centre id {centreId}"
                );
            }
        }
    }
}
