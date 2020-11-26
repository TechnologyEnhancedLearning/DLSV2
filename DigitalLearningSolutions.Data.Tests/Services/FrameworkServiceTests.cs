namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FakeItEasy;
    using NUnit.Framework;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    public class FrameworkServiceTests
    {
        private FrameworkService frameworkService;
        private const int AdminId = 1;
        private SqlConnection connection;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<FrameworkService>>();
            frameworkService = new FrameworkService(connection, logger);
        }
        [Test]
        public void GetFrameworksForAdminId_should_return_one_framework()
        {
            // When
            var result = frameworkService.GetFrameworksForAdminId(AdminId);

            // Then
            result.Should().HaveCount(1);
        }
        [Test]
        public void GetFrameworksForAdminId_should_return_no_frameworks_when_there_are_no_frameworks_for_AdminId()
        {
            // When
            var result = frameworkService.GetFrameworksForAdminId(10);

            // Then
            result.Should().HaveCount(0);
        }
    }

}
