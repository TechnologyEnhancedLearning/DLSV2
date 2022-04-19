namespace DigitalLearningSolutions.Data.Tests.DataServices.SelfAssessmentDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public partial class SelfAssessmentDataServiceTests
    {
        private const int SelfAssessmentId =4;
        private const int CandidateId = 11;
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
