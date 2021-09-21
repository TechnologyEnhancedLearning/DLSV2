namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using FluentAssertions;
    using NUnit.Framework;

    public class EditFilterViewModelTests
    {
        [Test]
        public void No_start_date_on_filter_selected_triggers_validation_error()
        {
            // Given
            var viewModel = new EditFiltersViewModel();
            const string expectedErrorMessage = "Start date is required";

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().HaveCount(3);
            result.First().ErrorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }

        [Test]
        public void Missing_end_date_doesnt_trigger_validation_when_EndDate_bool_is_false()
        {
            // Given
            var viewModel = new EditFiltersViewModel
            {
                StartDay = 1,
                StartMonth = 1,
                StartYear = 2021,
                EndDate = false
            };

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void Missing_end_date_triggers_validation_when_EndDate_bool_is_true()
        {
            // Given
            var viewModel = new EditFiltersViewModel
            {
                StartDay = 1,
                StartMonth = 1,
                StartYear = 2021,
                EndDate = true
            };
            const string expectedErrorMessage = "End date is required";

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().HaveCount(3);
            result.First().ErrorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }

        [TestCase(1, 1, 2021, 1, 1, 2020)]
        [TestCase(1, 2, 2021, 1, 1, 2021)]
        [TestCase(2, 1, 2021, 1, 1, 2021)]
        public void End_date_before_start_triggers_validation_when_EndDate_bool_is_true(
            int startDay,
            int startMonth,
            int startYear,
            int endDay,
            int endMonth,
            int endYear
        )
        {
            // Given
            var viewModel = new EditFiltersViewModel
            {
                StartDay = startDay,
                StartMonth = startMonth,
                StartYear = startYear,
                EndDay = endDay,
                EndMonth = endMonth,
                EndYear = endYear,
                EndDate = true
            };
            var expectedFirstError = new ValidationResult("End date must not precede start date", new[] { "StartDay" });
            var expectedSecondError = new ValidationResult(
                "",
                new[] { "StartMonth", "StartYear", "EndDay", "EndMonth", "EndYear" }
            );

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().HaveCount(2);
            result.First().Should().BeEquivalentTo(expectedFirstError);
            result.Last().Should().BeEquivalentTo(expectedSecondError);
        }

        [TestCase(1, 1, 2021, 1, 1, 2020)]
        [TestCase(1, 2, 2021, 1, 1, 2021)]
        [TestCase(2, 1, 2021, 1, 1, 2021)]
        public void End_date_before_start_doesnt_trigger_validation_when_EndDate_bool_is_false(
            int startDay,
            int startMonth,
            int startYear,
            int endDay,
            int endMonth,
            int endYear
        )
        {
            // Given
            var viewModel = new EditFiltersViewModel
            {
                StartDay = startDay,
                StartMonth = startMonth,
                StartYear = startYear,
                EndDay = endDay,
                EndMonth = endMonth,
                EndYear = endYear,
                EndDate = false
            };

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().BeEmpty();
        }

        [Test]
        public void CustomisationId_does_not_have_value_when_FilterType_is_not_course()
        {
            // Given
            var model = new EditFiltersViewModel
            {
                CustomisationId = 1,
                FilterType = CourseFilterType.None
            };

            // Then
            model.CustomisationId.Should().BeNull();
        }

        [Test]
        public void CourseCategoryId_does_not_have_value_when_FilterType_is_not_course_category()
        {
            // Given
            var model = new EditFiltersViewModel
            {
                CourseCategoryId = 1,
                FilterType = CourseFilterType.None
            };

            // Then
            model.CustomisationId.Should().BeNull();
        }
    }
}
