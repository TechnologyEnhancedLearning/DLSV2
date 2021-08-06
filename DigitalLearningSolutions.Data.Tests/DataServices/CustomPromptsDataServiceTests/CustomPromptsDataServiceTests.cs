namespace DigitalLearningSolutions.Data.Tests.DataServices.CustomPromptsDataServiceTests
{
    using DigitalLearningSolutions.Data.DataServices.CustomPromptsDataService;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using NUnit.Framework;

    public partial class CustomPromptsDataServiceTests
    {
        private ICustomPromptsDataService customPromptsDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            customPromptsDataService = new CustomPromptsDataService(connection);
        }
    }
}
