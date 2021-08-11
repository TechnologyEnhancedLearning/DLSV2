﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
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
                new PeriodOfActivity(new DateInformation{Date = DateTime.Parse("2001-01-01"), Interval = ReportInterval.Months}, null),
                new PeriodOfActivity(new DateInformation{Date = DateTime.Parse("2002-02-02"), Interval = ReportInterval.Months}, null)
            };

            // when
            var model = new UsageStatsTableViewModel(monthlyData);

            // then
            model.Rows.First().Period.Should().Be("February, 2002");
            model.Rows.Last().Period.Should().Be("January, 2001");
        }
    }
}
