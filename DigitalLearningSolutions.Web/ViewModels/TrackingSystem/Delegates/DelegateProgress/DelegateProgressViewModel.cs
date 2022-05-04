namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using DateHelper = DigitalLearningSolutions.Web.Helpers.DateHelper;

    public class DelegateProgressViewModel : DelegateCourseInfoViewModel
    {
        public DelegateProgressViewModel(
            DelegateAccessRoute accessedVia,
            DetailedCourseProgress details,
            string currentSystemBaseUrl,
            ReturnPageQuery? returnPageQuery = null
        ) : base(details, accessedVia, returnPageQuery)
        {
            DelegateName = DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                details.DelegateCourseInfo.DelegateFirstName,
                details.DelegateCourseInfo.DelegateLastName
            );
            DelegateEmail = details.DelegateCourseInfo.DelegateEmail;
            DelegateNumber = details.DelegateCourseInfo.DelegateNumber;
            ProfessionalRegistrationNumber = PrnStringHelper.GetPrnDisplayString(
                details.DelegateCourseInfo.HasBeenPromptedForPrn,
                details.DelegateCourseInfo.ProfessionalRegistrationNumber
            );

            IsCourseActive = details.DelegateCourseInfo.IsCourseActive;

            RemovedDate = details.DelegateCourseInfo.RemovedDate?.ToString(DateHelper.StandardDateFormat);

            AdminFields = details.CourseAdminFields.Select(
                    cp =>
                        new DelegateCourseAdminField(
                            cp.PromptNumber,
                            cp.PromptText,
                            cp.Answer
                        )
                )
                .ToList();

            ProgressDownloadUrl = currentSystemBaseUrl + $"/tracking/summary?ProgressID={details.ProgressId}";

            Sections = details.Sections.Select(s => new SectionProgressViewModel(s));
        }

        public string DelegateName { get; set; }
        public string? DelegateEmail { get; set; }
        public string DelegateNumber { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }

        public bool IsCourseActive { get; set; }

        public string? RemovedDate { get; set; }
        public IEnumerable<DelegateCourseAdminField> AdminFields { get; set; }

        public string ProgressDownloadUrl { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }
}
