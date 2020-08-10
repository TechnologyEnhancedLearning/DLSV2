namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using NUnit.Framework;
    using FluentAssertions;

    [Parallelizable(ParallelScope.Fixtures)]
    public class SelfAssessmentServiceTests
    {
        private SelfAssessmentService selfAssessmentService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            selfAssessmentService = new SelfAssessmentService(connection);
        }

        [Test]
        public void Example_test()
        {
            var result = selfAssessmentService.Example();
            result.Should().Be("");
        }
    }
}
