namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Models.Common.Users;
    using DigitalLearningSolutions.Data.Models.External.Filtered;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public interface ISelfAssessmentService
    {
        // Candidate Assessments
        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int candidateId);

        CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int candidateId, int selfAssessmentId);

        void SetBookmark(int selfAssessmentId, int candidateId, string bookmark);

        void SetSubmittedDateNow(int selfAssessmentId, int candidateId);

        void SetUpdatedFlag(int selfAssessmentId, int candidateId, bool status);

        void UpdateLastAccessed(int selfAssessmentId, int candidateId);

        void IncrementLaunchCount(int selfAssessmentId, int candidateId);

        void SetCompleteByDate(int selfAssessmentId, int candidateId, DateTime? completeByDate);

        // Competencies
        IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId);

        IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(int candidateAssessmentId, int adminId);

        IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int candidateId);

        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(
            int assessmentQuestionId,
            int minValue,
            int maxValue,
            bool zeroBased
        );

        Competency? GetCompetencyByCandidateAssessmentResultId(int resultId, int candidateAssessmentId, int adminId);

        Competency? GetNthCompetency(int n, int selfAssessmentId, int candidateId); // 1 indexed

        void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int candidateId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        );

        IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int candidateId);

        IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int candidateId);

        List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int candidateId);

        void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int candidateId);

        void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int candidateId);

        // Supervisor
        IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(int selfAssessmentId, int candidateId);

        SelfAssessmentSupervisor? GetSupervisorForSelfAssessmentId(int selfAssessmentId, int candidateId);

        IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int candidateId
        );

        SupervisorComment? GetSupervisorComments(int candidateId, int resultId);


        IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int candidateId
        );

        IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(int selfAssessmentId, int candidateId);

        IEnumerable<Administrator> GetValidSupervisorsForActivity(int centreId, int selfAssessmentId, int candidateId);

        Administrator GetSupervisorByAdminId(int supervisorAdminId);

        IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int candidateId
        );

        SelfAssessmentSupervisor? GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
            int candidateAssessmentSupervisorId
        );

        IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int candidateId
        );

        void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId);

        void UpdateCandidateAssessmentSupervisorVerificationEmailSent(int candidateAssessmentSupervisorVerificationId);

        // Filtered
        Profile? GetFilteredProfileForCandidateById(int candidateId, int selfAssessmentId);

        IEnumerable<Goal> GetFilteredGoalsForCandidateId(int candidateId, int selfAssessmentId);

        void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset);
    }

    public class SelfAssessmentService : ISelfAssessmentService
    {
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public SelfAssessmentService(ISelfAssessmentDataService selfAssessmentDataService)
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
        }

        public CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int candidateId, int selfAssessmentId)
        {
            return selfAssessmentDataService.GetSelfAssessmentForCandidateById(candidateId, selfAssessmentId);
        }

        public void SetBookmark(int selfAssessmentId, int candidateId, string bookmark)
        {
            selfAssessmentDataService.SetBookmark(selfAssessmentId, candidateId, bookmark);
        }

        public void SetSubmittedDateNow(int selfAssessmentId, int candidateId)
        {
            selfAssessmentDataService.SetSubmittedDateNow(selfAssessmentId, candidateId);
        }

        public void SetUpdatedFlag(int selfAssessmentId, int candidateId, bool status)
        {
            selfAssessmentDataService.SetUpdatedFlag(selfAssessmentId, candidateId, status);
        }

        public void UpdateLastAccessed(int selfAssessmentId, int candidateId)
        {
            selfAssessmentDataService.UpdateLastAccessed(selfAssessmentId, candidateId);
        }

        public void IncrementLaunchCount(int selfAssessmentId, int candidateId)
        {
            selfAssessmentDataService.IncrementLaunchCount(selfAssessmentId, candidateId);
        }

        public void SetCompleteByDate(int selfAssessmentId, int candidateId, DateTime? completeByDate)
        {
            selfAssessmentDataService.SetCompleteByDate(selfAssessmentId, candidateId, completeByDate);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentResultsById(candidateAssessmentId, adminId);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(
            int candidateAssessmentId,
            int adminId
        )
        {
            return selfAssessmentDataService.GetCandidateAssessmentResultsForReviewById(candidateAssessmentId, adminId);
        }

        public IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int candidateId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentResultsToVerifyById(selfAssessmentId, candidateId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int candidateId
        )
        {
            return selfAssessmentDataService.GetSupervisorsForSelfAssessmentId(selfAssessmentId, candidateId);
        }

        public IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int candidateId
        )
        {
            return selfAssessmentDataService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, candidateId);
        }

        public SupervisorComment? GetSupervisorComments(int candidateId, int resultId)
        {
            return selfAssessmentDataService.GetSupervisorComments(candidateId, resultId);
        }

        public SelfAssessmentSupervisor? GetSupervisorForSelfAssessmentId(int selfAssessmentId, int candidateId)
        {
            return selfAssessmentDataService.GetSupervisorForSelfAssessmentId(selfAssessmentId, candidateId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int candidateId
        )
        {
            return selfAssessmentDataService.GetAllSupervisorsForSelfAssessmentId(selfAssessmentId, candidateId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(
            int selfAssessmentId,
            int candidateId
        )
        {
            return selfAssessmentDataService.GetOtherSupervisorsForCandidate(selfAssessmentId, candidateId);
        }

        public IEnumerable<Administrator> GetValidSupervisorsForActivity(
            int centreId,
            int selfAssessmentId,
            int candidateId
        )
        {
            return selfAssessmentDataService.GetValidSupervisorsForActivity(centreId, selfAssessmentId, candidateId);
        }

        public Administrator GetSupervisorByAdminId(int supervisorAdminId)
        {
            return selfAssessmentDataService.GetSupervisorByAdminId(supervisorAdminId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int candidateId
        )
        {
            return selfAssessmentDataService.GetResultReviewSupervisorsForSelfAssessmentId(
                selfAssessmentId,
                candidateId
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
            int candidateId
        )
        {
            return selfAssessmentDataService.GetSignOffSupervisorsForSelfAssessmentId(selfAssessmentId, candidateId);
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

        public Competency? GetNthCompetency(int n, int selfAssessmentId, int candidateId)
        {
            return selfAssessmentDataService.GetNthCompetency(n, selfAssessmentId, candidateId);
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

        public IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int candidateId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, candidateId);
        }

        public IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int candidateId)
        {
            return selfAssessmentDataService.GetSelfAssessmentsForCandidate(candidateId);
        }

        public IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int candidateId)
        {
            return selfAssessmentDataService.GetMostRecentResults(selfAssessmentId, candidateId);
        }

        public List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int candidateId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentIncludedSelfAssessmentStructureIds(
                selfAssessmentId,
                candidateId
            );
        }

        public void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int candidateId)
        {
            selfAssessmentDataService.InsertCandidateAssessmentOptionalCompetenciesIfNotExist(
                selfAssessmentId,
                candidateId
            );
        }

        public void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int candidateId)
        {
            selfAssessmentDataService.UpdateCandidateAssessmentOptionalCompetencies(
                selfAssessmentStructureId,
                candidateId
            );
        }
    }
}
