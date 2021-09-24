namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using NUnit.Framework;

    public class ProgressDataServiceTests
    {
        private IProgressDataService progressDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            progressDataService = new ProgressDataService(connection);
        }

        [Test]
        public void InsertNewAspProgressForTutorialIfNoneExist_inserts_new_record()
        {

        }
    }
}
