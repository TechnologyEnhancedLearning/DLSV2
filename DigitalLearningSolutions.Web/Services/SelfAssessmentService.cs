namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Data.Models.SelfAssessments.Export;

    public interface ISelfAssessmentService
    {
        // Candidate Assessments
        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForUser(int userId);

        CurrentSelfAssessment? GetSelfAssessmentForUserById(int userId, int selfAssessmentId);

        void SetBookmark(int selfAssessmentId, int userId, string bookmark);

        void SetSubmittedDateNow(int selfAssessmentId, int userId);

        void SetUpdatedFlag(int selfAssessmentId, int userId, bool status);

        void UpdateLastAccessed(int selfAssessmentId, int userId);

        void IncrementLaunchCount(int selfAssessmentId, int userId);

        void SetCompleteByDate(int selfAssessmentId, int userId, DateTime? completeByDate);

        bool CanDelegateAccessSelfAssessment(int userId, int selfAssessmentId);

        // Competencies
        IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId, int? selfAssessmentResultId = null);

        IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(int candidateAssessmentId, int adminId);

        IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int userId);
        IEnumerable<Competency> GetResultSupervisorVerifications(int selfAssessmentId, int userId);

        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(
            int assessmentQuestionId,
            int minValue,
            int maxValue,
            bool zeroBased
        );

        Competency? GetCompetencyByCandidateAssessmentResultId(int resultId, int candidateAssessmentId, int adminId);

        Competency? GetNthCompetency(int n, int selfAssessmentId, int userId); // 1 indexed

        void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int candidateId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        );

        IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int userId);

        IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int userId);

        List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int userId);

        void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int userId);

        void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int userId);

        // Supervisor
        IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(int selfAssessmentId, int userId);

        IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int userId
        );

        SupervisorComment? GetSupervisorComments(int userId, int resultId);

        IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        );

        IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(int selfAssessmentId, int userId);

        IEnumerable<Administrator> GetValidSupervisorsForActivity(int centreId, int selfAssessmentId, int userId);

        Administrator GetSupervisorByAdminId(int supervisorAdminId);

        IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        );

        SelfAssessmentSupervisor? GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
            int candidateAssessmentSupervisorId
        );

        IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        );

        void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId);

        void UpdateCandidateAssessmentSupervisorVerificationEmailSent(int candidateAssessmentSupervisorVerificationId);

        // Filtered
        Profile? GetFilteredProfileForCandidateById(int candidateId, int selfAssessmentId);

        IEnumerable<Goal> GetFilteredGoalsForCandidateId(int candidateId, int selfAssessmentId);

        void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset);

        // Export Self Assessment
        CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(
            int candidateAssessmentId,
            int userId
        );

        IEnumerable<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId,
            int userId
        );
    }

    public class SelfAssessmentService : ISelfAssessmentService
    {
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public SelfAssessmentService(ISelfAssessmentDataService selfAssessmentDataService)
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
        }

        public CurrentSelfAssessment? GetSelfAssessmentForUserById(int userId, int selfAssessmentId)
        {
            return selfAssessmentDataService.GetSelfAssessmentForUserById(userId, selfAssessmentId);
        }

        public void SetBookmark(int selfAssessmentId, int userId, string bookmark)
        {
            selfAssessmentDataService.SetBookmark(selfAssessmentId, userId, bookmark);
        }

        public void SetSubmittedDateNow(int selfAssessmentId, int userId)
        {
            selfAssessmentDataService.SetSubmittedDateNow(selfAssessmentId, userId);
        }

        public void SetUpdatedFlag(int selfAssessmentId, int userId, bool status)
        {
            selfAssessmentDataService.SetUpdatedFlag(selfAssessmentId, userId, status);
        }

        public void UpdateLastAccessed(int selfAssessmentId, int userId)
        {
            selfAssessmentDataService.UpdateLastAccessed(selfAssessmentId, userId);
        }

        public void IncrementLaunchCount(int selfAssessmentId, int userId)
        {
            selfAssessmentDataService.IncrementLaunchCount(selfAssessmentId, userId);
        }

        public void SetCompleteByDate(int selfAssessmentId, int userId, DateTime? completeByDate)
        {
            selfAssessmentDataService.SetCompleteByDate(selfAssessmentId, userId, completeByDate);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId, int? selfAssessmentResultId = null)
        {
            return selfAssessmentDataService.GetCandidateAssessmentResultsById(candidateAssessmentId, adminId, selfAssessmentResultId);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(
            int candidateAssessmentId,
            int adminId
        )
        {
            return selfAssessmentDataService.GetCandidateAssessmentResultsForReviewById(candidateAssessmentId, adminId);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int userId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentResultsToVerifyById(selfAssessmentId, userId);
        }

        public IEnumerable<Competency> GetResultSupervisorVerifications(int selfAssessmentId, int userId)
        {
            return selfAssessmentDataService.GetResultSupervisorVerifications(selfAssessmentId, userId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetSupervisorsForSelfAssessmentId(selfAssessmentId, userId);
        }

        public IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, userId);
        }

        public SupervisorComment? GetSupervisorComments(int userId, int resultId)
        {
            return selfAssessmentDataService.GetSupervisorComments(userId, resultId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetAllSupervisorsForSelfAssessmentId(selfAssessmentId, userId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(
            int selfAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetOtherSupervisorsForCandidate(selfAssessmentId, userId);
        }

        public IEnumerable<Administrator> GetValidSupervisorsForActivity(
            int centreId,
            int selfAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetValidSupervisorsForActivity(centreId, selfAssessmentId, userId);
        }

        public Administrator GetSupervisorByAdminId(int supervisorAdminId)
        {
            return selfAssessmentDataService.GetSupervisorByAdminId(supervisorAdminId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetResultReviewSupervisorsForSelfAssessmentId(
                selfAssessmentId,
                userId
            );
        }

        public SelfAssessmentSupervisor? GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
            int candidateAssessmentSupervisorId
        )
        {
            return selfAssessmentDataService.GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
                candidateAssessmentSupervisorId
            );
        }

        public IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetSignOffSupervisorsForSelfAssessmentId(selfAssessmentId, userId);
        }

        public void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId)
        {
            selfAssessmentDataService.InsertCandidateAssessmentSupervisorVerification(candidateAssessmentSupervisorId);
        }

        public void UpdateCandidateAssessmentSupervisorVerificationEmailSent(
            int candidateAssessmentSupervisorVerificationId
        )
        {
            selfAssessmentDataService.UpdateCandidateAssessmentSupervisorVerificationEmailSent(
                candidateAssessmentSupervisorVerificationId
            );
        }

        public Profile? GetFilteredProfileForCandidateById(int candidateId, int selfAssessmentId)
        {
            return selfAssessmentDataService.GetFilteredProfileForCandidateById(candidateId, selfAssessmentId);
        }

        public IEnumerable<Goal> GetFilteredGoalsForCandidateId(int candidateId, int selfAssessmentId)
        {
            return selfAssessmentDataService.GetFilteredGoalsForCandidateId(candidateId, selfAssessmentId);
        }

        public void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset)
        {
            selfAssessmentDataService.LogAssetLaunch(candidateId, selfAssessmentId, learningAsset);
        }

        public CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(
            int candidateAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetCandidateAssessmentExportSummary(candidateAssessmentId, userId);
        }

        public IEnumerable<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId,
            int userId
        )
        {
            return selfAssessmentDataService.GetCandidateAssessmentExportDetails(candidateAssessmentId, userId);
        }

        public bool CanDelegateAccessSelfAssessment(int userId, int selfAssessmentId)
        {
            var candidateAssessments = selfAssessmentDataService.GetCandidateAssessments(userId, selfAssessmentId);

            return candidateAssessments.Any(ca => ca.CompletedDate == null && ca.RemovedDate == null);
        }

        public IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(
            int assessmentQuestionId,
            int minValue,
            int maxValue,
            bool zeroBased
        )
        {
            return selfAssessmentDataService.GetLevelDescriptorsForAssessmentQuestion(
                assessmentQuestionId,
                minValue,
                maxValue,
                zeroBased
            );
        }

        public Competency? GetCompetencyByCandidateAssessmentResultId(
            int resultId,
            int candidateAssessmentId,
            int adminId
        )
        {
            return selfAssessmentDataService.GetCompetencyByCandidateAssessmentResultId(
                resultId,
                candidateAssessmentId,
                adminId
            );
        }

        public Competency? GetNthCompetency(int n, int selfAssessmentId, int userId)
        {
            return selfAssessmentDataService.GetNthCompetency(n, selfAssessmentId, userId);
        }

        public void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int candidateId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        )
        {
            selfAssessmentDataService.SetResultForCompetency(
                competencyId,
                selfAssessmentId,
                candidateId,
                assessmentQuestionId,
                result,
                supportingComments
            );
        }

        public IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int userId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, userId);
        }

        public IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForUser(int userId)
        {
            return selfAssessmentDataService.GetSelfAssessmentsForUser(userId);
        }

        public IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int userId)
        {
            return selfAssessmentDataService.GetMostRecentResults(selfAssessmentId, userId);
        }

        public List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int userId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentIncludedSelfAssessmentStructureIds(
                selfAssessmentId,
                userId
            );
        }

        public void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int userId)
        {
            selfAssessmentDataService.InsertCandidateAssessmentOptionalCompetenciesIfNotExist(
                selfAssessmentId,
                userId
            );
        }

        public void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int userId)
        {
            selfAssessmentDataService.UpdateCandidateAssessmentOptionalCompetencies(
                selfAssessmentStructureId,
                userId
            );
        }
    }
}
