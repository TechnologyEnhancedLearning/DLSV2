namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Centre.Reports
{
    using System.Linq;
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
                new MonthOfActivity((1, 2001), null),
                new MonthOfActivity((2, 2002), null)
            };

            // when
            var model = new UsageStatsTableViewModel(monthlyData);

            // then
            model.Rows.First().Period.Should().Be("February, 2002");
            model.Rows.Last().Period.Should().Be("January, 2001");
        }
    }
}
