using DigitalLearningSolutions.Data.Helpers;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using System;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Data.Models.SelfAssessments
{
    public class SelfAssessmentDelegate : BaseSearchableItem
    {
        public SelfAssessmentDelegate() { }
        public SelfAssessmentDelegate(int selfAssessmentId, string delegateLastName)
        {
            SelfAssessmentId = selfAssessmentId;
            DelegateLastName = delegateLastName;
        }
        public SelfAssessmentDelegate(SelfAssessmentDelegate delegateInfo)
        {
            DelegateId = delegateInfo.DelegateId;
            DelegateFirstName = delegateInfo.DelegateFirstName;
            DelegateLastName = delegateInfo.DelegateLastName;
            DelegateEmail = delegateInfo.DelegateEmail;
            IsDelegateActive = delegateInfo.IsDelegateActive;
            SelfAssessmentId = delegateInfo.SelfAssessmentId;
            CandidateNumber = delegateInfo.CandidateNumber;
            ProfessionalRegistrationNumber = delegateInfo.ProfessionalRegistrationNumber;
            StartedDate = delegateInfo.StartedDate;
            EnrolmentMethodId = delegateInfo.EnrolmentMethodId;
            CompleteBy = delegateInfo.CompleteBy;
            LastAccessed = delegateInfo.LastAccessed;
            LaunchCount = delegateInfo.LaunchCount;
            Progress = delegateInfo.Progress;
            SignedOff = delegateInfo.SignedOff;
            SubmittedDate = delegateInfo.SubmittedDate;
            RemovedDate = delegateInfo.RemovedDate;
            DelegateUserId = delegateInfo.DelegateUserId;
            Supervisors = delegateInfo.Supervisors;
            EnrolledByForename = delegateInfo.EnrolledByForename;
            EnrolledBySurname = delegateInfo.EnrolledBySurname;
            EnrolledByAdminActive = delegateInfo.EnrolledByAdminActive;
            SelfAssessed = delegateInfo.SelfAssessed;
            Confirmed = delegateInfo.Confirmed;
            RegistrationAnswer1=  delegateInfo.RegistrationAnswer1;
            RegistrationAnswer2 = delegateInfo.RegistrationAnswer2;
            RegistrationAnswer3 = delegateInfo.RegistrationAnswer3;
            RegistrationAnswer4 = delegateInfo.RegistrationAnswer4;
            RegistrationAnswer5 = delegateInfo.RegistrationAnswer5;
            RegistrationAnswer6 = delegateInfo.RegistrationAnswer6;
            CandidateAssessmentsId = delegateInfo.CandidateAssessmentsId;
            SupervisorSelfAssessmentReview = delegateInfo.SupervisorSelfAssessmentReview;
        SupervisorResultsReview = delegateInfo.SupervisorResultsReview;
    }
        public int DelegateId { get; set; }
        public int CandidateAssessmentsId { get; set; }
        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? DelegateEmail { get; set; }
        public bool IsDelegateActive { get; set; }
        public int SelfAssessmentId { get; set; }
        public string? CandidateNumber { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public DateTime StartedDate { get; set; }
        public int EnrolmentMethodId { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int LaunchCount { get; set; }
        public string? Progress { get; set; }
        public DateTime? SignedOff { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
        public int DelegateUserId { get; set; }
        public string? EnrolledByForename { get; set; }
        public string? EnrolledBySurname { get; set; }
        public bool? EnrolledByAdminActive { get; set; }
        public int SelfAssessed { get; set; }
        public int Confirmed { get; set; }
        public string? RegistrationAnswer1 { get; set; }
        public string? RegistrationAnswer2 { get; set; }
        public string? RegistrationAnswer3 { get; set; }
        public string? RegistrationAnswer4 { get; set; }
        public string? RegistrationAnswer5 { get; set; }
        public string? RegistrationAnswer6 { get; set; }
        public bool SupervisorSelfAssessmentReview { get; set; }
        public bool SupervisorResultsReview { get; set; }
        public bool Removed => RemovedDate.HasValue;
        public string?[] DelegateRegistrationPrompts =>
            new[]
            {
                RegistrationAnswer1,
                RegistrationAnswer2,
                RegistrationAnswer3,
                RegistrationAnswer4,
                RegistrationAnswer5,
                RegistrationAnswer6,
            };
        public List<SelfAssessmentSupervisor> Supervisors { get; set; } =
            new List<SelfAssessmentSupervisor>();

        public string FullNameForSearchingSorting =>
            NameQueryHelper.GetSortableFullName(DelegateFirstName, DelegateLastName);
        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? FullNameForSearchingSorting;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
        public override string?[] SearchableContent => new[] { SearchableName, DelegateEmail, CandidateNumber };
    }
}
