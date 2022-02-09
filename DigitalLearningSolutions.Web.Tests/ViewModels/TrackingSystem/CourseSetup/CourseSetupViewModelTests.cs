namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CourseSetupViewModelTests
    {
        private readonly IEnumerable<CourseStatistics> courses = new List<CourseStatistics>
        {
            new CourseStatistics { ApplicationName = "A" },
            new CourseStatistics { ApplicationName = "B" },
            new CourseStatistics { ApplicationName = "C" },
            new CourseStatistics { ApplicationName = "D" },
            new CourseStatistics { ApplicationName = "E" },
            new CourseStatistics { ApplicationName = "F" },
            new CourseStatistics { ApplicationName = "G" },
            new CourseStatistics { ApplicationName = "H" },
            new CourseStatistics { ApplicationName = "I" },
            new CourseStatistics { ApplicationName = "J" },
            new CourseStatistics { ApplicationName = "K" },
            new CourseStatistics { ApplicationName = "L" },
            new CourseStatistics { ApplicationName = "M" },
            new CourseStatistics { ApplicationName = "N" },
            new CourseStatistics { ApplicationName = "O" },
        };

        [Test]
        public void CourseSetupViewModel_should_default_to_returning_the_first_ten_delegates()
        {
            // When
            var model = new CourseSetupViewModel(
                courses,
                new List<string>(),
                new List<string>(),
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
        public void CourseSetupViewModel_should_correctly_return_the_second_page_of_delegates()
        {
            // When
            var model = new CourseSetupViewModel(
                courses,
                new List<string>(),
                new List<string>(),
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
        public void CourseSetupViewModel_filters_should_be_set()
        {
            // Given
            var categories = new[]
            {
                "Category 1",
                "Category 2",
            };
            var topics = new[]
            {
                "Topic 1",
                "Topic 2",
            };

            var expectedFilters = CourseStatisticsViewModelFilterOptions.GetFilterOptions(categories, topics);

            // When
            var model = new CourseSetupViewModel(
                courses,
                categories,
                topics,
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
        public void CourseSetupViewModel_with_custom_items_per_page_should_return_the_specified_number_of_delegates()
        {
            // When
            const int itemsPerPage = 12;
            var model = new CourseSetupViewModel(
                courses,
                new List<string>(),
                new List<string>(),
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
