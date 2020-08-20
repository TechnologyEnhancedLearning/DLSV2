namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ICentresService
    {
        string? GetBannerText(int centreId);
        CentreLogo GetCentreLogo(int centreId);
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

        public CentreLogo GetCentreLogo(int centreId)
        {
            return connection.QueryFirstOrDefault<CentreLogo>(
                @"SELECT CentreLogo as logoData,
                            LogoHeight as height,
                            LogoWidth as width,
                            LogoMimeType as mimeType
                        FROM Centres
                        WHERE CentreID = @centreId",
                new { centreId });
        }
    }
}
