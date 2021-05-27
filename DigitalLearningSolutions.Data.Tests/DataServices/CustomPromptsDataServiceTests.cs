namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using DigitalLearningSolutions.Data.DataServices;
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
        public void GetCentreCustomPromptsByCentreId_Returns_populated_CentreCustomPromptsResult()
        {
            // Given
            var expectedCentreCustomPromptsResult = CustomPromptsTestHelper.GetDefaultCentreCustomPromptsResult();

            // When
            var returnedCentreCustomPromptsResult = customPromptsDataService.GetCentreCustomPromptsByCentreId(29);

            // Then
            returnedCentreCustomPromptsResult.Should().BeEquivalentTo(expectedCentreCustomPromptsResult);
        }
    }
}
