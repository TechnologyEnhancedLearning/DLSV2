namespace DigitalLearningSolutions.Data.Services
{
    using Dapper;
    using System.Data;

    public interface ICentresService
    {
        string? GetBannerText(int centreId);
        string? GetCentreName(int centreId);
    }

    public class CentresService : ICentresService
    {
        private readonly IDbConnection connection;

        public CentresService(IDbConnection connection)
        {
            this.connection = connection;
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
            return connection.QueryFirstOrDefault<string?>(
                @"SELECT CentreName
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );
        }
    }
}
