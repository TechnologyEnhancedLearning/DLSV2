﻿namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
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

        IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int delegateId);
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

        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int delegateUserId, int centreId, int? adminIdCategoryID);
        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int delegateUserId, int centreId);
        CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int delegateUserId, int selfAssessmentId);

        void UpdateLastAccessed(int selfAssessmentId, int delegateUserId);
        void RemoveSignoffRequests(int selfAssessmentId, int delegateUserId, int competencyGroupsId);
        void RemoveSignoffRequestById(int candidateAssessmentSupervisorVerificationsId);
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

        IEnumerable<Administrator> GetValidSupervisorsForActivity(int selfAssessmentId, int delegateUserId);

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

        IEnumerable<SelfAssessmentDelegate> GetDelegatesOnSelfAssessmentForExport(int? selfAssessmentId, int centreId);

        IEnumerable<SelfAssessmentDelegate> GetSelfAssessmentActivityDelegatesExport(string searchString, int itemsPerPage, string sortBy, string sortDirection,
           int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, int currentRun, bool? submitted, bool? signedOff);
        int GetSelfAssessmentActivityDelegatesExportCount(string searchString, string sortBy, string sortDirection,
          int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, bool? submitted, bool? signedOff);
        string? GetSelfAssessmentActivityDelegatesSupervisor(int selfAssessmentId, int delegateUserId);

        RemoveSelfAssessmentDelegate? GetDelegateSelfAssessmentByCandidateAssessmentsId(int candidateAssessmentsId);
        void RemoveDelegateSelfAssessment(int candidateAssessmentsId);
        int? GetSupervisorsCountFromCandidateAssessmentId(int candidateAssessmentsId);
        bool CheckForSameCentre(int centreId, int candidateAssessmentsId);
        int? GetDelegateAccountId(int centreId, int delegateUserId);
        int CheckDelegateSelfAssessment(int candidateAssessmentsId);
        IEnumerable<CompetencyCountSelfAssessmentCertificate> GetCompetencyCountSelfAssessmentCertificate(int candidateAssessmentID);
        CompetencySelfAssessmentCertificate? GetCompetencySelfAssessmentCertificate(int candidateAssessmentID);
        IEnumerable<Accessor> GetAccessor(int selfAssessmentId, int delegateUserID);
        ActivitySummaryCompetencySelfAssesment? GetActivitySummaryCompetencySelfAssesment(int CandidateAssessmentSupervisorVerificationsId);
        bool IsUnsupervisedSelfAssessment(int selfAssessmentId);
        bool IsCentreSelfAssessment(int selfAssessmentId, int centreId);
        bool HasMinimumOptionalCompetencies(int selfAssessmentId, int delegateUserId);
        int GetSelfAssessmentCategoryId(int selfAssessmentId);
        void RemoveReviewCandidateAssessmentOptionalCompetencies(int id);
        public IEnumerable<SelfAssessmentResult> GetSelfAssessmentResultswithSupervisorVerificationsForDelegateSelfAssessmentCompetency(
        int delegateUserId,
        int selfAssessmentId,
        int competencyId
    );
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
                sa.Name AS [Name],
                MAX(casv.Verified) as SignedOff,
				sa.SupervisorSelfAssessmentReview,
				sa.SupervisorResultsReview";

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

                WHERE sa.ID = @selfAssessmentId 
                AND da.CentreID = @centreID AND csa.CentreID = @centreID
                AND (ca.RemovedDate IS NULL)
                AND ( u.FirstName + ' ' + u.LastName + ' ' + COALESCE(ucd.Email, u.PrimaryEmail) + ' ' + COALESCE(da.CandidateNumber, '') LIKE N'%' + @searchString + N'%')
                AND ((@isDelegateActive IS NULL) OR (@isDelegateActive = 1 AND (da.Active = 1)) OR (@isDelegateActive = 0 AND (da.Active = 0)))
				AND ((@removed IS NULL) OR (@removed = 1 AND (ca.RemovedDate IS NOT NULL)) OR (@removed = 0 AND (ca.RemovedDate IS NULL)))
                AND ((@submitted IS NULL) OR (@submitted = 1 AND (ca.SubmittedDate IS NOT NULL)) OR (@submitted = 0 AND (ca.SubmittedDate IS NULL)))
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%' ";

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
                ca.Id,
				sa.SupervisorSelfAssessmentReview,
				sa.SupervisorResultsReview";

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
            else
                orderBy = " ORDER BY LTRIM(u.LastName) " + sortDirection + ", LTRIM(u.FirstName) ";

            orderBy += " OFFSET " + offSet + " ROWS FETCH NEXT " + itemsPerPage + " ROWS ONLY ";

            var delegateQuery = selectColumnQuery + fromTableQuery + groupBy + orderBy;

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

            var delegateCountQuery = @$"SELECT COUNT(Matches) from(
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

        public IEnumerable<SelfAssessmentDelegate> GetDelegatesOnSelfAssessmentForExport(int? selfAssessmentId, int centreId)
        {
            var selectColumnQuery = $@"SELECT
                da.CandidateNumber,
                u.ID AS DelegateUserId,
                u.ProfessionalRegistrationNumber,
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
                MAX(casv.Verified) as SignedOff,
                da.Answer1 AS RegistrationAnswer1,da.Answer2 AS RegistrationAnswer2,da.Answer3 AS RegistrationAnswer3,da.Answer4 AS RegistrationAnswer4,da.Answer5 AS RegistrationAnswer5,da.Answer6 AS RegistrationAnswer6";

            var fromTableQuery = $@" FROM  dbo.SelfAssessments AS sa 
				INNER JOIN dbo.CandidateAssessments AS ca WITH (NOLOCK) ON sa.ID = ca.SelfAssessmentID 
				INNER JOIN dbo.CentreSelfAssessments AS csa  WITH (NOLOCK) ON sa.ID = csa.SelfAssessmentID 
                INNER JOIN dbo.DelegateAccounts da WITH (NOLOCK) ON ca.CentreID = da.CentreID AND ca.DelegateUserID = da.UserID AND da.CentreID = csa.CentreID
                INNER JOIN dbo.Users u WITH (NOLOCK) ON DA.UserID = u.ID
                LEFT JOIN UserCentreDetails AS ucd WITH (NOLOCK) ON ucd.UserID = da.UserID AND ucd.centreID = da.CentreID
				LEFT OUTER JOIN AdminAccounts AS aaEnrolledBy WITH (NOLOCK) ON aaEnrolledBy.ID = ca.EnrolledByAdminID
                LEFT OUTER JOIN Users AS uEnrolledBy WITH (NOLOCK) ON uEnrolledBy.ID = aaEnrolledBy.UserID 
                LEFT JOIN dbo.CandidateAssessmentSupervisors AS cas WITH (NOLOCK) ON ca.ID = cas.CandidateAssessmentID
                LEFT JOIN dbo.CandidateAssessmentSupervisorVerifications AS casv WITH (NOLOCK) ON cas.ID = casv.CandidateAssessmentSupervisorID AND(casv.Verified IS NOT NULL AND casv.SignedOff = 1) ";

            var whereQuery = $@" WHERE sa.ID = @selfAssessmentId 
                AND da.CentreID = @centreID AND csa.CentreID = @centreID
                AND (ca.RemovedDate IS NULL)
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%' ";

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
                da.Answer6
                ORDER BY LTRIM(u.LastName), LTRIM(u.FirstName)";


            var delegateQuery = selectColumnQuery + fromTableQuery + whereQuery + groupBy;

            IEnumerable<SelfAssessmentDelegate> delegates = connection.Query<SelfAssessmentDelegate>(
                delegateQuery,
                new { selfAssessmentId, centreId },
                commandTimeout: 3000
            );

            return delegates;
        }
        public IEnumerable<SelfAssessmentDelegate> GetSelfAssessmentActivityDelegatesExport(string searchString, int itemsPerPage, string sortBy, string sortDirection,
                    int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, int currentRun, bool? submitted, bool? signedOff)
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            var selectColumnQuery = $@"SELECT
                da.CandidateNumber,
                ca.Id AS CandidateAssessmentsId,
                u.ID AS DelegateUserId,
                u.ProfessionalRegistrationNumber,
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
                AND ((@submitted IS NULL) OR (@submitted = 1 AND (ca.SubmittedDate IS NOT NULL)) OR (@submitted = 0 AND (ca.SubmittedDate IS NULL)))
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%' ";

            var groupBy = $@" GROUP BY 
				da.CandidateNumber,
                ca.Id,
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

            if (signedOff != null)
            {
                groupBy += (bool)signedOff ? " HAVING MAX(casv.Verified) IS NOT NULL " : " HAVING MAX(casv.Verified) IS NULL ";
            }

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
            else if (sortBy == "SubmittedDate")
                orderBy = " ORDER BY ca.SubmittedDate " + sortOrder;
            else if (sortBy == "SignedOff")
                orderBy = " ORDER BY SignedOff " + sortOrder;
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
                    currentRun,
                    submitted,
                    signedOff
                },
                commandTimeout: 3000
            );


            return delegateUserCard;
        }
        public int GetSelfAssessmentActivityDelegatesExportCount(string searchString, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, bool? submitted, bool? signedOff)
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
                AND ((@submitted IS NULL) OR (@submitted = 1 AND (ca.SubmittedDate IS NOT NULL)) OR (@submitted = 0 AND (ca.SubmittedDate IS NULL)))
                AND COALESCE(ucd.Email, u.PrimaryEmail) LIKE '%_@_%' ";

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

            if (signedOff != null)
            {
                groupBy += (bool)signedOff ? " HAVING MAX(casv.Verified) IS NOT NULL " : " HAVING MAX(casv.Verified) IS NULL ";
            }

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
                    removed,
                    submitted,
                    signedOff
                },
                commandTimeout: 3000
            );
            return ResultCount;
        }
        public string? GetSelfAssessmentActivityDelegatesSupervisor(int selfAssessmentId, int delegateUserId)
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
        public RemoveSelfAssessmentDelegate? GetDelegateSelfAssessmentByCandidateAssessmentsId(int candidateAssessmentsId)
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
                @"BEGIN TRY
                    BEGIN TRANSACTION
                        UPDATE CandidateAssessments SET RemovedDate = GETUTCDATE(), RemovalMethodID = 2
                            WHERE ID = @candidateAssessmentsId AND RemovedDate IS NULL

                        COMMIT TRANSACTION
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH",
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
        public int? GetDelegateAccountId(int centreId, int delegateUserId)
        {
            return connection.QueryFirstOrDefault<int>(
                  @"SELECT ID FROM DelegateAccounts 
                      WHERE (CentreID = @centreId) AND ( UserId =@delegateUserId)",
                  new { centreId, delegateUserId }
              );
        }
        public int CheckDelegateSelfAssessment(int candidateAssessmentsId)
        {
            return connection.QueryFirstOrDefault<int>(
                  @"SELECT COUNT(ID) Num FROM CandidateAssessments 
                      WHERE (ID = @candidateAssessmentsId) AND ( RemovalMethodID =2)  AND (RemovedDate IS NOT NULL)",
                  new { candidateAssessmentsId }
              );
        }

        public bool IsUnsupervisedSelfAssessment(int selfAssessmentId)
        {
            var ResultCount = connection.ExecuteScalar<int>(
                @"SELECT COUNT(*) FROM SelfAssessments WHERE ID = @selfAssessmentId AND SupervisorSelfAssessmentReview = 0 AND SupervisorResultsReview = 0",
                new { selfAssessmentId }
            );
            return ResultCount > 0;
        }

        public bool IsCentreSelfAssessment(int selfAssessmentId, int centreId)
        {
            var ResultCount = connection.ExecuteScalar<int>(
                @"SELECT count(*) FROM CentreSelfAssessments WHERE SelfAssessmentID = @selfAssessmentId and CentreID = @centreId",
                new { selfAssessmentId, centreId }
            );
            return ResultCount > 0;
        }

        public bool HasMinimumOptionalCompetencies(int selfAssessmentId, int delegateUserId)
        {
            return connection.ExecuteScalar<bool>(
                        @"SELECT CASE WHEN COUNT(SAS.ID)>=(SELECT MinimumOptionalCompetencies FROM SelfAssessments WHERE ID = @selfAssessmentId) 
			                        THEN 1 ELSE 0 END AS HasMinimumOptionalCompetencies
	                        FROM CandidateAssessmentOptionalCompetencies AS CAOC
		                        INNER JOIN CandidateAssessments  AS CA
			                        ON CAOC.CandidateAssessmentID = CA.ID AND CA.SelfAssessmentID = @selfAssessmentId
			                        AND CA.DelegateUserID = @delegateUserId AND CA.RemovedDate IS NULL
		                        INNER JOIN SelfAssessmentStructure AS SAS
			                        ON CAOC.CompetencyID = SAS.CompetencyID AND CAOC.CompetencyGroupID = SAS.CompetencyGroupID
			                        AND SAS.SelfAssessmentID = @selfAssessmentId
		                        INNER JOIN SelfAssessments AS SA
			                        ON SAS.SelfAssessmentID = SA.ID					
	                        WHERE (CAOC.IncludedInSelfAssessment = 1)",
                        new { selfAssessmentId, delegateUserId }
                    );
        }

        public int GetSelfAssessmentCategoryId(int selfAssessmentId)
        {
            return connection.ExecuteScalar<int>(
                @"SELECT CategoryID FROM SelfAssessments WHERE ID = @selfAssessmentId",
                new { selfAssessmentId }
            );
        }
    }
}
