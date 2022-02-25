namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
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
                PromptsTestHelper.GetDefaultCourseAdminFieldsResult(
                    "System Access Granted",
                    "Test",
                    "Priority Access",
                    null,
                    null,
                    null,
                    2
                );

            // When
            var returnedCourseAdminFieldsResult = courseAdminFieldsDataService.GetCourseAdminFields(100);

            // Then
            returnedCourseAdminFieldsResult.Should().BeEquivalentTo(expectedCourseAdminFieldsResult);
        }

        [Test]
        public void UpdateAdminFieldForCourse_correctly_updates_custom_prompt()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? options = "options";

                // When
                courseAdminFieldsDataService.UpdateAdminFieldForCourse(100, 1, 1, options);
                var courseAdminFields = courseAdminFieldsDataService.GetCourseAdminFields(100);

                // Then
                using (new AssertionScope())
                {
                    courseAdminFields!.CustomField1Options.Should().BeEquivalentTo(options);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetPromptName_returns_expected_prompt_name()
        {
            // When
            var result = courseAdminFieldsDataService.GetPromptName(100, 1);

            // Then
            result.Should().BeEquivalentTo("System Access Granted");
        }

        [Test]
        public void UpdateAdminFieldForCourse_correctly_adds_admin_field()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? options = "options";

                // When
                courseAdminFieldsDataService.UpdateAdminFieldForCourse(100, 3, 1, options);
                var courseCustomPrompts = courseAdminFieldsDataService.GetCourseAdminFields(100);
                var customPrompt = courseAdminFieldsDataService.GetCoursePromptsAlphabetical()
                    .Single(c => c.id == 1)
                    .name;

                // Then
                using (new AssertionScope())
                {
                    courseCustomPrompts!.CustomField3Prompt.Should().BeEquivalentTo(customPrompt);
                    courseCustomPrompts.CustomField3Options.Should().BeEquivalentTo(options);
                }
            }
            finally
            {
                transaction.Dispose();
            }
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

        [Test]
        public void GetDelegateAnswersForCourseAdminFields_returns_expected_results()
        {
            // Given
            const int customisationId = 319;
            const int centreId = 101;
            var expectedResult = new List<DelegateCourseAdminFieldAnswers>
            {
                new DelegateCourseAdminFieldAnswers
                {
                    Answer1 = "s",
                    Answer2 = "sss",
                    Answer3 = "",
                },
                new DelegateCourseAdminFieldAnswers
                {
                    Answer1 = "uyuy",
                    Answer2 = "hhjkh",
                    Answer3 = "",
                }
            };

            // When
            var result = courseAdminFieldsDataService.GetDelegateAnswersForCourseAdminFields(customisationId, centreId)
                .ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(2);
                result.Should().BeEquivalentTo(expectedResult);
            }
        }
    }
}
