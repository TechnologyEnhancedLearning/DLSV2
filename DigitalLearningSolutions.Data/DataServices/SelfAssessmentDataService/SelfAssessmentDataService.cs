namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;
    using Microsoft.Extensions.Logging;

    public interface ISelfAssessmentDataService
    {
        //Self Assessments
        string? GetSelfAssessmentNameById(int selfAssessmentId);

        // CompetencyDataService
        IEnumerable<int> GetCompetencyIdsForSelfAssessment(int selfAssessmentId);

        Competency? GetNthCompetency(int n, int selfAssessmentId, int delegateUserId); // 1 indexed

        IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int delegateId);

        IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int delegateUserId, int? selfAssessmentResultId = null);

        IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(int candidateAssessmentId, int delegateUserId);

        IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int delegateUserId);
        IEnumerable<Competency> GetResultSupervisorVerifications(int selfAssessmentId, int delegateUserId);

        Competency? GetCompetencyByCandidateAssessmentResultId(int resultId, int candidateAssessmentId, int adminId);

        void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int delegateUserId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        );

        IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int delegateUserId);

        void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int delegateUserId);

        void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int delegateUserId);

        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(
            int assessmentQuestionId,
            int minValue,
            int maxValue,
            bool zeroBased
        );

        List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int delegateUserId);

        CompetencyAssessmentQuestionRoleRequirement? GetCompetencyAssessmentQuestionRoleRequirements(
            int competencyId,
            int selfAssessmentId,
            int assessmentQuestionId,
            int levelValue
        );

        IEnumerable<SelfAssessmentResult> GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
            int delegateUserId,
            int selfAssessmentId,
            int competencyId
        );

        // CandidateAssessmentsDataService

        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int delegateUserId, int centreId);

        CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int delegateUserId, int selfAssessmentId);

        void UpdateLastAccessed(int selfAssessmentId, int delegateUserId);
        void RemoveSignoffRequests(int selfAssessmentId, int delegateUserId, int competencyGroupsId);
        void SetCompleteByDate(int selfAssessmentId, int delegateUserId, DateTime? completeByDate);

        void SetSubmittedDateNow(int selfAssessmentId, int delegateUserId);

        void IncrementLaunchCount(int selfAssessmentId, int delegateUserId);

        void SetUpdatedFlag(int selfAssessmentId, int delegateUserId, bool status);

        void SetBookmark(int selfAssessmentId, int delegateUserId, string bookmark);

        IEnumerable<CandidateAssessment> GetCandidateAssessments(int delegateUserId, int selfAssessmentId);

        // SelfAssessmentSupervisorDataService
        SelfAssessmentSupervisor? GetSupervisorForSelfAssessmentId(int selfAssessmentId, int delegateUserId);

        IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(int selfAssessmentId, int delegateUserId);

        IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        );

        IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        );

        IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        );

        SelfAssessmentSupervisor? GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
            int candidateAssessmentSupervisorId
        );

        void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId);

        void UpdateCandidateAssessmentSupervisorVerificationEmailSent(int candidateAssessmentSupervisorVerificationId);

        SupervisorComment? GetSupervisorComments(int delegateUserId, int resultId);

        IEnumerable<Administrator> GetValidSupervisorsForActivity(int centreId, int selfAssessmentId, int delegateUserId);

        Administrator GetSupervisorByAdminId(int supervisorAdminId);

        IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int delegateUserId
        );

        // FilteredDataService
        Profile? GetFilteredProfileForCandidateById(int delegateUserId, int selfAssessmentId);

        IEnumerable<Goal> GetFilteredGoalsForCandidateId(int delegateUserId, int selfAssessmentId);

        void LogAssetLaunch(int delegateUserId, int selfAssessmentId, LearningAsset learningAsset);

        //Export Candidate Assessment
        CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(
            int candidateAssessmentId,
            int delegateUserId
        );

        List<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId,
            int delegateUserId
        );

        void RemoveEnrolment(int selfAssessmentId, int delegateUserId);
        (IEnumerable<SelfAssessmentDelegate>, int) GetSelfAssessmentDelegates(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, bool? submitted, bool? signedOff);

        IEnumerable<SelfAssessmentDelegate> GetSelfAssessmentActivityDelegatesExport(string searchString, int itemsPerPage, string sortBy, string sortDirection,
           int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, int currentRun);
        int GetSelfAssessmentActivityDelegatesExportCount(string searchString,  string sortBy, string sortDirection,
          int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed);
        string GetSelfAssessmentActivityDelegatesSupervisor(int selfAssessmentId, int delegateUserId);

        RemoveSelfAssessmentDelegate GetDelegateSelfAssessmentByCandidateAssessmentsId(int candidateAssessmentsId);
       void RemoveDelegateSelfAssessment(int candidateAssessmentsId);
        int? GetSupervisorsCountFromCandidateAssessmentId(int candidateAssessmentsId);
        bool CheckForSameCentre(int centreId, int candidateAssessmentsId);
    }

    public partial class SelfAssessmentDataService : ISelfAssessmentDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SelfAssessmentDataService> logger;

        public SelfAssessmentDataService(IDbConnection connection, ILogger<SelfAssessmentDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public string? GetSelfAssessmentNameById(int selfAssessmentId)
        {
            var name = connection.QueryFirstOrDefault<string?>(
                @"SELECT [Name]
                        FROM SelfAssessments
                        WHERE ID = @selfAssessmentId"
            ,
                new { selfAssessmentId }
            );
            return name;
        }

        public (IEnumerable<SelfAssessmentDelegate>, int) GetSelfAssessmentDelegates(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, bool? submitted, bool? signedOff)
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var cteAssessmentResults = $@"
                WITH LatestAssessmentResults AS
					(
						SELECT	s.DelegateUserID
								, CASE WHEN COALESCE (rr.LevelRAG, 0) = 3 THEN s.ID ELSE NULL END AS SelfAssessed
								, CASE WHEN sv.Verified IS NOT NULL AND sv.SignedOff = 1 AND COALESCE (rr.LevelRAG, 0) = 3 THEN s.ID ELSE NULL END AS Confirmed
								, CASE WHEN sas.Optional = 1  THEN s.CompetencyID ELSE NULL END AS Optional
						FROM   SelfAssessmentResults AS s LEFT OUTER JOIN
										SelfAssessmentStructure AS sas ON s.SelfAssessmentID = sas.SelfAssessmentID AND s.CompetencyID = sas.CompetencyID LEFT OUTER JOIN
										SelfAssessmentResultSupervisorVerifications AS sv ON s.ID = sv.SelfAssessmentResultId AND sv.Superceded = 0 LEFT OUTER JOIN
										CompetencyAssessmentQuestionRoleRequirements AS rr ON s.CompetencyID = rr.CompetencyID AND s.AssessmentQuestionID = rr.AssessmentQuestionID AND s.SelfAssessmentID = rr.SelfAssessmentID AND s.Result = rr.LevelValue
						WHERE (s.SelfAssessmentID = @selfAssessmentId)
					)";

            var selectColumnQuery = $@"
                SELECT da.CandidateNumber,
                u.ID AS DelegateUserId,
                u.ProfessionalRegistrationNumber,
                ca.Id AS CandidateAssessmentsId,
                ca.SelfAssessmentID As SelfAssessmentId,
                ca.StartedDate,
                ca.EnrolmentMethodId,
                ca.LastAccessed,
                ca.LaunchCount,
                ca.CompleteByDate AS CompleteBy,
                ca.SubmittedDate,
                ca.RemovedDate,
                ca.CompletedDate,
				uEnrolledBy.FirstName AS EnrolledByForename,
                uEnrolledBy.LastName AS EnrolledBySurname,
                aaEnrolledBy.Active AS EnrolledByAdminActive,
				da.CandidateNumber AS CandidateNumber,
                u.FirstName AS DelegateFirstName,
                u.LastName AS DelegateLastName,
                COALESCE(ucd.Email, u.PrimaryEmail) AS DelegateEmail,
                da.Active AS IsDelegateActive,
                sa.Name AS Name,
                COALESCE(COUNT(DISTINCT LAR.SelfAssessed), NULL) AS SelfAssessed,
                COALESCE(COUNT(DISTINCT LAR.Confirmed), NULL) AS Confirmed,
                MAX(casv.Verified) as SignedOff";

            var fromTableQuery = $@" FROM  dbo.SelfAssessments AS sa 
				INNER JOIN dbo.CandidateAssessments AS ca WITH (NOLOCK) ON sa.ID = ca.SelfAssessmentID 
				INNER JOIN dbo.CentreSelfAssessments AS csa  WITH (NOLOCK) ON sa.ID = csa.SelfAssessmentID 
                INNER JOIN dbo.DelegateAccounts da WITH (NOLOCK) ON ca.CentreID = da.CentreID AND ca.DelegateUserID = da.UserID AND da.CentreID = csa.CentreID
                INNER JOIN dbo.Users u WITH (NOLOCK) ON DA.UserID = u.ID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID
				LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy WITH (NOLOCK) ON aaEnrolledBy.ID = ca.EnrolledByAdminID
                LEFT OUTER JOIN Users AS uEnrolledBy WITH (NOLOCK) ON uEnrolledBy.ID = aaEnrolledBy.UserID
                LEFT JOIN dbo.CandidateAssessmentSupervisors AS cas WITH (NOLOCK) ON ca.ID = cas.CandidateAssessmentID
                LEFT JOIN dbo.CandidateAssessmentSupervisorVerifications AS casv WITH (NOLOCK) ON cas.ID = casv.CandidateAssessmentSupervisorID AND
                (casv.Verified IS NOT NULL AND casv.SignedOff = 1)
                LEFT OUTER JOIN LatestAssessmentResults AS LAR ON LAR.DelegateUserID = ca.DelegateUserID

                WHERE sa.ID = @selfAssessmentId 
                AND da.CentreID = @centreID AND csa.CentreID = @centreID
                AND (ca.RemovedDate IS NULL)
                AND ( u.FirstName + ' ' + u.LastName + ' ' + COALESCE(ucd.Email, u.PrimaryEmail) + ' ' + COALESCE(da.CandidateNumber, '') LIKE N'%' + @searchString + N'%')
                AND ((@isDelegateActive IS NULL) OR (@isDelegateActive = 1 AND (da.Active = 1)) OR (@isDelegateActive = 0 AND (da.Active = 0)))
				AND ((@removed IS NULL) OR (@removed = 1 AND (ca.RemovedDate IS NOT NULL)) OR (@removed = 0 AND (ca.RemovedDate IS NULL)))
                AND ((@submitted IS NULL) OR (@submitted = 1 AND (ca.SubmittedDate IS NOT NULL)) OR (@submitted = 0 AND (ca.SubmittedDate IS NULL)))
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%.__%' ";

            var groupBy = $@" GROUP BY 
				da.CandidateNumber,
                u.ID,
                u.ProfessionalRegistrationNumber,
                ca.SelfAssessmentID,
                ca.StartedDate,
                ca.EnrolmentMethodId,
                ca.LastAccessed,
                ca.LaunchCount,
                ca.CompleteByDate,
                ca.SubmittedDate,
                ca.RemovedDate,
                ca.CompletedDate,
				uEnrolledBy.FirstName,
                uEnrolledBy.LastName,
                aaEnrolledBy.Active,
				da.CandidateNumber,
                u.FirstName,
                u.LastName,
                COALESCE(ucd.Email, u.PrimaryEmail),
                da.Active,
                sa.Name,
                ca.Id";

            if (signedOff != null)
            {
                groupBy += (bool)signedOff ? " HAVING MAX(casv.Verified) IS NOT NULL " : " HAVING MAX(casv.Verified) IS NULL ";
            }

            string orderBy;
            sortDirection = sortDirection == "Ascending" ? "ASC" : "DESC";
            string sortOrder = sortDirection + ", LTRIM(u.LastName)" + sortDirection;

            if (sortBy == "LastAccessed")
                orderBy = " ORDER BY ca.LastAccessed " + sortOrder;
            else if (sortBy == "StartedDate")
                orderBy = " ORDER BY ca.StartedDate " + sortOrder;
            else if (sortBy == "SignedOff")
                orderBy = " ORDER BY SignedOff " + sortOrder;
            else if (sortBy == "SubmittedDate")
                orderBy = " ORDER BY ca.SubmittedDate " + sortOrder;
            else if (sortBy == "SelfAssessed")
                orderBy = " ORDER BY SelfAssessed " + sortOrder;
            else if (sortBy == "Confirmed")
                orderBy = " ORDER BY Confirmed " + sortOrder;
            else
                orderBy = " ORDER BY LTRIM(u.LastName) " + sortDirection + ", LTRIM(u.FirstName) ";

            orderBy += " OFFSET " + offSet + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";

            var delegateQuery = cteAssessmentResults + selectColumnQuery + fromTableQuery + groupBy + orderBy;

            IEnumerable<SelfAssessmentDelegate> delegateUserCard = connection.Query<SelfAssessmentDelegate>(
                delegateQuery,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    selfAssessmentId,
                    centreId,
                    isDelegateActive,
                    removed,
                    submitted,
                    signedOff
                },
                commandTimeout: 3000
            );

            var delegateCountQuery = cteAssessmentResults + @$"SELECT COUNT(Matches) from(
                                    SELECT  COUNT(*) AS Matches " + fromTableQuery + groupBy + ") AS ct";

            int ResultCount = connection.ExecuteScalar<int>(
                delegateCountQuery,
                new
                {
                    searchString,
                    offSet,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    selfAssessmentId,
                    centreId,
                    isDelegateActive,
                    removed,
                    submitted,
                    signedOff
                },
                commandTimeout: 3000
            );
            return (delegateUserCard, ResultCount);
        }
