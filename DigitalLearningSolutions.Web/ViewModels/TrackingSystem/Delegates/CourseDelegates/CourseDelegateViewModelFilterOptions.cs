namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class CourseDelegateViewModelFilterOptions
    {
        public static readonly IEnumerable<FilterOptionViewModel> ActiveStatusOptions = new[]
        {
            CourseDelegateAccountStatusFilterOptions.Inactive,
            CourseDelegateAccountStatusFilterOptions.Active,
        };

        public static readonly IEnumerable<FilterOptionViewModel> LockedStatusOptions = new[]
        {
            CourseDelegateProgressLockedFilterOptions.Locked,
            CourseDelegateProgressLockedFilterOptions.NotLocked,
        };

        public static readonly IEnumerable<FilterOptionViewModel> RemovedStatusOptions = new[]
        {
            CourseDelegateProgressRemovedFilterOptions.Removed,
            CourseDelegateProgressRemovedFilterOptions.NotRemoved,
        };

        public static List<FilterViewModel> GetAllCourseDelegatesFilterViewModels(IEnumerable<CourseAdminField> adminFields)
        {
            var filters = new List<FilterViewModel>
            {
                new FilterViewModel("ActiveStatus", "Active Status", ActiveStatusOptions),
                new FilterViewModel("LockedStatus", "Locked Status", LockedStatusOptions),
                new FilterViewModel("RemovedStatus", "Removed Status", RemovedStatusOptions),
            };
            filters.AddRange(
                adminFields.Select(
                    field => new FilterViewModel(
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

        private static IEnumerable<FilterOptionViewModel> GetCourseDelegateAdminFieldOptions(CourseAdminField adminField)
        {
            if (adminField.Options.Count > 0)
            {
                return DelegatesViewModelFilters.GetPromptOptions(adminField);
            }

            var filterValueName =
                AdminFieldsHelper.GetAdminFieldAnswerName(adminField.PromptNumber);

            var options = new List<FilterOptionViewModel>
            {
                new FilterOptionViewModel(
                    "Not blank",
                    FilteringHelper.BuildFilterValueString(
                        filterValueName,
                        filterValueName,
                        FilteringHelper.FreeTextNotBlankValue
                    ),
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
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
