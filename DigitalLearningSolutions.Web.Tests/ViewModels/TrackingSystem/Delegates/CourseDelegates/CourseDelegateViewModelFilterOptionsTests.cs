namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseDelegateViewModelFilterOptionsTests
    {
        [Test]
        public void GetAllCourseDelegatesFilterViewModels_should_return_correct_admin_field_filters()
        {
            // Given
            var (adminFields, expectedFilters) = GetSampleAdminFieldsAndFilters();

            // When
            var result = CourseDelegateViewModelFilterOptions.GetAllCourseDelegatesFilterViewModels(
                adminFields
            );

            // Then
            expectedFilters.ForEach(expectedFilter => result.Should().ContainEquivalentOf(expectedFilter));
        }

        private static (List<CourseAdminField> adminFields, List<FilterModel> filters) GetSampleAdminFieldsAndFilters()
        {
            var adminField1 = PromptsTestHelper.GetDefaultCourseAdminField(
                1,
                "System access",
                "Yes\r\nNo"
            );
            var adminField3 = PromptsTestHelper.GetDefaultCourseAdminField(3, "Some Free Text Field");
            var adminFields = new List<CourseAdminField> { adminField1, adminField3 };

            var adminField1Options = new[]
            {
                new FilterOptionModel(
                    "Yes",
                    "Answer1(System access)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Yes",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "No",
                    "Answer1(System access)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "No",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "No option selected",
                    "Answer1(System access)" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                ),
            };
            var adminField3Options = new[]
            {
                new FilterOptionModel(
                    "Not blank",
                    "Answer3(Some Free Text Field)" + FilteringHelper.Separator +
                    "Answer3" + FilteringHelper.Separator + FilteringHelper.FreeTextNotBlankValue,
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Blank",
                    "Answer3(Some Free Text Field)" + FilteringHelper.Separator +
                    "Answer3" + FilteringHelper.Separator + FilteringHelper.FreeTextBlankValue,
                    FilterStatus.Default
                ),
            };
            var adminFieldFilters = new List<FilterModel>
            {
                new FilterModel("CourseAdminField1", "System access", adminField1Options),
                new FilterModel("CourseAdminField3", "Some Free Text Field", adminField3Options),
            };

            return (adminFields, adminFieldFilters);
        }
    }
}
