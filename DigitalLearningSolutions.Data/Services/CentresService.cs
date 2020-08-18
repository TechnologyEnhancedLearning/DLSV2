namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;

    public interface ICentresService
    {
        string? GetBannerText(int centreId);
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
            return connection.Query<string?>(
                @"SELECT BannerText
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId }
            ).FirstOrDefault();
        }
    }
}
