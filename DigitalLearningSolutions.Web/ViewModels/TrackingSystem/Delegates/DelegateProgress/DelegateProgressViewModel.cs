namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class DelegateProgressViewModel : DelegateCourseInfoViewModel
    {
        public DelegateProgressViewModel(
            DelegateAccessRoute accessedVia,
            DetailedCourseProgress details,
            string currentSystemBaseUrl,
            ReturnPageQuery? returnPageQuery = null
        ) : base(details, accessedVia, returnPageQuery)
        {
            IsCourseActive = details.DelegateCourseInfo.IsCourseActive;

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

        public bool IsCourseActive { get; set; }

        public IEnumerable<DelegateCourseAdminField> AdminFields { get; set; }

        public string ProgressDownloadUrl { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }
}
