﻿namespace DigitalLearningSolutions.Data.Tests.DataServices.SelfAssessmentDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class SelfAssessmentDataServiceTests
    {
        private const int SelfAssessmentId = 1;
        private const int DelegateId = 11;
        private const int delegateUserId = 11486;
        private ISelfAssessmentDataService selfAssessmentDataService = null!;
        private SqlConnection connection = null!;
        private CompetencyTestHelper competencyTestHelper = null!;

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            var logger = A.Fake<ILogger<SelfAssessmentDataService>>();
            selfAssessmentDataService = new SelfAssessmentDataService(connection, logger);

            competencyTestHelper = new CompetencyTestHelper(connection);
        }
    }
}