public IEnumerable<SelfAssessmentDelegate> GetSelfAssessmentActivityDelegatesExport(string searchString, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, int currentRun)
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var selectColumnQuery = $@"SELECT
                da.CandidateNumber,
                u.ID AS DelegateUserId,
                u.ProfessionalRegistrationNumber,
                ca.SelfAssessmentID As SelfAssessmentId,
                ca.StartedDate,
                ca.EnrolmentMethodId,
                ca.LastAccessed,
                ca.LaunchCount,
                ca.CompleteByDate,
                ca.SubmittedDate,
                ca.RemovedDate,
                ca.CompletedDate,
				uEnrolledBy.FirstName AS EnrolledByForename,
                uEnrolledBy.LastName AS EnrolledBySurname,
                aaEnrolledBy.Active AS EnrolledByAdminActive,
				da.CandidateNumber AS CandidateNumber,
                u.FirstName AS DelegateFirstName,
                u.LastName AS DelegateLastName,
                COALESCE(ucd.Email, u.PrimaryEmail) AS DelegateEmail,
                da.Active AS IsDelegateActive ";

            selectColumnQuery += ",MAX(casv.Verified) as SignedOff,da.Answer1 AS RegistrationAnswer1,da.Answer2 AS RegistrationAnswer2,da.Answer3 AS RegistrationAnswer3,da.Answer4 AS RegistrationAnswer4,da.Answer5 AS RegistrationAnswer5,da.Answer6 AS RegistrationAnswer6";

            var fromTableQuery = $@" FROM  dbo.SelfAssessments AS sa 
				INNER JOIN dbo.CandidateAssessments AS ca WITH (NOLOCK) ON sa.ID = ca.SelfAssessmentID 
				INNER JOIN dbo.CentreSelfAssessments AS csa  WITH (NOLOCK) ON sa.ID = csa.SelfAssessmentID 
                INNER JOIN dbo.DelegateAccounts da WITH (NOLOCK) ON ca.CentreID = da.CentreID AND ca.DelegateUserID = da.UserID AND da.CentreID = csa.CentreID
                INNER JOIN dbo.Users u WITH (NOLOCK) ON DA.UserID = u.ID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID
				LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy WITH (NOLOCK) ON aaEnrolledBy.ID = ca.EnrolledByAdminID
                LEFT OUTER JOIN Users AS uEnrolledBy WITH (NOLOCK) ON uEnrolledBy.ID = aaEnrolledBy.UserID ";

            var signedOffJoin = $@" LEFT JOIN dbo.CandidateAssessmentSupervisors AS cas WITH (NOLOCK) ON ca.ID = cas.CandidateAssessmentID
                LEFT JOIN dbo.CandidateAssessmentSupervisorVerifications AS casv WITH (NOLOCK) ON cas.ID = casv.CandidateAssessmentSupervisorID AND(casv.Verified IS NOT NULL AND casv.SignedOff = 1) ";

            var whereQuery = $@" WHERE sa.ID = @selfAssessmentId 
                AND da.CentreID = @centreID AND csa.CentreID = @centreID
                AND (ca.RemovedDate IS NULL)
                AND ( u.FirstName + ' ' + u.LastName + ' ' + COALESCE(ucd.Email, u.PrimaryEmail) + ' ' + COALESCE(da.CandidateNumber, '') LIKE N'%' + @searchString + N'%')
                AND ((@isDelegateActive IS NULL) OR (@isDelegateActive = 1 AND (da.Active = 1)) OR (@isDelegateActive = 0 AND (da.Active = 0)))
				AND ((@removed IS NULL) OR (@removed = 1 AND (ca.RemovedDate IS NOT NULL)) OR (@removed = 0 AND (ca.RemovedDate IS NULL)))
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%.__%' ";

            var groupBy = $@" GROUP BY 
				da.CandidateNumber,
                u.ID,
                u.ProfessionalRegistrationNumber,
                ca.SelfAssessmentID,
                ca.StartedDate,
                ca.EnrolmentMethodId,
                ca.LastAccessed,
                ca.LaunchCount,
                ca.CompleteByDate,
                ca.SubmittedDate,
                ca.RemovedDate,
                ca.CompletedDate,
				uEnrolledBy.FirstName,
                uEnrolledBy.LastName,
                aaEnrolledBy.Active,
				da.CandidateNumber,
                u.FirstName,
                u.LastName,
                COALESCE(ucd.Email, u.PrimaryEmail),
                da.Active,
				da.Answer1,			
                da.Answer2,
                da.Answer3,
                da.Answer4,
                da.Answer5,
                da.Answer6";

            string orderBy;
            string sortOrder = sortDirection == "Ascending" ? "ASC" : "DESC";

            if (sortBy == "Enrolled")
                orderBy = " ORDER BY ca.StartedDate " + sortOrder + ", LTRIM(u.LastName)";
            else if (sortBy == "CompleteBy")
                orderBy = " ORDER BY ca.CompleteByDate " + sortOrder + ", LTRIM(u.LastName)";
            else if (sortBy == "Completed")
                orderBy = " ORDER BY ca.CompletedDate " + sortOrder + ", LTRIM(u.LastName)";
            else if (sortBy == "CandidateNumber")
                orderBy = " ORDER BY da.CandidateNumber " + sortOrder + ", LTRIM(u.LastName)";
            else
                orderBy = " ORDER BY LTRIM(u.LastName) " + sortOrder + ", LTRIM(u.FirstName) ";

            orderBy += " OFFSET " + itemsPerPage * (currentRun - 1) + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";

            var delegateQuery = selectColumnQuery + fromTableQuery + signedOffJoin + whereQuery + groupBy + orderBy;

            IEnumerable<SelfAssessmentDelegate> delegateUserCard = connection.Query<SelfAssessmentDelegate>(
                delegateQuery,
                new
                {
                    searchString,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    selfAssessmentId,
                    centreId,
                    isDelegateActive,
                    removed,
                    currentRun
                },
                commandTimeout: 3000
            );


            return delegateUserCard;
        }
        public int GetSelfAssessmentActivityDelegatesExportCount(string searchString,  string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed)
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();

            var fromTableQuery = $@" FROM  dbo.SelfAssessments AS sa 
				INNER JOIN dbo.CandidateAssessments AS ca WITH (NOLOCK) ON sa.ID = ca.SelfAssessmentID 
				INNER JOIN dbo.CentreSelfAssessments AS csa  WITH (NOLOCK) ON sa.ID = csa.SelfAssessmentID 
                INNER JOIN dbo.DelegateAccounts da WITH (NOLOCK) ON ca.CentreID = da.CentreID AND ca.DelegateUserID = da.UserID AND da.CentreID = csa.CentreID
                INNER JOIN dbo.Users u WITH (NOLOCK) ON DA.UserID = u.ID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID
				LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy WITH (NOLOCK) ON aaEnrolledBy.ID = ca.EnrolledByAdminID
                LEFT OUTER JOIN Users AS uEnrolledBy WITH (NOLOCK) ON uEnrolledBy.ID = aaEnrolledBy.UserID ";

            var signedOffJoin = $@" LEFT JOIN dbo.CandidateAssessmentSupervisors AS cas WITH (NOLOCK) ON ca.ID = cas.CandidateAssessmentID
                LEFT JOIN dbo.CandidateAssessmentSupervisorVerifications AS casv WITH (NOLOCK) ON cas.ID = casv.CandidateAssessmentSupervisorID AND(casv.Verified IS NOT NULL AND casv.SignedOff = 1) ";

            var whereQuery = $@" WHERE sa.ID = @selfAssessmentId 
                AND da.CentreID = @centreID AND csa.CentreID = @centreID
                AND (ca.RemovedDate IS NULL)
                AND ( u.FirstName + ' ' + u.LastName + ' ' + COALESCE(ucd.Email, u.PrimaryEmail) + ' ' + COALESCE(da.CandidateNumber, '') LIKE N'%' + @searchString + N'%')
                AND ((@isDelegateActive IS NULL) OR (@isDelegateActive = 1 AND (da.Active = 1)) OR (@isDelegateActive = 0 AND (da.Active = 0)))
				AND ((@removed IS NULL) OR (@removed = 1 AND (ca.RemovedDate IS NOT NULL)) OR (@removed = 0 AND (ca.RemovedDate IS NULL)))
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%.__%' ";

            var groupBy = $@" GROUP BY 
				da.CandidateNumber,
                u.ID,
                u.ProfessionalRegistrationNumber,
                ca.SelfAssessmentID,
                ca.StartedDate,
                ca.EnrolmentMethodId,
                ca.LastAccessed,
                ca.LaunchCount,
                ca.CompleteByDate,
                ca.SubmittedDate,
                ca.RemovedDate,
                ca.CompletedDate,
				uEnrolledBy.FirstName,
                uEnrolledBy.LastName,
                aaEnrolledBy.Active,
				da.CandidateNumber,
                u.FirstName,
                u.LastName,
                COALESCE(ucd.Email, u.PrimaryEmail),
                da.Active";


            var delegateCountQuery = @$"SELECT  COUNT(*) AS Matches " + fromTableQuery + whereQuery;

            int ResultCount = connection.ExecuteScalar<int>(
                delegateCountQuery,
                new
                {
                    searchString,
                    sortBy,
                    sortDirection,
                    selfAssessmentId,
                    centreId,
                    isDelegateActive,
                    removed
                },
                commandTimeout: 3000
            );
            return ResultCount;
        }
        public string GetSelfAssessmentActivityDelegatesSupervisor(int selfAssessmentId, int delegateUserId)
        {
            return connection.Query<string>(
                @$"SELECT
                        au.Forename + ' ' + au.Surname AS SupervisorName
                    FROM CandidateAssessmentSupervisorVerifications AS casv
                    INNER JOIN CandidateAssessmentSupervisors AS cas
                        ON casv.CandidateAssessmentSupervisorID = cas.ID
                    INNER JOIN CandidateAssessments AS ca
                        ON cas.CandidateAssessmentID = ca.ID
                    INNER JOIN SupervisorDelegates AS sd
                        ON cas.SupervisorDelegateId = sd.ID
                    INNER JOIN AdminUsers AS au
                        ON sd.SupervisorAdminID = au.AdminID
                    LEFT OUTER JOIN SelfAssessmentSupervisorRoles AS sasr
                        ON cas.SelfAssessmentSupervisorRoleID = sasr.ID
                    WHERE ((ca.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview = 1)
                        OR (ca.DelegateUserID = @delegateUserId) AND (ca.SelfAssessmentID = @selfAssessmentId) AND (sasr.SelfAssessmentReview IS NULL))
						AND casv.SignedOff=1
                    ORDER BY casv.Requested DESC",
                new { delegateUserId, selfAssessmentId }
            ).FirstOrDefault();
        }
        public RemoveSelfAssessmentDelegate GetDelegateSelfAssessmentByCandidateAssessmentsId(int candidateAssessmentsId)
        {
            return connection.QueryFirstOrDefault<RemoveSelfAssessmentDelegate>(
                 @"Select
                    ca.Id AS CandidateAssessmentsId,
                    ca.SelfAssessmentID,
                    u.FirstName, 
                    u.LastName, 
                    COALESCE(ucd.Email, u.PrimaryEmail) AS Email,
                    sa.Name AS SelfAssessmentsName
                  FROM  dbo.SelfAssessments AS sa 
				INNER JOIN dbo.CandidateAssessments AS ca WITH (NOLOCK) ON sa.ID = ca.SelfAssessmentID 
				INNER JOIN dbo.CentreSelfAssessments AS csa  WITH (NOLOCK) ON sa.ID = csa.SelfAssessmentID 
                INNER JOIN dbo.DelegateAccounts da WITH (NOLOCK) ON ca.CentreID = da.CentreID AND ca.DelegateUserID = da.UserID AND da.CentreID = csa.CentreID
                INNER JOIN dbo.Users u WITH (NOLOCK) ON DA.UserID = u.ID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID
				WHERE  (ca.id =@candidateAssessmentsId)",
                 new { candidateAssessmentsId }
             );
        }
        public void RemoveDelegateSelfAssessment(int candidateAssessmentsId)
        {
            connection.Execute(
                @"UPDATE CandidateAssessments SET RemovedDate = GETUTCDATE(), RemovalMethodID =2
                      WHERE ID = @candidateAssessmentsId",
                new { candidateAssessmentsId }
            );
        }
        public int? GetSupervisorsCountFromCandidateAssessmentId(int candidateAssessmentsId)
        {
            int ResultCount = connection.ExecuteScalar<int>(
                @"SELECT COUNT(ID)
                    FROM CandidateAssessmentSupervisors
                    WHERE CandidateAssessmentID = @candidateAssessmentsId and Removed IS NULL",
                new { candidateAssessmentsId }
            );
            return ResultCount;
        }
        public bool CheckForSameCentre(int centreId, int candidateAssessmentsId)
        {
            int ResultCount = connection.ExecuteScalar<int>(
                @"SELECT Count(DISTINCT ID) FROM CandidateAssessments WHERE ID = @candidateAssessmentsId
                    and CentreID=@centreId",
                new { centreId, candidateAssessmentsId }
            );
            return ResultCount == 1 ? true : false;
        }
    }
}
