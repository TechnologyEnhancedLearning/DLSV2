namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.Extensions.Logging;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.Versioning;

    public interface ICentreApplicationsDataService
    {
        CentreApplication? GetCentreApplicationByCentreAndApplicationID(int centreId, int applicationId);
        IEnumerable<CourseForPublish> GetCentralCoursesForPublish(int centreId);
        IEnumerable<CourseForPublish> GetOtherCoursesForPublish(int centreId, string searchTerm);
        IEnumerable<CourseForPublish> GetPathwaysCoursesForPublish(int centreId);
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
                        WHERE (cta.CentreID = @centreId) AND (cta.ApplicationID = @applicationId) AND (cu.Active = 1)
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
                connection.Execute(
                    @"DELETE 
                            FROM   CentreApplications
                            WHERE (CentreID = @centreId) AND (ApplicationID = @applicationId)",
                    new { centreId, applicationId }
                );
                connection.Execute(
                    @"UPDATE Customisations
                        SET Active = 0
                        WHERE (CentreID = @centreId) AND (ApplicationID = @applicationId)",
                    new { centreId, applicationId });
            }

            public void InsertCentreApplication(int centreId, int applicationId)
            {
                connection.Execute(
                        @"INSERT INTO CentreApplications
                             (CentreID, ApplicationID)
                                SELECT @centreId, @applicationId
                                WHERE (NOT EXISTS
                                (SELECT CentreApplicationID
                                FROM    CentreApplications
                                WHERE (CentreID = @centreId) AND (ApplicationID = @applicationId)))",
                        new { centreId, applicationId }
                    );
            }

            public IEnumerable<CourseForPublish> GetCentralCoursesForPublish(int centreId)
            {
                return connection.Query<CourseForPublish>(
                $@"SELECT a.ApplicationID AS Id, a.ApplicationName AS Course, b.BrandName AS Provider
                    FROM   Applications AS a INNER JOIN
                                 Brands AS b ON a.BrandID = b.BrandID INNER JOIN
                                 Centres AS c ON a.CreatedByCentreID = c.CentreID
                    WHERE (a.ASPMenu = 1) AND (a.ArchivedDate IS NULL) AND (a.CoreContent = 1) AND (a.Debug = 0) AND (a.DefaultContentTypeID = 1) AND (a.ApplicationID NOT IN
                                     (SELECT ApplicationID
                                         FROM    CentreApplications
                                     WHERE (CentreID = @centreId)))
                    ORDER BY Course",
                new { centreId }
            );
            }

            public IEnumerable<CourseForPublish> GetOtherCoursesForPublish(int centreId, string searchTerm)
            {
                return connection.Query<CourseForPublish>(
                $@"SELECT TOP(30) a.ApplicationID AS Id, a.ApplicationName AS Course, c.CentreName AS Provider
                    FROM   Applications AS a INNER JOIN
                                 Centres AS c ON a.CreatedByCentreID = c.CentreID
                    WHERE (a.ASPMenu = 1) AND (a.ArchivedDate IS NULL) AND (a.CoreContent = 0) AND (a.Debug = 0) AND (a.DefaultContentTypeID = 1) AND (a.ApplicationID NOT IN
                                     (SELECT ApplicationID
                                     FROM    CentreApplications
                                     WHERE (CentreID = @centreId))) AND (c.CentreName LIKE '%' + @searchTerm + '%') OR
                                 (a.ASPMenu = 1) AND (a.ArchivedDate IS NULL) AND (a.CoreContent = 0) AND (a.Debug = 0) AND (a.DefaultContentTypeID = 1) AND (a.ApplicationName LIKE '%' + @searchTerm + '%')
                    ORDER BY Course",
                new { centreId, searchTerm }
                );
            }

            public IEnumerable<CourseForPublish> GetPathwaysCoursesForPublish(int centreId)
            {
                return connection.Query<CourseForPublish>(
                $@"SELECT a.ApplicationID AS Id, a.ApplicationName AS Course, c.CustomisationName AS Provider
                    FROM   Customisations AS c INNER JOIN
                                 Applications AS a ON c.ApplicationID = a.ApplicationID
                    WHERE (a.ArchivedDate IS NULL) AND (a.ApplicationID NOT IN
                                     (SELECT ApplicationID
                                     FROM    CentreApplications
                                     WHERE (CentreID = @centreId))) AND (c.CustomisationName = 'NHS PATHWAYS CENTRAL') AND (c.Active = 1)
                    ORDER BY Course",
                new { centreId }
                );
            }
        }

    }
}
