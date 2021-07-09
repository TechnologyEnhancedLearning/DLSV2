namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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

        [Test]
        public void UpdateCustomPromptForCentre_correctly_adds_custom_prompt()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? options = "options";

                // When
                customPromptsDataService.UpdateCustomPromptForCentre(2, 1, 1, false, options);
                var centreCustomPrompts = customPromptsDataService.GetCentreCustomPromptsByCentreId(2);
                var customPrompt = customPromptsDataService.GetCustomPromptsAlphabetical().Single(c => c.Item1 == 1).Item2;

                // Then
                using (new AssertionScope())
                {
                    centreCustomPrompts.CustomField1Prompt.Should().BeEquivalentTo(customPrompt);
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
        public void GetPromptNameForCentreAndPromptNumber_returns_expected_prompt_name()
        {
            // When
            var result = customPromptsDataService.GetPromptNameForCentreAndPromptNumber(101, 1);

            // Then
            result.Should().BeEquivalentTo("Role type");
        }

        [Test]
        public void GetCourseCustomPrompts_returns_populated_CourseCustomPromptsResult()
        {
            // Given
            var expectedCourseCustomPromptsResult =
                CustomPromptsTestHelper.GetDefaultCourseCustomPromptsResult(
                    1379,
                    customField1Prompt: null,
                    customField1Options: "Yes\nNo\nNot sure",
                    customField1Mandatory: true,
                    customField2Prompt: null,
                    customField2Options:"Yes\nNo\nNot sure"
                    );

            // When
            var returnedCourseCustomPromptsResult = customPromptsDataService.GetCourseCustomPrompts(1379, 101, 0);

            // Then
            returnedCourseCustomPromptsResult.Should().BeEquivalentTo(expectedCourseCustomPromptsResult);
        }
    }
}
