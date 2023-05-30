﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FluentAssertions;
    using NUnit.Framework;

    public class CourseStatisticsViewModelFilterOptionsTests
    {
        private readonly FilterModel expectedCategoriesFilterViewModel = new FilterModel(
            "CategoryName",
            "Category",
            new[]
            {
                new FilterOptionModel(
                    "Category 1",
                    "CategoryName" + FilteringHelper.Separator + "CategoryName" + FilteringHelper.Separator +
                    "Category 1",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Category 2",
                    "CategoryName" + FilteringHelper.Separator + "CategoryName" + FilteringHelper.Separator +
                    "Category 2",
                    FilterStatus.Default
                ),
            }
        );

        private readonly FilterModel expectedStatusFilterViewModel = new FilterModel(
            "Active",
            "Status",
            new[]
            {
                new FilterOptionModel(
                    "Inactive",
                    "Status" + FilteringHelper.Separator + "Active" + FilteringHelper.Separator +
                    "false",
                    FilterStatus.Warning
                ),
                new FilterOptionModel(
                    "Active",
                    "Status" + FilteringHelper.Separator + "Active" + FilteringHelper.Separator +
                    "true",
                    FilterStatus.Success
                ),
                new FilterOptionModel(
                    "Archived",
                    "Status" + FilteringHelper.Separator + "Archived" + FilteringHelper.Separator +
                    "true",
                    FilterStatus.Default
                ),
            }
        );

        private readonly FilterModel expectedTopicsFilterViewModel = new FilterModel(
            "CourseTopic",
            "Topic",
            new[]
            {
                new FilterOptionModel(
                    "Topic 1",
                    "CourseTopic" + FilteringHelper.Separator + "CourseTopic" + FilteringHelper.Separator +
                    "Topic 1",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Topic 2",
                    "CourseTopic" + FilteringHelper.Separator + "CourseTopic" + FilteringHelper.Separator +
                    "Topic 2",
                    FilterStatus.Default
                ),
            }
        );

        private readonly FilterModel expectedVisibilityFilterViewModel = new FilterModel(
            "HideInLearnerPortal",
            "Visibility",
            new[]
            {
                new FilterOptionModel(
                    "Hidden in Learning Portal",
                    "Visibility" + FilteringHelper.Separator + "HideInLearnerPortal" + FilteringHelper.Separator +
                    "true",
                    FilterStatus.Warning
                ),
                new FilterOptionModel(
                    "Visible in Learning Portal",
                    "Visibility" + FilteringHelper.Separator + "HideInLearnerPortal" + FilteringHelper.Separator +
                    "false",
                    FilterStatus.Success
                ),
            }
        );

        private readonly FilterModel expectedHasAdminFieldsFilterViewModel = new FilterModel(
            "HasAdminFields",
            "Admin fields",
            new[]
            {
                new FilterOptionModel(
                    "Has admin fields",
                    "HasAdminFields" + FilteringHelper.Separator + "HasAdminFields" + FilteringHelper.Separator +
                    "true",
                    FilterStatus.Default
                ),
                new FilterOptionModel(
                    "Doesn't have admin fields",
                    "HasAdminFields" + FilteringHelper.Separator + "HasAdminFields" + FilteringHelper.Separator +
                    "false",
                    FilterStatus.Default
                )
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
                new List<FilterModel>
                {
                    expectedCategoriesFilterViewModel,
                    expectedTopicsFilterViewModel,
                    expectedStatusFilterViewModel,
                    expectedVisibilityFilterViewModel,
                    expectedHasAdminFieldsFilterViewModel,
                }
            );
        }

        [Test]
        public void GetFilterOptions_excludes_category_option_if_passed_no_categories()
        {
            // When
            var result =
                CourseStatisticsViewModelFilterOptions.GetFilterOptions(new string[] { }, filterableTopics);

            // Then
            result.Should().BeEquivalentTo(
                new List<FilterModel>
                {
                    expectedTopicsFilterViewModel,
                    expectedStatusFilterViewModel,
                    expectedVisibilityFilterViewModel,
                    expectedHasAdminFieldsFilterViewModel,
                }
            );
        }
    }
}
