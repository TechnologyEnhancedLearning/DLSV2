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
                    @"SELECT TOP (1) CentreApplications.CentreApplicationID, CentreApplications.CentreID, Centres.CentreName, CentreApplications.ApplicationID, Applications.ApplicationName
                        FROM   CentreApplications INNER JOIN
                             Centres ON CentreApplications.CentreID = Centres.CentreID INNER JOIN
                             Applications ON CentreApplications.ApplicationID = Applications.ApplicationID
                        WHERE (CentreApplications.CentreID = @centreId) AND
                        (CentreApplications.ApplicationID = @applicationId)",
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
