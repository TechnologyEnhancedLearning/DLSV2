namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CentreRegistrationPromptsDataServiceTests
    {
        private ICentreRegistrationPromptsDataService centreRegistrationPromptsDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            centreRegistrationPromptsDataService = new CentreRegistrationPromptsDataService(connection);
        }

        [Test]
        public void GetCentreRegistrationPromptsByCentreId_Returns_populated_CentreRegistrationPromptsResult()
        {
            // Given
            var expectedCentreRegistrationPromptsResult = PromptsTestHelper.GetDefaultCentreRegistrationPromptsResult();

            // When
            var returnedCentreRegistrationPromptsResult = centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(29);

            // Then
            returnedCentreRegistrationPromptsResult.Should().BeEquivalentTo(expectedCentreRegistrationPromptsResult);
        }

        [Test]
        public void UpdateCentreRegistrationPrompt_correctly_updates_registration_prompt()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? options = "options";

                // When
                centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(2, 1, false, options);
                var centreRegistrationPrompts = centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(2);

                // Then
                using (new AssertionScope())
                {
                    centreRegistrationPrompts.CustomField1Mandatory.Should().BeFalse();
                    centreRegistrationPrompts.CustomField1Options.Should().BeEquivalentTo(options);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetCustomPromptsAlphabetical_should_contain_a_custom_prompt()
        {
            // When
            var result = centreRegistrationPromptsDataService.GetCustomPromptsAlphabetical().ToList();

            // Then
            result.Contains((1, "Department / team")).Should().BeTrue();
        }

        [Test]
        public void UpdateCentreRegistrationPrompt_correctly_adds_registration_prompt()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? options = "options";

                // When
                centreRegistrationPromptsDataService.UpdateCentreRegistrationPrompt(2, 1, 1, false, options);
                var centreRegistrationPrompts = centreRegistrationPromptsDataService.GetCentreRegistrationPromptsByCentreId(2);
                var customPrompt = centreRegistrationPromptsDataService.GetCustomPromptsAlphabetical()
                    .Single(c => c.Item1 == 1)
                    .Item2;

                // Then
                using (new AssertionScope())
                {
                    centreRegistrationPrompts.CustomField1Prompt.Should().BeEquivalentTo(customPrompt);
                    centreRegistrationPrompts.CustomField1Mandatory.Should().BeFalse();
                    centreRegistrationPrompts.CustomField1Options.Should().BeEquivalentTo(options);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetCentreRegistrationPromptNameAndPromptNumber_returns_expected_prompt_name()
        {
            // When
            var result = centreRegistrationPromptsDataService.GetCentreRegistrationPromptNameAndPromptNumber(101, 1);

            // Then
            result.Should().BeEquivalentTo("Role type");
        }
    }
}
