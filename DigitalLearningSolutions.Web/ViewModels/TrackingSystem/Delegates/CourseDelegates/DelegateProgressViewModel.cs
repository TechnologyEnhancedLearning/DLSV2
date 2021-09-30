namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class DelegateProgressViewModel
    {
        public DelegateProgressViewModel(
            DelegateCourseDetails details
        )
        {
            CustomisationId = details.DelegateCourseInfo.CustomisationId;
            CustomisationCentreId = details.DelegateCourseInfo.CustomisationCentreId;
            IsCourseActive = details.DelegateCourseInfo.IsCourseActive;
            AllCentresCourse = details.DelegateCourseInfo.AllCentresCourse;
            CourseCategoryId = details.DelegateCourseInfo.CourseCategoryId;
            CourseName = details.DelegateCourseInfo.CourseName;
            SupervisorAdminId = details.DelegateCourseInfo.SupervisorAdminId;
            SupervisorForename = details.DelegateCourseInfo.SupervisorForename;
            SupervisorSurname = details.DelegateCourseInfo.SupervisorSurname;
            Enrolled = details.DelegateCourseInfo.Enrolled;
            LastUpdated = details.DelegateCourseInfo.LastUpdated;
            CompleteBy = details.DelegateCourseInfo.CompleteBy;
            Completed = details.DelegateCourseInfo.Completed;
            Evaluated = details.DelegateCourseInfo.Evaluated;
            EnrolmentMethodId = details.DelegateCourseInfo.EnrolmentMethodId;
            EnrolledByAdminId = details.DelegateCourseInfo.EnrolledByAdminId;
            EnrolledByForename = details.DelegateCourseInfo.EnrolledByForename;
            EnrolledBySurname = details.DelegateCourseInfo.EnrolledBySurname;
            LoginCount = details.DelegateCourseInfo.LoginCount;
            LearningTime = details.DelegateCourseInfo.LearningTime;
            DiagnosticScore = details.DelegateCourseInfo.DiagnosticScore;
            IsAssessed = details.DelegateCourseInfo.IsAssessed;
            DelegateId = details.DelegateCourseInfo.DelegateId;
            DelegateFirstName = details.DelegateCourseInfo.DelegateFirstName;
            DelegateLastName = details.DelegateCourseInfo.DelegateLastName;
            DelegateEmail = details.DelegateCourseInfo.DelegateEmail;
            DelegateCentreId = details.DelegateCourseInfo.DelegateCentreId;
            AllAttempts = details.AttemptStats.totalAttempts;
            AttemptsPassed = details.AttemptStats.attemptsPassed;
            PassRate = details.AttemptStats.passRate;
            CustomFields = details.CustomPrompts.Select(
                    cp =>
                        new CustomFieldViewModel(
                            cp.CustomPromptNumber,
                            cp.CustomPromptText,
                            cp.Mandatory,
                            cp.Answer
                        )
                )
                .ToList();
        }

        public int CustomisationId { get; set; }
        public int CustomisationCentreId { get; set; }
        public bool IsCourseActive { get; set; }
        public bool AllCentresCourse { get; set; }
        public int CourseCategoryId { get; set; }
        public string CourseName { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? SupervisorForename { get; set; }
        public string? SupervisorSurname { get; set; }
        public DateTime Enrolled { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? CompleteBy { get; set; }
        public DateTime? Completed { get; set; }
        public DateTime? Evaluated { get; set; }
        public DateTime? RemovedDate { get; set; }
        public int EnrolmentMethodId { get; set; }
        public int? EnrolledByAdminId { get; set; }
        public string? EnrolledByForename { get; set; }
        public string? EnrolledBySurname { get; set; }
        public int LoginCount { get; set; }
        public int LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }
        public int DelegateId { get; set; }
        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? DelegateEmail { get; set; }
        public int DelegateCentreId { get; set; }
        public int AllAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public double PassRate { get; set; }
        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; }

        public string DelegateFullName =>
            DelegateFirstName == null ? DelegateLastName : $"{DelegateFirstName} {DelegateLastName}";

        public string DelegateNameAndEmail =>
            string.IsNullOrWhiteSpace(DelegateEmail) ? DelegateFullName : $"{DelegateFullName} ({DelegateEmail})";

        public string SupervisorFullName =>
            SupervisorAdminId == null ? "None" : $"{SupervisorForename} {SupervisorSurname}";

        public string? EnrolledByFullName =>
            EnrolledByAdminId == null ? null : $"{EnrolledByForename} {EnrolledBySurname}";

        public string EnrolmentMethod
        {
            get
            {
                return EnrolmentMethodId switch
                {
                    1 => "Self",
                    2 => "Enrolled by " + EnrolledByFullName,
                    3 => "Group",
                    _ => "System"
                };
            }
        }
    }
}
