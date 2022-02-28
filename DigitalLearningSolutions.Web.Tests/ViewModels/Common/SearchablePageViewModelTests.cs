namespace DigitalLearningSolutions.Web.Tests.ViewModels.Common
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Tests.NBuilderHelpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SearchablePageViewModelTests
    {
        private readonly CentreCourseDetails details = Builder<CentreCourseDetails>.CreateNew()
            .With(x => x.Courses = Builder<CourseStatisticsWithAdminFieldResponseCounts>
                .CreateListOfSize(14)
                .All()
                .With((g, i) => g.ApplicationName = NBuilderAlphabeticalPropertyNamingHelper.IndexToAlphabeticalString(i))
                .Build().ToArray())
            .And(x => x.Categories = new[] { "Category 1", "Category 2" })
            .And(x => x.Topics = new[] { "Topic 1", "Topic 2" })
            .Build();

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
                model.Courses.Any(c => c.CourseName == "K - CustomisationName11").Should()
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
                model.Courses.Count().Should().Be(4);
                model.Courses.First().CourseName.Should().BeEquivalentTo("K - CustomisationName11");
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
            model.Courses.First().CourseName.Should().BeEquivalentTo("K - CustomisationName11");
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
