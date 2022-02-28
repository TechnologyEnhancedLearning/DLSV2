namespace DigitalLearningSolutions.Web.Tests.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SearchablePageViewModelTests
    {
        private CentreCourseDetails details = null!;

        [SetUp]
        public void Setup()
        {
            details = Builder<CentreCourseDetails>.CreateNew()
                .With(x => x.Courses = new List<CourseStatisticsWithAdminFieldResponseCounts>
                {
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "A" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "B" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "C" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "D" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "E" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "F" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "G" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "H" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "I" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "J" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "K" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "L" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "M" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "N" },
                    new CourseStatisticsWithAdminFieldResponseCounts { ApplicationName = "O" },
                })
                .And(x => x.Categories = new[] { "Category 1", "Category 2" })
                .And(x => x.Topics = new[] { "Topic 1", "Topic 2" })
                .Build();
        }

        [Test]
        public void SearchablePageViewModel_should_default_to_returning_the_first_ten_delegates()
        {
            // When
            var model = new CourseSetupViewModel(
                details,
                null,
                nameof(CourseStatistics.SearchableName),
                GenericSortingHelper.Ascending,
                null,
                1,
                null
            );

            // Then
            using (new AssertionScope())
            {
                model.Courses.Count().Should().Be(BasePaginatedViewModel.DefaultItemsPerPage);
                model.Courses.Any(c => c.CourseName == "K").Should()
                    .BeFalse();
            }
        }

        [Test]
        public void SearchablePageViewModel_should_correctly_return_the_second_page_of_delegates()
        {
            // When
            var model = new CourseSetupViewModel(
                details,
                null,
                nameof(CourseStatistics.SearchableName),
                GenericSortingHelper.Ascending,
                null,
                2,
                null
            );

            // Then
            using (new AssertionScope())
            {
                model.Courses.Count().Should().Be(5);
                model.Courses.First().CourseName.Should().BeEquivalentTo("K");
            }
        }

        [Test]
        public void SearchablePageViewModel_filters_should_be_set()
        {
            var expectedFilters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(details.Categories, details.Topics);

            // When
            var model = new CourseSetupViewModel(
                details,
                null,
                nameof(CourseStatistics.SearchableName),
                GenericSortingHelper.Ascending,
                null,
                2,
                null
            );

            // Then
            model.Filters.Should().BeEquivalentTo(expectedFilters);
            model.Courses.First().CourseName.Should().BeEquivalentTo("K");
        }

        [Test]
        public void SearchablePageViewModel_with_custom_items_per_page_should_return_the_specified_number_of_delegates()
        {
            // When
            const int itemsPerPage = 12;

            var model = new CourseSetupViewModel(
                details,
                null,
                nameof(CourseStatistics.SearchableName),
                GenericSortingHelper.Ascending,
                null,
                1,
                itemsPerPage
            );

            // Then
            using (new AssertionScope())
            {
                model.Courses.Count().Should().Be(itemsPerPage);
                model.Courses.Any(c => c.CourseName == "M").Should()
                    .BeFalse();
            }
        }
    }
}
