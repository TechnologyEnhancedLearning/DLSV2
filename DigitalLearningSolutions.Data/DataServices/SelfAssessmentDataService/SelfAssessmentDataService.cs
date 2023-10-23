namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
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
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed);
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
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed)
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

            selectColumnQuery += ",MAX(casv.Verified) as SignedOff";

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

            orderBy += " OFFSET " + offSet + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";

            var delegateQuery = selectColumnQuery + fromTableQuery + signedOffJoin + whereQuery + groupBy + orderBy;

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
                    removed
                },
                commandTimeout: 3000
            );

            var delegateCountQuery = @$"SELECT  COUNT(*) AS Matches " + fromTableQuery + whereQuery;

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
                    removed
                },
                commandTimeout: 3000
            );
            return (delegateUserCard, ResultCount);
        }
    }
}
