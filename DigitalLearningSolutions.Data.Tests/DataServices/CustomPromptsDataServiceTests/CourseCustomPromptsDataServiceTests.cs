namespace DigitalLearningSolutions.Data.Tests.DataServices.CustomPromptsDataServiceTests
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class CustomPromptsDataServiceTests
    {
        [Test]
        public void GetCourseCustomPrompts_returns_populated_CourseCustomPromptsResult()
        {
            // Given
            var expectedCourseCustomPromptsResult =
                CustomPromptsTestHelper.GetDefaultCourseCustomPromptsResult(
                    null,
                    "Yes\nNo\nNot sure",
                    true,
                    null,
                    "Yes\nNo\nNot sure",
                    courseCategoryId: 2
                );

            // When
            var returnedCourseCustomPromptsResult = customPromptsDataService.GetCourseCustomPrompts(1379, 101, 0);

            // Then
            returnedCourseCustomPromptsResult.Should().BeEquivalentTo(expectedCourseCustomPromptsResult);
        }

        [Test]
        public void UpdateCustomPromptForCourse_correctly_updates_custom_prompt()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? options = "options";

                // When
                customPromptsDataService.UpdateCustomPromptForCourse(1379, 1, 1, false, options);
                var courseCustomPrompts = customPromptsDataService.GetCourseCustomPrompts(1379, 101, 0);

                // Then
                using (new AssertionScope())
                {
                    courseCustomPrompts.CustomField1Mandatory.Should().BeFalse();
                    courseCustomPrompts.CustomField1Options.Should().BeEquivalentTo(options);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetPromptNameForCourseAndPromptNumber_returns_expected_prompt_name()
        {
            // When
            var result = customPromptsDataService.GetPromptNameForCourseAndPromptNumber(100, 1);

            // Then
            result.Should().BeEquivalentTo("System Access Granted");
        }
    }
}
