using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.RoleProfiles;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Data.Models.Supervisor;
using System;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ISupervisorService
    {
        //GET DATA
        DashboardData? GetDashboardDataForAdminId(int adminId);
        IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId);
        SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId, int adminId, int delegateUserId);
        SupervisorDelegate GetSupervisorDelegate(int adminId, int delegateUserId);
        int? ValidateDelegate(int centreId, string delegateEmail);
        IEnumerable<DelegateSelfAssessment> GetSelfAssessmentsForSupervisorDelegateId(int supervisorDelegateId, int adminId, int? adminIdCategoryId);
        DelegateSelfAssessment? GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId, int? adminIdCategoryId);
        IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedSignOffs(int adminId);
        IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedReviews(int adminId);
        DelegateSelfAssessment? GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId);
        IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int candidateId, int centreId, int? categoryId);
        RoleProfile? GetRoleProfileById(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesForSelfAssessment(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesBySelfAssessmentIdForSupervisor(int selfAssessmentId);
        IEnumerable<SelfAssessmentSupervisorRole> GetDelegateNominatableSupervisorRolesForSelfAssessment(int selfAssessmentId);
        SelfAssessmentSupervisorRole? GetSupervisorRoleById(int id);
        DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(int selfAssessmentId, int supervisorDelegateId);
        DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(int candidateAssessmentId, int supervisorDelegateId);
        CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisorById(int candidateAssessmentSupervisorId);
        CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisor(int candidateAssessmentID, int supervisorDelegateId, int selfAssessmentSupervisorRoleId);
        SelfAssessmentResultSummary? GetSelfAssessmentResultSummary(int candidateAssessmentId, int supervisorDelegateId);
        IEnumerable<CandidateAssessmentSupervisorVerificationSummary> GetCandidateAssessmentSupervisorVerificationSummaries(int candidateAssessmentId);
        IEnumerable<SupervisorForEnrolDelegate> GetSupervisorForEnrolDelegate(int CentreID, int CategoryID);
        //UPDATE DATA
        bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId);
        bool RemoveSupervisorDelegateById(int supervisorDelegateId, int delegateUserId, int adminId);
        bool UpdateSelfAssessmentResultSupervisorVerifications(int selfAssessmentResultSupervisorVerificationId, string? comments, bool signedOff, int adminId);
        bool UpdateSelfAssessmentResultSupervisorVerificationsEmailSent(int selfAssessmentResultSupervisorVerificationId);
        int RemoveSelfAssessmentResultSupervisorVerificationById(int id);
        bool RemoveCandidateAssessment(int candidateAssessmentId);
        void UpdateNotificationSent(int supervisorDelegateId);
        void UpdateCandidateAssessmentSupervisorVerificationById(int? candidateAssessmentSupervisorVerificationId, string? supervisorComments, bool signedOff);
        //INSERT DATA
        int AddSuperviseDelegate(int? supervisorAdminId, int? delegateUserId, string delegateEmail, string supervisorEmail, int centreId);
        int EnrolDelegateOnAssessment(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, DateTime? completeByDate, int? selfAssessmentSupervisorRoleId, int adminId, int centreId, bool isLoggedInUser);
        int InsertCandidateAssessmentSupervisor(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId);
        bool InsertSelfAssessmentResultSupervisorVerification(int candidateAssessmentSupervisorId, int resultId);
        //DELETE DATA
        bool RemoveCandidateAssessmentSupervisor(int selfAssessmentId, int supervisorDelegateId);
        int IsSupervisorDelegateExistAndReturnId(int? supervisorAdminId, string delegateEmail, int centreId);
        SupervisorDelegate GetSupervisorDelegateById(int supervisorDelegateId);
        void RemoveCandidateAssessmentSupervisorVerification(int id);
        bool RemoveDelegateSelfAssessmentsupervisor(int candidateAssessmentId, int supervisorDelegateId);
        void UpdateCandidateAssessmentNonReportable(int candidateAssessmentId);
    }
    public class SupervisorService : ISupervisorService
    {
        private readonly ISupervisorDataService supervisorDataService;
        public SupervisorService(ISupervisorDataService supervisorDataService)
        {
            this.supervisorDataService = supervisorDataService;
        }
        public int AddSuperviseDelegate(int? supervisorAdminId, int? delegateUserId, string delegateEmail, string supervisorEmail, int centreId)
        {
            return supervisorDataService.AddSuperviseDelegate(supervisorAdminId, delegateUserId, delegateEmail, supervisorEmail, centreId);
        }

        public bool ConfirmSupervisorDelegateById(int supervisorDelegateId, int candidateId, int adminId)
        {
            return supervisorDataService.ConfirmSupervisorDelegateById(supervisorDelegateId, candidateId, adminId);
        }

        public int EnrolDelegateOnAssessment(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, DateTime? completeByDate, int? selfAssessmentSupervisorRoleId, int adminId, int centreId, bool isLoggedInUser)
        {
            return supervisorDataService.EnrolDelegateOnAssessment(delegateUserId, supervisorDelegateId, selfAssessmentId, completeByDate, selfAssessmentSupervisorRoleId, adminId, centreId, isLoggedInUser);
        }

        public IEnumerable<RoleProfile> GetAvailableRoleProfilesForDelegate(int candidateId, int centreId, int? categoryId)
        {
            return supervisorDataService.GetAvailableRoleProfilesForDelegate(candidateId, centreId, categoryId);
        }

        public CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisor(int candidateAssessmentID, int supervisorDelegateId, int selfAssessmentSupervisorRoleId)
        {
            return supervisorDataService.GetCandidateAssessmentSupervisor(candidateAssessmentID, supervisorDelegateId, selfAssessmentSupervisorRoleId);
        }

        public CandidateAssessmentSupervisor? GetCandidateAssessmentSupervisorById(int candidateAssessmentSupervisorId)
        {
            return supervisorDataService.GetCandidateAssessmentSupervisorById(candidateAssessmentSupervisorId);
        }

        public IEnumerable<CandidateAssessmentSupervisorVerificationSummary> GetCandidateAssessmentSupervisorVerificationSummaries(int candidateAssessmentId)
        {
            return supervisorDataService.GetCandidateAssessmentSupervisorVerificationSummaries(candidateAssessmentId);
        }

        public DashboardData? GetDashboardDataForAdminId(int adminId)
        {
            return supervisorDataService.GetDashboardDataForAdminId(adminId);
        }

        public IEnumerable<SelfAssessmentSupervisorRole> GetDelegateNominatableSupervisorRolesForSelfAssessment(int selfAssessmentId)
        {
            return supervisorDataService.GetDelegateNominatableSupervisorRolesForSelfAssessment(selfAssessmentId);
        }

        public RoleProfile? GetRoleProfileById(int selfAssessmentId)
        {
            return supervisorDataService.GetRoleProfileById(selfAssessmentId);
        }

        public DelegateSelfAssessment? GetSelfAssessmentBaseByCandidateAssessmentId(int candidateAssessmentId)
        {
            return supervisorDataService.GetSelfAssessmentBaseByCandidateAssessmentId(candidateAssessmentId);
        }

        public DelegateSelfAssessment? GetSelfAssessmentByCandidateAssessmentId(int candidateAssessmentId, int adminId, int? adminIdCategoryId)
        {
            return supervisorDataService.GetSelfAssessmentByCandidateAssessmentId(candidateAssessmentId, adminId, adminIdCategoryId);
        }

        public DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(int candidateAssessmentId, int supervisorDelegateId)
        {
            return supervisorDataService.GetSelfAssessmentBySupervisorDelegateCandidateAssessmentId(candidateAssessmentId, supervisorDelegateId);
        }

        public DelegateSelfAssessment? GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(int selfAssessmentId, int supervisorDelegateId)
        {
            return supervisorDataService.GetSelfAssessmentBySupervisorDelegateSelfAssessmentId(selfAssessmentId, supervisorDelegateId);
        }

        public SelfAssessmentResultSummary? GetSelfAssessmentResultSummary(int candidateAssessmentId, int supervisorDelegateId)
        {
            return supervisorDataService.GetSelfAssessmentResultSummary(candidateAssessmentId, supervisorDelegateId);
        }

        public IEnumerable<DelegateSelfAssessment> GetSelfAssessmentsForSupervisorDelegateId(int supervisorDelegateId, int adminId, int? adminIdCategoryId)
        {
            return supervisorDataService.GetSelfAssessmentsForSupervisorDelegateId(supervisorDelegateId, adminId, adminIdCategoryId);
        }

        public IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedReviews(int adminId)
        {
            return supervisorDataService.GetSupervisorDashboardToDoItemsForRequestedReviews(adminId);
        }

        public IEnumerable<SupervisorDashboardToDoItem> GetSupervisorDashboardToDoItemsForRequestedSignOffs(int adminId)
        {
            return supervisorDataService.GetSupervisorDashboardToDoItemsForRequestedSignOffs(adminId);
        }

        public SupervisorDelegate GetSupervisorDelegate(int adminId, int delegateUserId)
        {
            return supervisorDataService.GetSupervisorDelegate(adminId, delegateUserId);
        }

        public SupervisorDelegate GetSupervisorDelegateById(int supervisorDelegateId)
        {
            return supervisorDataService.GetSupervisorDelegateById(supervisorDelegateId);
        }

        public SupervisorDelegateDetail GetSupervisorDelegateDetailsById(int supervisorDelegateId, int adminId, int delegateUserId)
        {
            return supervisorDataService.GetSupervisorDelegateDetailsById(supervisorDelegateId, adminId, delegateUserId);
        }

        public IEnumerable<SupervisorDelegateDetail> GetSupervisorDelegateDetailsForAdminId(int adminId)
        {
            return supervisorDataService.GetSupervisorDelegateDetailsForAdminId(adminId);
        }

        public IEnumerable<SupervisorForEnrolDelegate> GetSupervisorForEnrolDelegate(int CentreID, int CategoryID)
        {
            return supervisorDataService.GetSupervisorForEnrolDelegate(CentreID, CategoryID);
        }

        public SelfAssessmentSupervisorRole? GetSupervisorRoleById(int id)
        {
            return supervisorDataService.GetSupervisorRoleById(id);
        }

        public IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesBySelfAssessmentIdForSupervisor(int selfAssessmentId)
        {
            return supervisorDataService.GetSupervisorRolesBySelfAssessmentIdForSupervisor(selfAssessmentId);
        }

        public IEnumerable<SelfAssessmentSupervisorRole> GetSupervisorRolesForSelfAssessment(int selfAssessmentId)
        {
            return supervisorDataService.GetSupervisorRolesForSelfAssessment(selfAssessmentId);
        }

        public int InsertCandidateAssessmentSupervisor(int delegateUserId, int supervisorDelegateId, int selfAssessmentId, int? selfAssessmentSupervisorRoleId)
        {
            return supervisorDataService.InsertCandidateAssessmentSupervisor(delegateUserId, supervisorDelegateId, selfAssessmentId, selfAssessmentSupervisorRoleId);
        }

        public bool InsertSelfAssessmentResultSupervisorVerification(int candidateAssessmentSupervisorId, int resultId)
        {
            return supervisorDataService.InsertSelfAssessmentResultSupervisorVerification(candidateAssessmentSupervisorId, resultId);
        }

        public int IsSupervisorDelegateExistAndReturnId(int? supervisorAdminId, string delegateEmail, int centreId)
        {
            return supervisorDataService.IsSupervisorDelegateExistAndReturnId(supervisorAdminId, delegateEmail, centreId);
        }

        public bool RemoveCandidateAssessment(int candidateAssessmentId)
        {
            return supervisorDataService.RemoveCandidateAssessment(candidateAssessmentId);
        }

        public bool RemoveCandidateAssessmentSupervisor(int selfAssessmentId, int supervisorDelegateId)
        {
            return supervisorDataService.RemoveCandidateAssessmentSupervisor(selfAssessmentId, supervisorDelegateId);
        }

        public void RemoveCandidateAssessmentSupervisorVerification(int id)
        {
            supervisorDataService.RemoveCandidateAssessmentSupervisorVerification(id);
        }

        public bool RemoveDelegateSelfAssessmentsupervisor(int candidateAssessmentId, int supervisorDelegateId)
        {
            return supervisorDataService.RemoveDelegateSelfAssessmentsupervisor(candidateAssessmentId, supervisorDelegateId);
        }

        public int RemoveSelfAssessmentResultSupervisorVerificationById(int id)
        {
            return supervisorDataService.RemoveSelfAssessmentResultSupervisorVerificationById(id);
        }

        public bool RemoveSupervisorDelegateById(int supervisorDelegateId, int delegateUserId, int adminId)
        {
            return supervisorDataService.RemoveSupervisorDelegateById(supervisorDelegateId, delegateUserId, adminId);
        }

        public void UpdateCandidateAssessmentNonReportable(int candidateAssessmentId)
        {
            supervisorDataService.UpdateCandidateAssessmentNonReportable(candidateAssessmentId);
        }

        public void UpdateCandidateAssessmentSupervisorVerificationById(int? candidateAssessmentSupervisorVerificationId, string? supervisorComments, bool signedOff)
        {
            supervisorDataService.UpdateCandidateAssessmentSupervisorVerificationById(candidateAssessmentSupervisorVerificationId, supervisorComments, signedOff);
        }

        public void UpdateNotificationSent(int supervisorDelegateId)
        {
            supervisorDataService.UpdateNotificationSent(supervisorDelegateId);
        }

        public bool UpdateSelfAssessmentResultSupervisorVerifications(int selfAssessmentResultSupervisorVerificationId, string? comments, bool signedOff, int adminId)
        {
            return supervisorDataService.UpdateSelfAssessmentResultSupervisorVerifications(selfAssessmentResultSupervisorVerificationId, comments, signedOff, adminId);
        }

        public bool UpdateSelfAssessmentResultSupervisorVerificationsEmailSent(int selfAssessmentResultSupervisorVerificationId)
        {
            return supervisorDataService.UpdateSelfAssessmentResultSupervisorVerificationsEmailSent(selfAssessmentResultSupervisorVerificationId);
        }

        public int? ValidateDelegate(int centreId, string delegateEmail)
        {
            return supervisorDataService.ValidateDelegate(centreId, delegateEmail);
        }
    }
}
