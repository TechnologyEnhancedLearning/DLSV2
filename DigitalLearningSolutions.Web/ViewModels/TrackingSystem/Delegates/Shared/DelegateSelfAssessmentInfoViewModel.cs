namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DateHelper = DigitalLearningSolutions.Web.Helpers.DateHelper;

    public class DelegateSelfAssessmentInfoViewModel : BaseFilterableViewModel
    {
        public DelegateSelfAssessmentInfoViewModel(
            SelfAssessmentDelegate selfAssessmentDelegate,
            DelegateAccessRoute accessedVia,
            ReturnPageQuery returnPageQuery
        ) : this(selfAssessmentDelegate)
        {
            AccessedVia = accessedVia;
            ReturnPageQuery = returnPageQuery;
            Tags = FilterableTagHelper.GetCurrentTagsForSelfAssessmentDelegate(selfAssessmentDelegate);
        }

        private DelegateSelfAssessmentInfoViewModel(SelfAssessmentDelegate delegateInfo)
        {
            CandidateAssessmentsId = delegateInfo.CandidateAssessmentsId;
            DelegateId = delegateInfo.DelegateId;
            SelfAssessmentId = delegateInfo.SelfAssessmentId;
            CandidateNumber = delegateInfo.CandidateNumber;
            ProfessionalRegistrationNumber = delegateInfo.ProfessionalRegistrationNumber;
            Email = delegateInfo.DelegateEmail;
            StartedDate = delegateInfo.StartedDate.ToString(DateHelper.StandardDateAndTimeFormat);
            CompleteBy = delegateInfo.CompleteBy?.ToString(DateHelper.StandardDateAndTimeFormat);
            LastAccessed = delegateInfo.LastAccessed?.ToString(DateHelper.StandardDateAndTimeFormat);
            SubmittedDate = delegateInfo.SubmittedDate?.ToString(DateHelper.StandardDateAndTimeFormat);
            RemovedDate = delegateInfo.RemovedDate?.ToString(DateHelper.StandardDateAndTimeFormat);
            LaunchCount = delegateInfo.LaunchCount.ToString();
            Progress = delegateInfo.Progress;
            SignedOff = delegateInfo.SignedOff?.ToString(DateHelper.StandardDateAndTimeFormat);
            DelegateUserId = delegateInfo.DelegateUserId;
            SelfAssessmentDelegatesDisplayName =
            NameQueryHelper.GetSortableFullName(delegateInfo.DelegateFirstName, delegateInfo.DelegateLastName);
            Supervisors = delegateInfo.Supervisors;

            var enrolledByFullName = DisplayStringHelper.GetPotentiallyInactiveAdminName(
                delegateInfo.EnrolledByForename,
                delegateInfo.EnrolledBySurname,
                delegateInfo.EnrolledByAdminActive
            );
            EnrolmentMethod = delegateInfo.EnrolmentMethodId switch
            {
                1 => "Self enrolled",
                2 => enrolledByFullName==null ? "Admin/supervisor enrolled": "Enrolled by " + enrolledByFullName,
                3 => "Group",
                _ => "System",
            };
        }
        public DelegateAccessRoute AccessedVia { get; set; }
        public ReturnPageQuery? ReturnPageQuery { get; set; }
        public int CandidateAssessmentsId { get; set; }
        public int DelegateId { get; set; }
        public int SelfAssessmentId { get; set; }
        public string? CandidateNumber { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public string? Email { get; set; }
        public string StartedDate { get; set; }
        public string EnrolmentMethod { get; set; }
        public string? CompleteBy { get; set; }
        public string? LastAccessed { get; set; }
        public string LaunchCount { get; set; }
        public string? Progress { get; set; }
        public string? SignedOff { get; set; }
        public string? SubmittedDate { get; set; }
        public string? RemovedDate { get; set; }
        public int DelegateUserId { get; set; }
        public string SelfAssessmentDelegatesDisplayName { get; set; }
        public List<SelfAssessmentSupervisor> Supervisors { get; set; }

    }
}
