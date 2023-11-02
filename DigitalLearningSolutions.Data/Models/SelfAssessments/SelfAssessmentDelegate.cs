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
        }
        public int DelegateId { get; set; }
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
        public bool Removed => RemovedDate.HasValue;

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
