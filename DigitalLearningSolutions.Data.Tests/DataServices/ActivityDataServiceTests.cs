﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ActivityDataServiceTests
    {
        private IActivityDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new ActivityDataService(connection);
        }

        [Test]
        public void GetActivityForMonthsInYear_gets_activity_by_month_for_date_range()
        {
            // when
            var filterData = new ActivityFilterData
            {
                StartDate = DateTime.Parse("2014-01-01 00:00:00.000"),
                EndDate = DateTime.Parse("2014-01-31 23:59:59.999"),
                ReportInterval = ReportInterval.Months
            };
            var result = service.GetRawActivity(101, filterData).ToList();

            // then
            using (new AssertionScope())
            {
                result.Count.Should().Be(13);

                var first = result.First();
                first.LogDate.Should().Be(DateTime.Parse("2014-01-08 11:04:35.753"));
                first.LogYear.Should().Be(2014);
                first.LogQuarter.Should().Be(1);
                first.LogMonth.Should().Be(1);
                first.Completed.Should().Be(false);
                first.Evaluated.Should().Be(false);
                first.Registered.Should().Be(true);

                var last = result.Last();
                last.LogDate.Should().Be(DateTime.Parse("2014-01-31 09:43:28.840"));
                last.LogYear.Should().Be(2014);
                last.LogQuarter.Should().Be(1);
                last.LogMonth.Should().Be(1);
                last.Completed.Should().Be(false);
                last.Evaluated.Should().Be(false);
                last.Registered.Should().Be(true);
            }
        }
    }
}
