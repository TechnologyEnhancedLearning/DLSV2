namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseStatisticsViewModelFilterOptionsTests
    {
        private readonly FilterViewModel expectedCategoriesFilterViewModel = new FilterViewModel(
            "CategoryName",
            "Category",
            new[]
            {
                new FilterOptionViewModel(
                    "Category 1",
                    "CategoryName" + FilteringHelper.Separator + "CategoryName" + FilteringHelper.Separator +
                    "Category 1",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "Category 2",
                    "CategoryName" + FilteringHelper.Separator + "CategoryName" + FilteringHelper.Separator +
                    "Category 2",
                    FilterStatus.Default
                ),
            }
        );

        private readonly FilterViewModel expectedStatusFilterViewModel = new FilterViewModel(
            "Active",
            "Status",
            new[]
            {
                new FilterOptionViewModel(
                    "Inactive",
                    "Status" + FilteringHelper.Separator + "Active" + FilteringHelper.Separator +
                    "false",
                    FilterStatus.Warning
                ),
                new FilterOptionViewModel(
                    "Active",
                    "Status" + FilteringHelper.Separator + "Active" + FilteringHelper.Separator +
                    "true",
                    FilterStatus.Success
                ),
            }
        );

        private readonly FilterViewModel expectedTopicsFilterViewModel = new FilterViewModel(
            "CourseTopic",
            "Topic",
            new[]
            {
                new FilterOptionViewModel(
                    "Topic 1",
                    "CourseTopic" + FilteringHelper.Separator + "CourseTopic" + FilteringHelper.Separator +
                    "Topic 1",
                    FilterStatus.Default
                ),
                new FilterOptionViewModel(
                    "Topic 2",
                    "CourseTopic" + FilteringHelper.Separator + "CourseTopic" + FilteringHelper.Separator +
                    "Topic 2",
                    FilterStatus.Default
                ),
            }
        );

        private readonly FilterViewModel expectedVisibilityFilterViewModel = new FilterViewModel(
            "HideInLearnerPortal",
            "Visibility",
            new[]
            {
                new FilterOptionViewModel(
                    "Hidden in Learning Portal",
                    "Visibility" + FilteringHelper.Separator + "HideInLearnerPortal" + FilteringHelper.Separator +
                    "true",
                    FilterStatus.Warning
                ),
                new FilterOptionViewModel(
                    "Visible in Learning Portal",
                    "Visibility" + FilteringHelper.Separator + "HideInLearnerPortal" + FilteringHelper.Separator +
                    "false",
                    FilterStatus.Success
                ),
            }
        );

        private readonly List<string> filterableCategories = new List<string> { "Category 1", "Category 2" };

        private readonly List<string> filterableTopics = new List<string> { "Topic 1", "Topic 2" };

        [Test]
        public void GetFilterOptions_correctly_sets_up_filters()
        {
            // When
            var result =
                CourseStatisticsViewModelFilterOptions.GetFilterOptions(filterableCategories, filterableTopics);

            // Then
            result.Should().BeEquivalentTo(
                new List<FilterViewModel>
                {
                    expectedCategoriesFilterViewModel,
                    expectedTopicsFilterViewModel,
                    expectedStatusFilterViewModel,
                    expectedVisibilityFilterViewModel,
                }
            );
        }
    }
}
