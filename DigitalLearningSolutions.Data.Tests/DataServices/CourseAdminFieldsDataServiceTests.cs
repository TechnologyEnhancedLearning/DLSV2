namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class CourseAdminFieldsDataServiceTests
    {
        private ICourseAdminFieldsDataService courseAdminFieldsDataService = null!;

        [SetUp]
        public void Setup()
        {
            var connection = ServiceTestHelper.GetDatabaseConnection();
            courseAdminFieldsDataService = new CourseAdminFieldsDataService(connection);
        }

        [Test]
        public void GetCourseAdminFields_returns_populated_CourseAdminFieldsResult()
        {
            // Given
            var expectedCourseAdminFieldsResult =
                CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult(
                    null,
                    "Yes\nNo\nNot sure",
                    true,
                    null,
                    "Yes\nNo\nNot sure",
                    courseCategoryId: 2
                );

            // When
            var returnedCourseAdminFieldsResult = courseAdminFieldsDataService.GetCourseAdminFields(1379, 101, 0);

            // Then
            returnedCourseAdminFieldsResult.Should().BeEquivalentTo(expectedCourseAdminFieldsResult);
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
                courseAdminFieldsDataService.UpdateCustomPromptForCourse(1379, 1, 1, false, options);
                var courseAdminFields = courseAdminFieldsDataService.GetCourseAdminFields(1379, 101, 0);

                // Then
                using (new AssertionScope())
                {
                    courseAdminFields.CustomField1Mandatory.Should().BeFalse();
                    courseAdminFields.CustomField1Options.Should().BeEquivalentTo(options);
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
            var result = courseAdminFieldsDataService.GetPromptName(100, 1);

            // Then
            result.Should().BeEquivalentTo("System Access Granted");
        }

        [Test]
        public void GetAnswerCountForCourseAdminField_returns_expected_count()
        {
            // When
            var count = courseAdminFieldsDataService.GetAnswerCountForCourseAdminField(100, 1);

            // Then
            count.Should().Be(1);
        }

        [Test]
        public void DeleteAllAnswersForCourseAdminField_deletes_all_answers()
        {
            using var transaction = new TransactionScope();
            try
            {
                // When
                courseAdminFieldsDataService.DeleteAllAnswersForCourseAdminField(100, 1);
                var updatedCount = courseAdminFieldsDataService.GetAnswerCountForCourseAdminField(100, 1);

                // Then
                updatedCount.Should().Be(0);
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
