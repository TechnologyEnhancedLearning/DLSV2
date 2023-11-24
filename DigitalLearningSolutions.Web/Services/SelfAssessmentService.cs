﻿namespace DigitalLearningSolutions.Web.Services
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
        //Self Assessments
        string? GetSelfAssessmentNameById(int selfAssessmentId);
        // Candidate Assessments
        IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int delegateUserId, int centreId);

        CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int delegateUserId, int selfAssessmentId);

        void SetBookmark(int selfAssessmentId, int delegateUserId, string bookmark);

        void SetSubmittedDateNow(int selfAssessmentId, int delegateUserId);

        void SetUpdatedFlag(int selfAssessmentId, int delegateUserId, bool status);

        void UpdateLastAccessed(int selfAssessmentId, int delegateUserId);
        void RemoveSignoffRequests(int selfAssessmentId, int delegateUserId, int competencyGroupsId);
        void IncrementLaunchCount(int selfAssessmentId, int delegateUserId);

        void SetCompleteByDate(int selfAssessmentId, int delegateUserId, DateTime? completeByDate);

        bool CanDelegateAccessSelfAssessment(int delegateUserId, int selfAssessmentId);

        // Competencies
        IEnumerable<Competency> GetCandidateAssessmentResultsById(int candidateAssessmentId, int adminId, int? selfAssessmentResultId = null);

        IEnumerable<Competency> GetCandidateAssessmentResultsForReviewById(int candidateAssessmentId, int adminId);

        IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int delegateId);
        IEnumerable<Competency> GetResultSupervisorVerifications(int selfAssessmentId, int delegateId);

        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestion(
            int assessmentQuestionId,
            int minValue,
            int maxValue,
            bool zeroBased
        );

        Competency? GetCompetencyByCandidateAssessmentResultId(int resultId, int candidateAssessmentId, int adminId);

        Competency? GetNthCompetency(int n, int selfAssessmentId, int delegateId); // 1 indexed

        void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int delegateUserId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        );

        IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int delegateUserId);

        IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int delegateId);

        List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int delegateUserId);

        void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int delegateUserId);

        void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int delegateUserId);

        // Supervisor

        IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int delegateUserId
        );

        SupervisorComment? GetSupervisorComments(int delegateUserId, int resultId);

        IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        );

        IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(int selfAssessmentId, int delegateUserId);

        IEnumerable<Administrator> GetValidSupervisorsForActivity(int centreId, int selfAssessmentId, int delegateUserId);

        Administrator GetSupervisorByAdminId(int supervisorAdminId);

        IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        );

        SelfAssessmentSupervisor? GetSelfAssessmentSupervisorByCandidateAssessmentSupervisorId(
            int candidateAssessmentSupervisorId
        );

        IEnumerable<SelfAssessmentSupervisor> GetSignOffSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        );

        void InsertCandidateAssessmentSupervisorVerification(int candidateAssessmentSupervisorId);

        void UpdateCandidateAssessmentSupervisorVerificationEmailSent(int candidateAssessmentSupervisorVerificationId);

        // Filtered
        Profile? GetFilteredProfileForCandidateById(int delegateUserId, int selfAssessmentId);

        IEnumerable<Goal> GetFilteredGoalsForCandidateId(int delegateUserId, int selfAssessmentId);

        void LogAssetLaunch(int candidateId, int selfAssessmentId, LearningAsset learningAsset);

        // Export Self Assessment
        CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(
            int candidateAssessmentId,
            int delegateUserId
        );

        IEnumerable<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId,
            int delegateUserId
        );

        void RemoveEnrolment(int selfAssessmentId, int delegateUserId);
        public (SelfAssessmentDelegatesData, int) GetSelfAssessmentDelegatesPerPage(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, bool? submitted, bool? signedOff);
        public SelfAssessmentDelegatesData GetSelfAssessmentActivityDelegatesExport(string searchString, int itemsPerPage, string sortBy, string sortDirection,
           int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, int currentRun);
        public int GetSelfAssessmentActivityDelegatesExportCount(string searchString,  string sortBy, string sortDirection,
           int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed);
        public string GetSelfAssessmentActivityDelegatesSupervisor(int selfAssessmentId, int delegateUserId);
        RemoveSelfAssessmentDelegate GetDelegateSelfAssessmentByCandidateAssessmentsId(int candidateAssessmentsId);
       void RemoveDelegateSelfAssessment(int candidateAssessmentsId);
       public int? GetSupervisorsCountFromCandidateAssessmentId(int candidateAssessmentsId);
        public bool CheckForSameCentre(int centreId, int candidateAssessmentsId);
    }

    public class SelfAssessmentService : ISelfAssessmentService
    {
        private readonly ISelfAssessmentDataService selfAssessmentDataService;

        public SelfAssessmentService(ISelfAssessmentDataService selfAssessmentDataService)
        {
            this.selfAssessmentDataService = selfAssessmentDataService;
        }

        public CurrentSelfAssessment? GetSelfAssessmentForCandidateById(int delegateUserId, int selfAssessmentId)
        {
            return selfAssessmentDataService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);
        }

        public void SetBookmark(int selfAssessmentId, int delegateUserId, string bookmark)
        {
            selfAssessmentDataService.SetBookmark(selfAssessmentId, delegateUserId, bookmark);
        }

        public void SetSubmittedDateNow(int selfAssessmentId, int delegateUserId)
        {
            selfAssessmentDataService.SetSubmittedDateNow(selfAssessmentId, delegateUserId);
        }

        public void SetUpdatedFlag(int selfAssessmentId, int delegateUserId, bool status)
        {
            selfAssessmentDataService.SetUpdatedFlag(selfAssessmentId, delegateUserId, status);
        }

        public void UpdateLastAccessed(int selfAssessmentId, int delegateUserId)
        {
            selfAssessmentDataService.UpdateLastAccessed(selfAssessmentId, delegateUserId);
        }

        public void RemoveSignoffRequests(int selfAssessmentId, int delegateUserId, int competencyGroupId)
        {
            selfAssessmentDataService.RemoveSignoffRequests(selfAssessmentId, delegateUserId, competencyGroupId);
        }
        public void IncrementLaunchCount(int selfAssessmentId, int delegateUserId)
        {
            selfAssessmentDataService.IncrementLaunchCount(selfAssessmentId, delegateUserId);
        }

        public void SetCompleteByDate(int selfAssessmentId, int delegateUserId, DateTime? completeByDate)
        {
            selfAssessmentDataService.SetCompleteByDate(selfAssessmentId, delegateUserId, completeByDate);
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

        public IEnumerable<Competency> GetCandidateAssessmentResultsToVerifyById(int selfAssessmentId, int delegateId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentResultsToVerifyById(selfAssessmentId, delegateId);
        }

        public IEnumerable<Competency> GetResultSupervisorVerifications(int selfAssessmentId, int delegateId)
        {
            return selfAssessmentDataService.GetResultSupervisorVerifications(selfAssessmentId, delegateId);
        }

        public IEnumerable<SupervisorSignOff> GetSupervisorSignOffsForCandidateAssessment(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetSupervisorSignOffsForCandidateAssessment(selfAssessmentId, delegateUserId);
        }

        public SupervisorComment? GetSupervisorComments(int delegateUserId, int resultId)
        {
            return selfAssessmentDataService.GetSupervisorComments(delegateUserId, resultId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetAllSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetAllSupervisorsForSelfAssessmentId(selfAssessmentId, delegateUserId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetOtherSupervisorsForCandidate(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetOtherSupervisorsForCandidate(selfAssessmentId, delegateUserId);
        }

        public IEnumerable<Administrator> GetValidSupervisorsForActivity(
            int centreId,
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetValidSupervisorsForActivity(centreId, selfAssessmentId, delegateUserId).Where(c => !Guid.TryParse(c.Email, out _));
        }

        public Administrator GetSupervisorByAdminId(int supervisorAdminId)
        {
            return selfAssessmentDataService.GetSupervisorByAdminId(supervisorAdminId);
        }

        public IEnumerable<SelfAssessmentSupervisor> GetResultReviewSupervisorsForSelfAssessmentId(
            int selfAssessmentId,
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetResultReviewSupervisorsForSelfAssessmentId(
                selfAssessmentId,
                delegateUserId
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
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetSignOffSupervisorsForSelfAssessmentId(selfAssessmentId, delegateUserId);
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

        public Profile? GetFilteredProfileForCandidateById(int delegateUserId, int selfAssessmentId)
        {
            return selfAssessmentDataService.GetFilteredProfileForCandidateById(delegateUserId, selfAssessmentId);
        }

        public IEnumerable<Goal> GetFilteredGoalsForCandidateId(int delegateUserId, int selfAssessmentId)
        {
            return selfAssessmentDataService.GetFilteredGoalsForCandidateId(delegateUserId, selfAssessmentId);
        }

        public void LogAssetLaunch(int delegateUserId, int selfAssessmentId, LearningAsset learningAsset)
        {
            selfAssessmentDataService.LogAssetLaunch(delegateUserId, selfAssessmentId, learningAsset);
        }

        public CandidateAssessmentExportSummary GetCandidateAssessmentExportSummary(
            int candidateAssessmentId,
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetCandidateAssessmentExportSummary(candidateAssessmentId, delegateUserId);
        }

        public IEnumerable<CandidateAssessmentExportDetail> GetCandidateAssessmentExportDetails(
            int candidateAssessmentId,
            int delegateUserId
        )
        {
            return selfAssessmentDataService.GetCandidateAssessmentExportDetails(candidateAssessmentId, delegateUserId);
        }

        public bool CanDelegateAccessSelfAssessment(int delegateUserId, int selfAssessmentId)
        {
            var candidateAssessments = selfAssessmentDataService.GetCandidateAssessments(delegateUserId, selfAssessmentId);

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

        public Competency? GetNthCompetency(int n, int selfAssessmentId, int delegateId)
        {
            return selfAssessmentDataService.GetNthCompetency(n, selfAssessmentId, delegateId);
        }

        public void SetResultForCompetency(
            int competencyId,
            int selfAssessmentId,
            int delegateUserId,
            int assessmentQuestionId,
            int? result,
            string? supportingComments
        )
        {
            selfAssessmentDataService.SetResultForCompetency(
                competencyId,
                selfAssessmentId,
                delegateUserId,
                assessmentQuestionId,
                result,
                supportingComments
            );
        }

        public IEnumerable<Competency> GetCandidateAssessmentOptionalCompetencies(int selfAssessmentId, int delegateUserId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentOptionalCompetencies(selfAssessmentId, delegateUserId);
        }

        public IEnumerable<CurrentSelfAssessment> GetSelfAssessmentsForCandidate(int delegateUserId, int centreId)
        {
            return selfAssessmentDataService.GetSelfAssessmentsForCandidate(delegateUserId, centreId);
        }

        public IEnumerable<Competency> GetMostRecentResults(int selfAssessmentId, int delegateId)
        {
            return selfAssessmentDataService.GetMostRecentResults(selfAssessmentId, delegateId);
        }

        public List<int> GetCandidateAssessmentIncludedSelfAssessmentStructureIds(int selfAssessmentId, int delegateUserId)
        {
            return selfAssessmentDataService.GetCandidateAssessmentIncludedSelfAssessmentStructureIds(
                selfAssessmentId,
                delegateUserId
            );
        }

        public void InsertCandidateAssessmentOptionalCompetenciesIfNotExist(int selfAssessmentId, int delegateUserId)
        {
            selfAssessmentDataService.InsertCandidateAssessmentOptionalCompetenciesIfNotExist(
                selfAssessmentId,
                delegateUserId
            );
        }

        public void UpdateCandidateAssessmentOptionalCompetencies(int selfAssessmentStructureId, int delegateUserId)
        {
            selfAssessmentDataService.UpdateCandidateAssessmentOptionalCompetencies(
                selfAssessmentStructureId,
                delegateUserId
            );
        }

        public void RemoveEnrolment(int selfAssessmentId, int delegateUserId)
        {
            selfAssessmentDataService.RemoveEnrolment(selfAssessmentId, delegateUserId);
        }

        public string? GetSelfAssessmentNameById(int selfAssessmentId)
        {
            return selfAssessmentDataService.GetSelfAssessmentNameById(selfAssessmentId);
        }

        public (SelfAssessmentDelegatesData, int) GetSelfAssessmentDelegatesPerPage(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, bool? submitted, bool? signedOff)
        {
            (var delegateselfAssessments, int resultCount) = selfAssessmentDataService.GetSelfAssessmentDelegates(searchString, offSet, itemsPerPage, sortBy, sortDirection,
            selfAssessmentId, centreId, isDelegateActive, removed, submitted, signedOff);

            List<SelfAssessmentDelegate> selfAssessmentDelegateList = new List<SelfAssessmentDelegate>();
            foreach (var delegateInfo in delegateselfAssessments)
            {
                var supervisors = selfAssessmentDataService.GetAllSupervisorsForSelfAssessmentId(
                delegateInfo.SelfAssessmentId,
                delegateInfo.DelegateUserId
            ).ToList();

                delegateInfo.Supervisors = supervisors;
                selfAssessmentDelegateList.Add(new SelfAssessmentDelegate(delegateInfo));
            }
            return (new SelfAssessmentDelegatesData(selfAssessmentDelegateList), resultCount);
        }
        public SelfAssessmentDelegatesData GetSelfAssessmentActivityDelegatesExport(string searchString, int itemsPerPage, string sortBy, string sortDirection,
            int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed, int currentRun)
        {
            var delegateselfAssessments = selfAssessmentDataService.GetSelfAssessmentActivityDelegatesExport(searchString, itemsPerPage, sortBy, sortDirection,
            selfAssessmentId, centreId, isDelegateActive, removed, currentRun);

            List<SelfAssessmentDelegate> selfAssessmentDelegateList = new List<SelfAssessmentDelegate>();
            foreach (var delegateInfo in delegateselfAssessments)
            {
                var supervisors = selfAssessmentDataService.GetAllSupervisorsForSelfAssessmentId(
                delegateInfo.SelfAssessmentId,
                delegateInfo.DelegateUserId
            ).ToList();

                delegateInfo.Supervisors = supervisors;
                selfAssessmentDelegateList.Add(new SelfAssessmentDelegate(delegateInfo));
            }
            return new SelfAssessmentDelegatesData(selfAssessmentDelegateList);
        }
        public int GetSelfAssessmentActivityDelegatesExportCount(string searchString, string sortBy, string sortDirection,
           int? selfAssessmentId, int centreId, bool? isDelegateActive, bool? removed)
        {
            int resultCount = selfAssessmentDataService.GetSelfAssessmentActivityDelegatesExportCount(searchString, sortBy, sortDirection,
            selfAssessmentId, centreId, isDelegateActive, removed);


            return resultCount;
        }
        public string GetSelfAssessmentActivityDelegatesSupervisor(int selfAssessmentId, int delegateUserId)
        {
            return selfAssessmentDataService.GetSelfAssessmentActivityDelegatesSupervisor(selfAssessmentId, delegateUserId);
        }
        public RemoveSelfAssessmentDelegate GetDelegateSelfAssessmentByCandidateAssessmentsId(int candidateAssessmentsId)
        {
            return selfAssessmentDataService.GetDelegateSelfAssessmentByCandidateAssessmentsId(candidateAssessmentsId);
        }
        public void RemoveDelegateSelfAssessment(int candidateAssessmentsId)
        {
            selfAssessmentDataService.RemoveDelegateSelfAssessment(candidateAssessmentsId);
        }
        public int? GetSupervisorsCountFromCandidateAssessmentId(int candidateAssessmentsId)
        {
            return selfAssessmentDataService.GetSupervisorsCountFromCandidateAssessmentId(candidateAssessmentsId);
        }
        public bool CheckForSameCentre(int centreId, int candidateAssessmentsId)
        {
            return selfAssessmentDataService.CheckForSameCentre(centreId, candidateAssessmentsId);
        }
    }
}
