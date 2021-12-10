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
    using FluentAssertions.Execution;
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
                CustomPromptsTestHelper.GetDefaultCustomPrompt(1, "System Access Granted", "Test");
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCustomPrompt(2, "Priority Access");
            var customPrompts = new List<CustomPrompt> { expectedPrompt1, expectedPrompt2 };
            var expectedCourseAdminFields = CustomPromptsTestHelper.GetDefaultCourseAdminFields(customPrompts);
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(100))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());

            // When
            var result = courseAdminFieldsService.GetCustomPromptsForCourse(100);

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
                "Test",
                answer: answer1
            );
            var expected2 = CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(
                2,
                "Priority Access",
                answer: answer2
            );
            var expected = new List<CustomPromptWithAnswer> { expected1, expected2 };
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(100))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());
            var delegateCourseInfo = new DelegateCourseInfo { Answer1 = answer1, Answer2 = answer2 };

            // When
            var result = courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(delegateCourseInfo, 100);

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
        public void GetCoursePromptsAlphabeticalList_calls_data_service()
        {
            // Given
            const string promptName = "Access Permissions";
            var coursePrompts = new List<(int, string)> { (1, promptName) };
            A.CallTo(() => courseAdminFieldsDataService.GetCoursePromptsAlphabetical()).Returns
                (coursePrompts);

            // When
            var result = courseAdminFieldsService.GetCoursePromptsAlphabeticalList();

            // Then
            A.CallTo(() => courseAdminFieldsDataService.GetCoursePromptsAlphabetical()).MustHaveHappened();
            result.Should().BeEquivalentTo(coursePrompts);
        }

        [Test]
        public void AddCustomPromptToCourse_adds_prompt_to_course_at_next_prompt_number()
        {
            // Given
            A.CallTo
            (
                () => courseAdminFieldsDataService.UpdateCustomPromptForCourse(100, A<int>._, A<int>._, null)
            ).DoesNothing();
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(100))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());

            // When
            var result = courseAdminFieldsService.AddCustomPromptToCourse(100, 3, null);

            // Then
            A.CallTo
            (
                () => courseAdminFieldsDataService.UpdateCustomPromptForCourse(100, 3, 3, null)
            ).MustHaveHappened();
            result.Should().BeTrue();
        }

        [Test]
        public void AddCustomPromptToCourse_does_not_add_prompt_if_course_has_all_prompts_defined()
        {
            // Given
            A.CallTo
            (
                () => courseAdminFieldsDataService.UpdateCustomPromptForCourse(100, A<int>._, A<int>._, null)
            ).DoesNothing();
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(100))
                .Returns(
                    CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult(
                        "System Access Granted",
                        "Test",
                        "Priority Access",
                        "",
                        "Access Permissions"
                    )
                );

            // When
            var result = courseAdminFieldsService.AddCustomPromptToCourse(
                100,
                3,
                "Adding a fourth prompt"
            );

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => courseAdminFieldsDataService.UpdateCustomPromptForCourse(
                            100,
                            A<int>._,
                            A<int>._,
                            "Adding a fourth prompt"
                        )
                    )
                    .MustNotHaveHappened();
                result.Should().BeFalse();
            }
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
