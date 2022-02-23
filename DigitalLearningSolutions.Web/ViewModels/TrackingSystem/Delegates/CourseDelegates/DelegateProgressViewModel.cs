namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;

    public class DelegateProgressViewModel : DelegateCourseInfoViewModel
    {
        public DelegateProgressViewModel(
            DelegateProgressAccessRoute accessedVia,
            DelegateCourseDetails details,
            int? page
        ) : base(details)
        {
            AccessedVia = accessedVia;
            CustomisationCentreId = details.DelegateCourseInfo.CustomisationCentreId;
            IsCourseActive = details.DelegateCourseInfo.IsCourseActive;
            AllCentresCourse = details.DelegateCourseInfo.AllCentresCourse;
            CourseCategoryId = details.DelegateCourseInfo.CourseCategoryId;
            SupervisorAdminId = details.DelegateCourseInfo.SupervisorAdminId;
            EnrolmentMethodId = details.DelegateCourseInfo.EnrolmentMethodId;
            EnrolledByAdminId = details.DelegateCourseInfo.EnrolledByAdminId;
            EnrolledByForename = details.DelegateCourseInfo.EnrolledByForename;
            EnrolledBySurname = details.DelegateCourseInfo.EnrolledBySurname;
            RemovedDate = details.DelegateCourseInfo.RemovedDate?.ToString(DateHelper.StandardDateFormat);
            DelegateId = details.DelegateCourseInfo.DelegateId;
            DelegateFirstName = details.DelegateCourseInfo.DelegateFirstName;
            DelegateLastName = details.DelegateCourseInfo.DelegateLastName;
            DelegateEmail = details.DelegateCourseInfo.DelegateEmail;
            DelegateCentreId = details.DelegateCourseInfo.DelegateCentreId;
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
            Page = page;
        }

        public DelegateProgressAccessRoute AccessedVia { get; set; }
        public int CustomisationCentreId { get; set; }
        public bool IsCourseActive { get; set; }
        public bool AllCentresCourse { get; set; }
        public int CourseCategoryId { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? RemovedDate { get; set; }
        public int EnrolmentMethodId { get; set; }
        public int? EnrolledByAdminId { get; set; }
        public string? EnrolledByForename { get; set; }
        public string? EnrolledBySurname { get; set; }
        public int DelegateId { get; set; }
        public string? DelegateFirstName { get; set; }
        public string DelegateLastName { get; set; }
        public string? DelegateEmail { get; set; }
        public int DelegateCentreId { get; set; }
        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; }

        public string DelegateFullName =>
            DelegateFirstName == null ? DelegateLastName : $"{DelegateFirstName} {DelegateLastName}";

        public string DelegateNameAndEmail =>
            string.IsNullOrWhiteSpace(DelegateEmail) ? DelegateFullName : $"{DelegateFullName} ({DelegateEmail})";

        public string? EnrolledByFullName =>
            EnrolledByAdminId == null ? null : $"{EnrolledByForename} {EnrolledBySurname}";

        public string SupervisorFullName =>
            SupervisorAdminId == null ? "None" : $"{SupervisorForename} {SupervisorSurname}";

        public new string EnrolmentMethod
        {
            get
            {
                return EnrolmentMethodId switch
                {
                    1 => "Self",
                    2 => "Enrolled by " + EnrolledByFullName,
                    3 => "Group",
                    _ => "System",
                };
            }
        }

        public int? Page { get; set; }
    }
}
