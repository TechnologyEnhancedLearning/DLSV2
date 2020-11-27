namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ILogoService
    {
        Logo? GetLogo(int? centreId, int? customisationId);
    }

    public class LogoService : ILogoService
    {
        private readonly IDbConnection connection;

        public LogoService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public Logo? GetLogo(int? centreId, int? customisationId)
        {
            try
            {
                var logo = connection.QueryFirstOrDefault<Logo>(
                    @"SELECT Centres.CentreName,
                         Centres.CentreLogo,
                         Centres.LogoMimeType as CentreMimeType,
                         Brands.BrandName,
                         Brands.BrandLogo
                    FROM Centres
                         LEFT JOIN Customisations
                         ON Centres.CentreID = Customisations.CentreID
                            AND Customisations.CustomisationID = @customisationId

                         LEFT JOIN Applications
                         ON Applications.ApplicationID = Customisations.ApplicationID

                         LEFT JOIN Brands
                         ON Applications.BrandID = Brands.BrandID
                   WHERE Centres.CentreID = @centreId;",
                    new { centreId, customisationId });
                return logo;
            }
            catch (LogoNotFoundException)
            {
                return null;
            }
        }
    }
}
