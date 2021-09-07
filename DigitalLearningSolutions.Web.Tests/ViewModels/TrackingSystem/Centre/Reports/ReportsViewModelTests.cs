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
                ReportInterval.Years
            );

            // when
            var model = new ReportsFilterModel(filterData, "", "", "", false);

            // then
            model.DateRange.Should().Be("01/01/2001 - 02/02/2002");
        }
    }
}
