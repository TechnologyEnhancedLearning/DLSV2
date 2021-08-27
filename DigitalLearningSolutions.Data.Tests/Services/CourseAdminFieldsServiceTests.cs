namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class CourseAdminFieldsServiceTests
    {
        private ICourseAdminFieldsDataService courseAdminFieldsDataService = null!;
        private ICourseAdminFieldsService courseAdminFieldsService = null!;
        private ILogger<CourseAdminFieldsService> logger = null!;

        [SetUp]
        public void Setup()
        {
            courseAdminFieldsDataService = A.Fake<ICourseAdminFieldsDataService>();
            logger = A.Fake<ILogger<CourseAdminFieldsService>>();
            courseAdminFieldsService = new CourseAdminFieldsService(courseAdminFieldsDataService, logger);
        }

        [Test]
        public void GetCustomPromptsForCourse_Returns_Populated_CourseAdminFields()
        {
            // Given
            var expectedPrompt1 =
                CustomPromptsTestHelper.GetDefaultCustomPrompt(1, "System Access Granted", "Yes\r\nNo");
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, "Access Permissions");
            var customPrompts = new List<CustomPrompt> { expectedPrompt1, expectedPrompt2 };
            var expectedCourseAdminFields = CustomPromptsTestHelper.GetDefaultCourseAdminFields(customPrompts);
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(27920, 101, 0))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());

            // When
            var result = courseAdminFieldsService.GetCustomPromptsForCourse(27920, 101, 0);

            // Then
            result.Should().BeEquivalentTo(expectedCourseAdminFields);
        }

        [Test]
        public void GetCustomPromptsWithAnswersForCourse_Returns_Populated_List_of_CustomPromptWithAnswer()
        {
            // Given
            const string answer1 = "ans1";
            const string answer2 = "ans2";
            var expected1 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(
                1,
                "System Access Granted",
                "Yes\r\nNo",
                answer: answer1
            );
            var expected2 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(
                2,
                "Access Permissions",
                answer: answer2
            );
            var expected = new List<CustomPromptWithAnswer> { expected1, expected2 };
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(27920, 101, 0))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());
            var delegateCourseInfo = new DelegateCourseInfo { Answer1 = answer1, Answer2 = answer2 };

            // When
            var result = courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(delegateCourseInfo, 27920, 101);

            // Then
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateCustomPromptForCourse_calls_data_service()
        {
            // Given
            A.CallTo(() => courseAdminFieldsDataService.UpdateCustomPromptForCourse(1, 1, null)).DoesNothing();

            // When
            courseAdminFieldsService.UpdateCustomPromptForCourse(1, 1, null);

            // Then
            A.CallTo(() => courseAdminFieldsDataService.UpdateCustomPromptForCourse(1, 1, null)).MustHaveHappened();
        }

        [Test]
        public void RemoveCustomPromptFromCourse_calls_data_service_with_correct_values()
        {
            // Given
            A.CallTo(() => courseAdminFieldsDataService.UpdateCustomPromptForCourse(1, 1, 0, null))
                .DoesNothing();
            A.CallTo(() => courseAdminFieldsDataService.DeleteAllAnswersForCourseAdminField(1, 1)).DoesNothing();

            // When
            courseAdminFieldsService.RemoveCustomPromptFromCourse(1, 1);

            // Then
            A.CallTo(
                () => courseAdminFieldsDataService.UpdateCustomPromptForCourse(1, 1, 0, null)
            ).MustHaveHappened();
            A.CallTo(() => courseAdminFieldsDataService.DeleteAllAnswersForCourseAdminField(1, 1))
                .MustHaveHappened();
        }
    }
}
