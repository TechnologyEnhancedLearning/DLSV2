namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using DateHelper = Helpers.DateHelper;

    public class DelegateProgressViewModel : DelegateCourseInfoViewModel
    {
        public DelegateProgressViewModel(
            DelegateAccessRoute accessedVia,
            DelegateCourseDetails details,
            int? page
        ) : base(details)
        {
            AccessedVia = accessedVia;
            IsCourseActive = details.DelegateCourseInfo.IsCourseActive;
            ProfessionalRegistrationNumber = DisplayStringHelper.GetPrnDisplayString(
                details.DelegateCourseInfo.HasBeenPromptedForPrn,
                details.DelegateCourseInfo.ProfessionalRegistrationNumber
            );
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
            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                details.DelegateCourseInfo.DelegateFirstName,
                details.DelegateCourseInfo.DelegateLastName
            );
            DelegateEmail = details.DelegateCourseInfo.DelegateEmail;
            AdminFields = details.CourseAdminFields.Select(
                    cp =>
                        new DelegateCourseAdminField(
                            cp.PromptNumber,
                            cp.PromptText,
                            cp.Answer
                        )
                )
                .ToList();
            Page = page;
        }

        public DelegateAccessRoute AccessedVia { get; set; }
        public bool IsCourseActive { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public string? RemovedDate { get; set; }
        public int EnrolmentMethodId { get; set; }
        public IEnumerable<DelegateCourseAdminField> AdminFields { get; set; }
        public string DelegateName { get; set; }
        public string? DelegateEmail { get; set; }
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
