namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class DateHelperTests
    {
        [Test]
        public void GetMonthsAndYearsBetweenDates_returns_single_entry_for_dates_in_same_month()
        {
            // when
            var startDate = DateTime.Parse("2014-01-01 00:00:00.000");
            var endDate = DateTime.Parse("2014-01-31 23:59:59.999");
            var result = DateHelper.GetMonthsAndYearsBetweenDates(startDate, endDate).ToList();

            // then
            result.Count.Should().Be(1);
            result[0].Year.Should().Be(2014);
            result[0].Month.Should().Be(1);
        }

        [Test]
        public void GetMonthsAndYearsBetweenDates_returns_loops_over_months_and_increments_years()
        {
            // when
            var startDate = DateTime.Parse("2013-12-31 23:59:59.999");
            var endDate = DateTime.Parse("2014-01-01 00:00:00.000");
            var result = DateHelper.GetMonthsAndYearsBetweenDates(startDate, endDate).ToList();

            // then
            result.Count.Should().Be(2);
            result[0].Year.Should().Be(2013);
            result[0].Month.Should().Be(12);
            result[1].Year.Should().Be(2014);
            result[1].Month.Should().Be(1);
        }

        [Test]
        public void GetMonthsAndYearsBetweenDates_increments_months_within_year()
        {
            // when
            var startDate = DateTime.Parse("2014-01-31 23:59:59.999");
            var endDate = DateTime.Parse("2014-02-01 00:00:00.000");
            var result = DateHelper.GetMonthsAndYearsBetweenDates(startDate, endDate).ToList();

            // then
            result.Count.Should().Be(2);
            result[0].Year.Should().Be(2014);
            result[0].Month.Should().Be(1);
            result[1].Year.Should().Be(2014);
            result[1].Month.Should().Be(2);
        }

        [Test]
        public void GetMonthsAndYearsBetweenDates_returns_13_months_across_two_years_for_one_year_difference()
        {
            // when
            var startDate = DateTime.Parse("2014-01-01 00:00:00.000");
            var endDate = DateTime.Parse("2015-01-01 00:00:00.000");
            var result = DateHelper.GetMonthsAndYearsBetweenDates(startDate, endDate).ToList();

            // then
            result.Count.Should().Be(13);
            result.Select(m => m.Month).Should().BeEquivalentTo(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 1 });
            result.Take(12).Select(m => m.Year).Should().AllBeEquivalentTo(2014);
            result[12].Year.Should().Be(2015);
        }
    }
}
