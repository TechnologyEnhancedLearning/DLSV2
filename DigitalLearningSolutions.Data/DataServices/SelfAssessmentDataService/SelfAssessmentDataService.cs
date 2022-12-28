namespace DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;
    using Microsoft.Extensions.Logging;

    public interface ISelfAssessmentDataService
    {
        // CompetencyDataService
        IEnumerable<int> GetCompetencyIdsForSelfAssessment(int selfAssessmentId);

        Competency? GetNthCompetency(int n, int selfAssessmentId, int userId); // 1 indexed

        IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int userId);

        IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId, int? selfAssessmentResultId = null);

        IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(int candidateAssessmentId, int adminId);

        IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int userId);
        IEnumerable<Competency> GetResultSupervisorVerifications(int selfAssessmentId, int userId);

        Competency? GetCompetencyByCandidateAssessmentResultId(int resultId, int candidateAssessmentId, int adminId);

        void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int candidateId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        );

        IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int userId);

        void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int userId);

        void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int userId);

        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(
            int assessmentQuestionId,
            int minValue,
            int maxValue,
            bool zeroBased
        );

        List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int userId);

        CompetencyAssessmentQuestionRoleRequirement? GetCompetencyAssessmentQuestionRoleRequirements(
            int competencyId,
            int selfAssessmentId,
            int assessmentQuestionId,
            int levelValue
        );

        IEnumerable<SelfAssessmentResult> GetSelfAssessmentResultsForDelegateSelfAssessmentCompetency(
            int delegateId,
            int selfAssessmentId,
            int competencyId
        );

        // CandidateAssessmentsDataService
        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForUser(int userId);

        CurrentSelfAssessment? GetSelfAssessmentForUserById(int userId, int selfAssessmentId);

        void UpdateLastAccessed(int selfAssessmentId, int userId);

        void SetCompleteByDate(int selfAssessmentId, int userId, DateTime? completeByDate);

        void SetSubmittedDateNow(int selfAssessmentId, int userId);

        void IncrementLaunchCount(int selfAssessmentId, int userId);

        void SetUpdatedFlag(int selfAssessmentId, int userId, bool status);

        void SetBookmark(int selfAssessmentId, int userId, string bookmark);

        IEnumerable<CandidateAssessment> GetCandidateAssessments(int userId, int selfAssessmentId);

        // SelfAssessmentSupervisorDataService
        SelfAssessmentSupervisor? GetSupervisorForSelfAssessmentId(int selfAssessmentId, int candidateId);

        IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(int selfAssessmentId, int userId);

        IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(int selfAssessmentId, int userId);

        IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        );

        IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        );

        IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        );

        SelfAssessmentSupervisor? GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
            int candidateAssessmentSupervisorId
        );

        void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId);

        void UpdateCandidateAssessmentSupervisorVerificationEmailSent(int candidateAssessmentSupervisorVerificationId);

        SupervisorComment? GetSupervisorComments(int userId, int resultId);

        IEnumerable<Administrator> GetValidSupervisorsForActivity(int centreId, int selfAssessmentId, int userId);

        Administrator GetSupervisorByAdminId(int supervisorAdminId);

        IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int userId
        );

        // FilteredDataService
        Profile? GetFilteredProfileForCandidateById(int candidateId, int selfAssessmentId);

        IEnumerable<Goal> GetFilteredGoalsForCandidateId(int candidateId, int selfAssessmentId);

        void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset);

        //Export Candidate Assessment
        CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(
            int candidateAssessmentId,
            int userId
        );

        List<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId,
            int userId
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
    }
}
