namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
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
                return connection.QueryFirstOrDefault<Logo>(
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
                   WHERE Centres.CentreID = @centreId AND (Applications.DefaultContentTypeId <> 4 OR Applications.DefaultContentTypeId IS NULL);",
                    new { centreId, customisationId });
            }
            catch (DataException e)
            {
                if (e.InnerException is LogoNotFoundException)
                {
                    return null;
                }
                else throw;
            }
        }
    }
}
