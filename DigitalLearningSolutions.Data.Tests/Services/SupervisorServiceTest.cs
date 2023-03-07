using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Tests.TestHelpers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace DigitalLearningSolutions.Data.Tests.Services
{
    public class SupervisorServiceTest
    {
        private ISupervisorService supervisorService = null!;

        [SetUp]
        public void SetUp()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SupervisorService>>();
            supervisorService = new SupervisorService(connection, logger);
        }


        [Test]
        public void GetNonNationalSupervisorDelegateAssessmentWithoutAssignedSupervisor_returns_empty_record()
        {
            // When
            var result = supervisorService.GetSelfAssessmentsForSupervisorDelegateId(8, 1);

            // Then
            result.Should().BeEmpty();
        }
    }
}
