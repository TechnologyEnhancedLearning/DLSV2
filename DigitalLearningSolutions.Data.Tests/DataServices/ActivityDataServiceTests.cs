namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    class ActivityDataServiceTests
    {
        private IActivityDataService service = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            service = new ActivityDataService(connection);
        }

        [Test]
        public void GetActivityForMonthsInYear_gets_activity_for_months_in_year()
        {
            // when
            var result = service.GetActivityForMonthsInYear(101, 2014, new[] { 1, 2, 3, 4 }).ToList();

            // then
            result.Count().Should().Be(4);

            var first = result.First();
            first.Year.Should().Be(2014);
            first.Month.Should().Be(4);
            first.Completions.Should().Be(0);
            first.Evaluations.Should().Be(1);
            first.Registrations.Should().Be(7);

            var last = result.Last();
            last.Year.Should().Be(2014);
            last.Month.Should().Be(1);
            last.Completions.Should().Be(1);
            last.Evaluations.Should().Be(0);
            last.Registrations.Should().Be(12);
        }
    }
}
