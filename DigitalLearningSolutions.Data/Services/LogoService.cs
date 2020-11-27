namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;

    public interface ILogoService
    {
        Logo GetLogo(int? centreId, int? customisationId);
    }

    public class LogoService : ILogoService
    {
        private readonly IDbConnection connection;

        public LogoService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public Logo GetLogo(int? centreId, int? customisationId)
        {
            using var multi = connection.QueryMultiple(
                @"SELECT Centres.CentreName AS LogoName,
                         Centres.CentreLogo AS LogoData,
                         Centres.LogoMimeType
                    FROM Centres
                   WHERE Centres.CentreID = @centreId;

                  SELECT Brands.BrandName AS LogoName, Brands.BrandLogo AS LogoData
                    FROM Customisations
                         INNER JOIN Applications
                         ON Applications.ApplicationID = Customisations.ApplicationID

                         INNER JOIN Brands
                         ON Applications.BrandID = Brands.BrandID
                   WHERE Customisations.CustomisationID = @customisationId AND CentreID = @centreId;",
                new { centreId, customisationId });

            var centreLogo = multi.Read<Logo>().FirstOrDefault();
            var brandLogo = multi.Read<Logo>().FirstOrDefault();

            if (centreLogo?.LogoUrl != null)
            {
                return centreLogo;
            }
            if (brandLogo?.LogoUrl != null)
            {
                return brandLogo;
            }
            return new Logo("", null);
        }
    }
}
