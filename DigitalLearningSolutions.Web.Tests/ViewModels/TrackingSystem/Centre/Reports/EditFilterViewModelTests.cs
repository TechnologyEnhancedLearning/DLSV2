namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
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
            const string expectedErrorMessage = "Enter a Start Date";

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().HaveCount(3);
            result.First().ErrorMessage.Should().BeEquivalentTo(expectedErrorMessage);
        }

        [Test]
        public void Start_date_before_data_start_triggers_validation_error()
        {
            // Given
            var viewModel = new EditFiltersViewModel
            {
                StartDay = 1,
                StartMonth = 1,
                StartYear = 2021,
                EndDate = false,
                DataStart = DateTime.Parse("2222-2-2"),
            };
            const string expectedErrorMessage = "Enter a start date after the start of data for this centre";

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().HaveCount(2);
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
                EndDate = false,
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
                EndDate = true,
            };
            const string expectedErrorMessage = "Enter an End Date";

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
                EndDate = true,
            };
            var expectedFirstError = new ValidationResult("Enter an end date after the start date", new[] { "EndDay" });
            var expectedSecondError = new ValidationResult(
                "",
                new[] { "EndMonth", "EndYear" }
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
                EndDate = false,
            };

            // When
            var result = viewModel.Validate(new ValidationContext(viewModel)).ToList();

            // Then
            result.Should().BeEmpty();
        }
    }
}
