namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.DataServices.SelfAssessmentDataService;
    using DigitalLearningSolutions.Data.Services;
    using FakeItEasy;
    using NUnit.Framework;

    public class SelfAssessmentServiceTests
    {
        private ISelfAssessmentDataService selfAssessmentDataService = null!;
        private ISelfAssessmentService selfAssessmentService = null!;

        [SetUp]
        public void Setup()
        {
            selfAssessmentDataService = A.Fake<ISelfAssessmentDataService>();
            selfAssessmentService = new SelfAssessmentService(selfAssessmentDataService);
        }
    }
}
