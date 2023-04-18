namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Certificates;
    using Microsoft.Extensions.Logging;
    using System.Data;
    public interface ICertificateDataService
    {
        CertificateInformation? GetCertificateDetailsById(int progressId);
        CertificateInformation? GetPreviewCertificateForCentre(int centreId);
    }
    public class CertificateDataService : ICertificateDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CertificateDataService> logger;

        public CertificateDataService(IDbConnection connection, ILogger<CertificateDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
        public CertificateInformation? GetCertificateDetailsById(int progressId)
        {
            var certificateInfo = connection.QueryFirstOrDefault<CertificateInformation>(
                @"SELECT
                  p.ProgressID,
                  u.FirstName AS DelegateFirstName,
	                u.LastName AS DelegateLastName,
	                ce.ContactForename,
                	ce.ContactSurname,
	                ce.CentreName,
	                ce.CentreID,
	                ce.SignatureImage,
	                ce.SignatureWidth,
	                ce.SignatureHeight,
	                ce.CentreLogo,
	                ce.LogoWidth,
	                ce.LogoHeight,
	                ce.LogoMimeType,
	                a.ApplicationName AS CourseName,
	                p.Completed AS CompletionDate,
	                a.AppGroupID,
	                a.CreatedByCentreID
                FROM Candidates AS ca 
			          INNER JOIN Centres AS ce ON ca.CentreID = ce.CentreID 
			          INNER JOIN Progress AS p ON ca.CandidateID = p.CandidateID 
			          INNER JOIN Customisations AS cu ON p.CustomisationID = cu.CustomisationID 
			          INNER JOIN Applications AS a ON cu.ApplicationID = a.ApplicationID
			          INNER JOIN DelegateAccounts AS da ON da.ID = p.CandidateID
                INNER JOIN Users AS u ON u.ID = da.UserID
			          LEFT OUTER JOIN AdminAccounts AS AC ON AC.ID = p.SupervisorAdminId
               WHERE p.ProgressID = @progressId",
                new { progressId }
            );

            if (certificateInfo == null)
            {
                logger.LogWarning($"No certificate information found for progress id {progressId}");
                return null;
            }

            if (certificateInfo.CentreLogo?.Length < 10)
            {
                certificateInfo.CentreLogo = null;
            }

            return certificateInfo;
        }
        public CertificateInformation? GetPreviewCertificateForCentre(int centreId)
        {
            var certificate = connection.QueryFirstOrDefault<CertificateInformation>(
                @"SELECT
                  0 AS ProgressID,
                  N'Joseph' AS DelegateFirstName, 
	                N'Bloggs' AS DelegateLastName, 
	                ContactForename, 
	                ContactSurname, 
	                CentreName, 
	                CentreID,
	                SignatureImage,
	                SignatureWidth, 
	                SignatureHeight, 
	                CentreLogo, 
	                LogoWidth, 
	                LogoHeight, 
	                LogoMimeType,
	                'Course name here' AS CourseName,
	                CONVERT(DATETIME, '2023-01-01 00:00:00', 102) AS CompletionDate,	
                    3 AS AppGroupID, 
	                101 AS CreatedByCentreID
                FROM Centres
                WHERE CentreID = @centreId",
                new { centreId }
            );
            if (certificate == null)
            {
                logger.LogWarning($"No certificate information found for centre id {centreId}");
                return null;
            }
            if (certificate.CentreLogo?.Length < 10)
            {
                certificate.CentreLogo = null;
            }
            return certificate;
        }
    }
}
