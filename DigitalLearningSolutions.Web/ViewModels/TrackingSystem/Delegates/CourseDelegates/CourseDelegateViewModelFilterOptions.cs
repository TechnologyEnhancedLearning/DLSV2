namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

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

        public static List<FilterModel> GetAllCourseDelegatesFilterViewModels(IEnumerable<CourseAdminField> adminFields)
        {
            var filters = new List<FilterModel>
            {
                new FilterModel("ActiveStatus", "Active Status", ActiveStatusOptions),
                new FilterModel("LockedStatus", "Locked Status", LockedStatusOptions),
                new FilterModel("RemovedStatus", "Removed Status", RemovedStatusOptions),
            };
            filters.AddRange(
                adminFields.Select(
                    field => new FilterModel(
                        $"CourseAdminField{field.PromptNumber}",
                        field.PromptText,
                        GetCourseDelegateAdminFieldOptions(field)
                    )
                )
            );
            return filters;
        }

        public static Dictionary<int, string> GetAdminFieldFilters(
            IEnumerable<DelegateCourseAdminField> adminFields,
            IEnumerable<CourseAdminField> fieldsWithOptions
        )
        {
            var fieldsWithOptionsIds = fieldsWithOptions.Select(c => c.PromptNumber);
            return adminFields
                .Select(
                    adminField => new KeyValuePair<int, string>(
                        adminField.PromptNumber,
                        GetFilterValueForAdminField(adminField, fieldsWithOptionsIds.Contains(adminField.PromptNumber))
                    )
                ).ToDictionary(x => x.Key, x => x.Value);
        }

        private static string GetFilterValueForAdminField(DelegateCourseAdminField delegateCourseAdminField, bool adminFieldHasOptions)
        {
            var filterValueName =
                AdminFieldsHelper.GetAdminFieldAnswerName(delegateCourseAdminField.PromptNumber);

            string propertyValue;

            if (adminFieldHasOptions)
            {
                propertyValue = string.IsNullOrEmpty(delegateCourseAdminField.Answer)
                    ? FilteringHelper.EmptyValue
                    : delegateCourseAdminField.Answer;
            }
            else
            {
                propertyValue = string.IsNullOrEmpty(delegateCourseAdminField.Answer)
                    ? FilteringHelper.FreeTextBlankValue
                    : FilteringHelper.FreeTextNotBlankValue;
            }

            return FilteringHelper.BuildFilterValueString(filterValueName, filterValueName, propertyValue);
        }

        private static IEnumerable<FilterOptionModel> GetCourseDelegateAdminFieldOptions(CourseAdminField adminField)
        {
            if (adminField.Options.Count > 0)
            {
                return DelegatesViewModelFilters.GetPromptOptions(adminField);
            }

            var filterValueName =
                AdminFieldsHelper.GetAdminFieldAnswerName(adminField.PromptNumber);

            var options = new List<FilterOptionModel>
            {
                new FilterOptionModel(
                    "Not blank",
                    FilteringHelper.BuildFilterValueString(
                        filterValueName,
                        filterValueName,
                        FilteringHelper.FreeTextNotBlankValue
                    ),
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Blank",
                    FilteringHelper.BuildFilterValueString(
                        filterValueName,
                        filterValueName,
                        FilteringHelper.FreeTextBlankValue
                    ),
                    FilterStatus.Default
                ),
            };
            return options;
        }
    }
}
