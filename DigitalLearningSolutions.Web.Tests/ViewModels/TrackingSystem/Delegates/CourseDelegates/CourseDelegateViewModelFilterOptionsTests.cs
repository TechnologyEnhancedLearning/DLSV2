namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.CourseDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
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

        private static (List<CustomPrompt> adminFields, List<FilterViewModel> filters) GetSampleAdminFieldsAndFilters()
        {
            var adminField1 = CustomPromptsTestHelper.GetDefaultCustomPrompt(
                1,
                "System access",
                "Yes\r\nNo"
            );
            var adminField3 = CustomPromptsTestHelper.GetDefaultCustomPrompt(3, "Some Free Text Field");
            var adminFields = new List<CustomPrompt> { adminField1, adminField3 };

            var adminField1Options = new[]
            {
                new FilterOptionViewModel(
                    "Yes",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "Yes",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "No",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + "No",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "No option selected",
                    "Answer1" + FilteringHelper.Separator +
                    "Answer1" + FilteringHelper.Separator + FilteringHelper.EmptyValue,
                    FilterStatus.Default
                ),
            };
            var adminField3Options = new[]
            {
                new FilterOptionViewModel(
                    "Not blank",
                    "Answer3" + FilteringHelper.Separator +
                    "Answer3" + FilteringHelper.Separator + FilteringHelper.FreeTextNotBlankValue,
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "Blank",
                    "Answer3" + FilteringHelper.Separator +
                    "Answer3" + FilteringHelper.Separator + FilteringHelper.FreeTextBlankValue,
                    FilterStatus.Default
                ),
            };
            var adminFieldFilters = new List<FilterViewModel>
            {
                new FilterViewModel("CustomPrompt1", "System access", adminField1Options),
                new FilterViewModel("CustomPrompt3", "Some Free Text Field", adminField3Options),
            };

            return (adminFields, adminFieldFilters);
        }
    }
}
