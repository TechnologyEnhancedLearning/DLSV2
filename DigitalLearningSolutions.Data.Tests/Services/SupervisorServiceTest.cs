using AngleSharp.Css;
using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.RoleProfiles;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Data.Models.Supervisor;
using DigitalLearningSolutions.Data.Services;
using DigitalLearningSolutions.Data.Tests.TestHelpers;
using DocumentFormat.OpenXml.Office2010.Excel;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
        public void GetSupervisorDelegateAssessmentWithoutAssignedSupervisor_returns_matching_record()
        {
            // When
            var result = supervisorService.GetSelfAssessmentsForSupervisorDelegateId(8, 1);

            // Then
            result.Should().Contain(x => x.IsAssignedToSupervisor == false);
        }
    }
}
