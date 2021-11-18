namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports;
    using FluentAssertions;
    using NUnit.Framework;

    public class ReportsViewModelTests
    {
        [Test]
        public void UsageStatsTableViewModel_reverses_data_in_time()
        {
            // Given
            var monthlyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2001-01-01"), ReportInterval.Months),
                    1,
                    1,
                    1
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-01"), ReportInterval.Months),
                    0,
                    0,
                    0
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                monthlyData,
                DateTime.Parse("2001-01-01"),
                DateTime.Parse("2002-02-01")
            );

            // Then
            model.Rows.First().Completions.Should().Be(0);
            model.Rows.Last().Completions.Should().Be(1);
        }

        [Test]
        public void ReportsFilterModel_correctly_formats_date_range()
        {
            // Given
            var filterData = new ActivityFilterData(
                DateTime.Parse("2001-01-01"),
                DateTime.Parse("2002-02-02"),
                null,
                null,
                null,
                CourseFilterType.None,
                ReportInterval.Years
            );

            // When
            var model = new ReportsFilterModel(filterData, "", "", "", false);

            // Then
            model.StartDate.Should().Be("01/01/2001");
            model.EndDate.Should().Be("02/02/2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_day_interval_string_correctly()
        {
            // Given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-01"), ReportInterval.Days),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-02"), ReportInterval.Days),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-03"), ReportInterval.Days),
                    null
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                dailyData,
                DateTime.Parse("2002-02-01"),
                DateTime.Parse("2002-02-03")
            );

            // Then
            model.Rows.ToList()[1].Period.Should().Be("2/02/2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_week_interval_string_correctly()
        {
            // Given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-01"), ReportInterval.Weeks),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-08"), ReportInterval.Weeks),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-15"), ReportInterval.Weeks),
                    null
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                dailyData,
                DateTime.Parse("2002-02-01"),
                DateTime.Parse("2002-02-15")
            );

            // Then
            model.Rows.ToList()[1].Period.Should().Be("Week commencing 8/02/2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_month_interval_string_correctly()
        {
            // Given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-01-01"), ReportInterval.Months),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-01"), ReportInterval.Months),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-03-01"), ReportInterval.Months),
                    null
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                dailyData,
                DateTime.Parse("2002-01-01"),
                DateTime.Parse("2002-03-02")
            );

            // Then
            model.Rows.ToList()[1].Period.Should().Be("February, 2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_quarter_interval_string_correctly()
        {
            // Given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-01-01"), ReportInterval.Quarters),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-04-01"), ReportInterval.Quarters),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-07-01"), ReportInterval.Quarters),
                    null
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                dailyData,
                DateTime.Parse("2002-02-02"),
                DateTime.Parse("2002-08-02")
            );

            // Then
            model.Rows.ToList()[1].Period.Should().Be("Quarter 2, 2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_year_interval_string_correctly()
        {
            // Given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-02"), ReportInterval.Years),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2003-02-02"), ReportInterval.Years),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2004-02-02"), ReportInterval.Years),
                    null
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                dailyData,
                DateTime.Parse("2002-02-02"),
                DateTime.Parse("2004-02-02")
            );

            // Then
            model.Rows.ToList()[1].Period.Should().Be("2003");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_boundary_period_strings_correctly()
        {
            // Given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-01-01"), ReportInterval.Years),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2003-01-01"), ReportInterval.Years),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2004-01-01"), ReportInterval.Years),
                    null
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                dailyData,
                DateTime.Parse("2002-02-02"),
                DateTime.Parse("2004-02-02")
            );

            // Then
            model.Rows.First().Period.Should().Be("01/01/2004 to 02/02/2004");
            model.Rows.Last().Period.Should().Be("02/02/2002 to 31/12/2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_single_period_string_correctly()
        {
            // Given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-01-01"), ReportInterval.Years),
                    null
                ),
            };

            // When
            var model = new UsageStatsTableViewModel(
                dailyData,
                DateTime.Parse("2002-02-02"),
                DateTime.Parse("2002-02-03")
            );

            // Then
            model.Rows.Count().Should().Be(1);
            model.Rows.First().Period.Should().Be("02/02/2002 to 03/02/2002");
        }
    }
}
