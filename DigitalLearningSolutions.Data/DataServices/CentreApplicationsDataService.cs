namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Extensions.Logging;
    using System.Data;

    public interface ICentreApplicationsDataService
    {
        CentreApplication? GetCentreApplicationByCentreAndApplicationID(int centreId, int applicationId);
        void DeleteCentreApplicationByCentreAndApplicationID(int centreId, int applicationId);
        void InsertCentreApplication(int centreId, int applicationId);
        public class CentreApplicationsDataService : ICentreApplicationsDataService
        {
            private readonly IDbConnection connection;
            private readonly ILogger<CentresDataService> logger;
            public CentreApplicationsDataService(IDbConnection connection, ILogger<CentresDataService> logger)
            {
                this.connection = connection;
                this.logger = logger;
            }

            public CentreApplication? GetCentreApplicationByCentreAndApplicationID(int centreId, int applicationId)
            {
                var centreApplication = connection.QueryFirstOrDefault<CentreApplication?>(
                    @"SELECT TOP (1) cta.CentreApplicationID, cta.CentreID, ct.CentreName, cta.ApplicationID, a.ApplicationName, COUNT(cu.CustomisationID) AS CustomisationCount
                        FROM   CentreApplications AS cta INNER JOIN
                                     Centres AS ct ON cta.CentreID = ct.CentreID INNER JOIN
                                     Applications AS a ON cta.ApplicationID = a.ApplicationID LEFT OUTER JOIN
                                     Customisations AS cu ON cta.CentreID = cu.CentreID AND cta.ApplicationID = cu.ApplicationID
                        WHERE (cta.CentreID = @centreId) AND (cta.ApplicationID = @applicationId)
                        GROUP BY cta.CentreApplicationID, cta.CentreID, ct.CentreName, cta.ApplicationID, a.ApplicationName",
                     new { centreId, applicationId }
                    );
                if (centreApplication == null)
                {
                    logger.LogWarning($"No centre application found for centre id {centreId} and application id {applicationId}");
                }
                return centreApplication;
            }

            public void DeleteCentreApplicationByCentreAndApplicationID(int centreId, int applicationId)
            {
                {
                    connection.Execute(
                        @"DELETE 
                            FROM   CentreApplications
                            WHERE (CentreID = @centreId) AND (ApplicationID = @applicationId)",
                        new { centreId, applicationId }
                    );
                }
            }

            public void InsertCentreApplication(int centreId, int applicationId)
            {
                connection.Execute(
                        @"INSERT INTO CentreApplications
                             (CentreID, ApplicationID)
                                SELECT @centreId, @applicationId
                                WHERE (NOT EXISTS
                                (SELECT CentreApplicationID
                                FROM    CentreApplications AS CentreApplications_1
                                WHERE (CentreID = @centreId) AND (ApplicationID = @applicationId)))",
                        new { centreId, applicationId }
                    );
            }
        }

    }
}
