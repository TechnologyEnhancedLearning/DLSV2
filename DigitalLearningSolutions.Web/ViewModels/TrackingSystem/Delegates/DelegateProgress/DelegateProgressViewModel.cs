namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Progress;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;
    using Microsoft.Extensions.Configuration;

    public class DelegateProgressViewModel : DelegateCourseInfoViewModel
    {
        public DelegateProgressViewModel(
            DelegateAccessRoute accessedVia,
            DetailedCourseProgress progress,
            IConfiguration config,
            ReturnPageQuery? returnPageQuery = null
        ) : base(progress, accessedVia, returnPageQuery)
        {
            IsCourseActive = progress.IsCourseActive;

            AdminFields = progress.CourseAdminFields.Select(
                    cp =>
                        new DelegateCourseAdminField(
                            cp.PromptNumber,
                            cp.PromptText,
                            cp.Answer
                        )
                )
                .ToList();

            ProgressDownloadUrl = OldSystemEndpointHelper.GetDownloadSummaryUrl(config, progress.ProgressId); ;

            Sections = progress.Sections.Select(s => new SectionProgressViewModel(s));
        }

        public bool IsCourseActive { get; set; }

        public IEnumerable<DelegateCourseAdminField> AdminFields { get; set; }

        public string ProgressDownloadUrl { get; set; }

        public IEnumerable<SectionProgressViewModel> Sections { get; set; }
    }
}
