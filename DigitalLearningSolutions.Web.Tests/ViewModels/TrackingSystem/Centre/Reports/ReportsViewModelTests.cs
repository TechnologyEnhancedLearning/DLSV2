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
            // given
            var monthlyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2001-01-01"), ReportInterval.Months),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-02"), ReportInterval.Months),
                    null
                )
            };

            // when
            var model = new UsageStatsTableViewModel(monthlyData);

            // then
            model.Rows.First().Period.Should().Be("February, 2002");
            model.Rows.Last().Period.Should().Be("January, 2001");
        }

        [Test]
        public void ReportsFilterModel_correctly_formats_date_range()
        {
            // given
            var filterData = new ActivityFilterData(
                DateTime.Parse("2001-01-01"),
                DateTime.Parse("2002-02-02"),
                null,
                null,
                null,
                CourseFilterType.None,
                ReportInterval.Years
            );

            // when
            var model = new ReportsFilterModel(filterData, "", "", "", false);

            // then
            model.DateRange.Should().Be("01/01/2001 - 02/02/2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_day_interval_string_correctly()
        {
            // given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2001-01-01"), ReportInterval.Days),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-02"), ReportInterval.Days),
                    null
                )
            };

            // when
            var model = new UsageStatsTableViewModel(dailyData);

            // then
            model.Rows.First().Period.Should().Be("2/02/2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_week_interval_string_correctly()
        {
            // given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2001-01-01"), ReportInterval.Weeks),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-02"), ReportInterval.Weeks),
                    null
                )
            };

            // when
            var model = new UsageStatsTableViewModel(dailyData);

            // then
            model.Rows.First().Period.Should().Be("Week commencing 2/02/2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_quarter_interval_string_correctly()
        {
            // given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2001-01-01"), ReportInterval.Quarters),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-02"), ReportInterval.Quarters),
                    null
                )
            };

            // when
            var model = new UsageStatsTableViewModel(dailyData);

            // then
            model.Rows.First().Period.Should().Be("Quarter 1, 2002");
        }

        [Test]
        public void UsageStatsTableViewModel_formats_year_interval_string_correctly()
        {
            // given
            var dailyData = new[]
            {
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2001-01-01"), ReportInterval.Years),
                    null
                ),
                new PeriodOfActivity(
                    new DateInformation(DateTime.Parse("2002-02-02"), ReportInterval.Years),
                    null
                )
            };

            // when
            var model = new UsageStatsTableViewModel(dailyData);

            // then
            model.Rows.First().Period.Should().Be("2002");
        }
    }
}
