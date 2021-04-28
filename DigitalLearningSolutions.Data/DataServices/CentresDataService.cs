namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;

    public interface ICentresDataService
    {
        string? GetBannerText(int centreId);
        string? GetCentreName(int centreId);
        IEnumerable<(int, string)> GetActiveCentresAlphabetical();
    }

    public class CentresDataService: ICentresDataService
    {
        private readonly IDbConnection connection;

        public CentresDataService(IDbConnection connection)
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
            var name = connection.QueryFirstOrDefault<string?>(
                @"SELECT CentreName
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            );

            return name;
        }

        public IEnumerable<(int, string)> GetActiveCentresAlphabetical()
        {
            var centres = connection.Query<(int, string)>(
                @"SELECT CentreID, CentreName
                        FROM Centres
                        WHERE Active = 1
                        ORDER BY CentreName"
            );
            return centres;
        }
    }
}
