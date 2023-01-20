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
        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int delegateUserId);

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

        IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(int selfAssessmentId, int delegateUserId);

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
