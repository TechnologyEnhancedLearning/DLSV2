namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        private IUserDataService userDataService = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
        }
    }
}
