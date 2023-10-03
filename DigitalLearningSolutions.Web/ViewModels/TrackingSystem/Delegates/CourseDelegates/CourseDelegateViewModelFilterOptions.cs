namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class CourseDelegateViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionModel> ActiveStatusOptions = new[]
        {
            CourseDelegateAccountStatusFilterOptions.Inactive,
            CourseDelegateAccountStatusFilterOptions.Active,
        };

        public static readonly IEnumerable<FilterOptionModel> LockedStatusOptions = new[]
        {
            CourseDelegateProgressLockedFilterOptions.Locked,
            CourseDelegateProgressLockedFilterOptions.NotLocked,
        };

        public static readonly IEnumerable<FilterOptionModel> RemovedStatusOptions = new[]
        {
            CourseDelegateProgressRemovedFilterOptions.Removed,
            CourseDelegateProgressRemovedFilterOptions.NotRemoved,
        };

        public static readonly IEnumerable<FilterOptionModel> CompletionStatusOptions = new[]
        {
            CourseDelegateCompletionFilterOptions.Complete,
            CourseDelegateCompletionFilterOptions.Incomplete,
        };

        public static List<FilterModel> GetAllCourseDelegatesFilterViewModels(IEnumerable<CourseAdminField> adminFields)
        {
            var filters = new List<FilterModel>
            {
                new FilterModel("ActiveStatus", "Active Status", ActiveStatusOptions,"status"),
                new FilterModel("LockedStatus", "Locked Status", LockedStatusOptions, "status"),
                new FilterModel("RemovedStatus", "Removed Status", RemovedStatusOptions, "status"),
                new FilterModel("CompletionStatus", "Completion Status", CompletionStatusOptions, "status")
            };
            filters.AddRange(
                adminFields.Select(
                    field => new FilterModel(
                        $"CourseAdminField{field.PromptNumber}",
                        field.PromptText,
                        FilteringHelper.GetPromptFilterOptions(field),
                        "prompts"
                    )
                )
            );
            return filters;
        }
    }
}
