﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class AllCourseDelegatesViewModel : BaseJavaScriptFilterableViewModel
    {
        public AllCourseDelegatesViewModel(IEnumerable<CourseDelegate> delegates, IList<CustomPrompt> adminFields)
        {
            Delegates = delegates.Select(
                d =>
                {
                    var adminFieldViewModels = AdminFieldsHelper.GetCourseAdminFieldViewModels(d, adminFields);
                    var adminFieldsWithOptions = adminFields.Where(field => field.Options.Count > 0);
                    return new SearchableCourseDelegateViewModel(d, adminFieldViewModels, adminFieldsWithOptions);
                }
            );
            Filters = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(adminFields)
                .SelectAppliedFilterViewModels();
        }

        public IEnumerable<SearchableCourseDelegateViewModel> Delegates { get; set; }
    }
}
