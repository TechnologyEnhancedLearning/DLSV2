namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CustomPromptsDataServiceTests
    {
        private ICustomPromptsDataService customPromptsDataService = null!;

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

        [Test]
        public void UpdateCustomPrompt_correctly_updates_custom_prompt()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? options = "options";

                // When
                customPromptsDataService.UpdateCustomPromptForCentre(2, 1, false, options);
                var centreCustomPrompts = customPromptsDataService.GetCentreCustomPromptsByCentreId(2);

                // Then
                using (new AssertionScope())
                {
                    centreCustomPrompts.CustomField1Mandatory.Should().BeFalse();
                    centreCustomPrompts.CustomField1Options.Should().BeEquivalentTo(options);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void Get_custom_prompts_should_contain_a_custom_prompt()
        {
            // When
            var result = customPromptsDataService.GetCustomPromptsAlphabetical().ToList();

            // Then
            result.Contains((1, "Department / team")).Should().BeTrue();
        }
    }
}
