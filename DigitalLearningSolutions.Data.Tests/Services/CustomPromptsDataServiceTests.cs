namespace DigitalLearningSolutions.Data.Tests.Services
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CustomPromptsDataServiceTests
    {
        private ICustomPromptsDataService customPromptsDataService;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            customPromptsDataService = new CustomPromptsDataService(connection);
        }

        [Test]
        public void GetCentreCustomPromptsByCentreId()
        {
            // Given
            var expectedCentreCustomPromptsResult = CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult();

            //When
            var returnedCentreCustomPromptsResult = customPromptsDataService.GetCentreCustomPromptsByCentreId(2);

            // Then
            returnedCentreCustomPromptsResult.Should().BeEquivalentTo(expectedCentreCustomPromptsResult);
        }
    }
}
