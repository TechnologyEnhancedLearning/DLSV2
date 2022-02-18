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
            IsCourseActive = details.DelegateCourseInfo.IsCourseActive;
            Supervisor = details.DelegateCourseInfo.SupervisorAdminId != null
                ? DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    details.DelegateCourseInfo.SupervisorForename,
                    details.DelegateCourseInfo.SupervisorSurname,
                    details.DelegateCourseInfo.SupervisorAdminActive!.Value
                )
                : "None";
            EnrolmentMethodId = details.DelegateCourseInfo.EnrolmentMethodId;
            EnrolledByFullName = DisplayStringHelper.GetPotentiallyInactiveAdminName(
                details.DelegateCourseInfo.EnrolledByForename,
                details.DelegateCourseInfo.EnrolledBySurname,
                details.DelegateCourseInfo.EnrolledByAdminActive
            );
            RemovedDate = details.DelegateCourseInfo.RemovedDate?.ToString(DateHelper.StandardDateFormat);
            DelegateId = details.DelegateCourseInfo.DelegateId;
            var delegateFullName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                details.DelegateCourseInfo.DelegateFirstName,
                details.DelegateCourseInfo.DelegateLastName
            );
            DelegateNameAndEmail = DisplayStringHelper.GetNameWithEmailForDisplay(
                delegateFullName,
                details.DelegateCourseInfo.DelegateEmail
            );
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
        public bool IsCourseActive { get; set; }
        public string? RemovedDate { get; set; }
        public int EnrolmentMethodId { get; set; }
        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; }
        public string DelegateNameAndEmail { get; set; }
        public string? EnrolledByFullName { get; set; }

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
