﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
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
            courseAdminFieldsService = new CourseAdminFieldsService(
                courseAdminFieldsDataService,
                logger
            );
        }

        [Test]
        public void GetCoursePromptsForCourse_Returns_Populated_CourseAdminFields()
        {
            // Given
            var expectedPrompt1 =
                CustomPromptsTestHelper.GetDefaultCoursePrompt(1, "System Access Granted", "Test");
            var expectedPrompt2 = CustomPromptsTestHelper.GetDefaultCoursePrompt(2, "Priority Access");
            var customPrompts = new List<CoursePrompt> { expectedPrompt1, expectedPrompt2 };
            var expectedCourseAdminFields = CustomPromptsTestHelper.GetDefaultCourseAdminFields(customPrompts);
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(100))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());

            // When
            var result = courseAdminFieldsService.GetCoursePromptsForCourse(100);

            // Then
            result.Should().BeEquivalentTo(expectedCourseAdminFields);
        }

        [Test]
        public void GetCoursePromptsWithAnswersForCourse_Returns_Populated_List_of_CustomPromptWithAnswer()
        {
            // Given
            const string answer1 = "ans1";
            const string answer2 = "ans2";
            var expected1 = CustomPromptsTestHelper.GetDefaultCoursePromptWithAnswer(
                1,
                "System Access Granted",
                "Test",
                answer: answer1
            );
            var expected2 = CustomPromptsTestHelper.GetDefaultCoursePromptWithAnswer(
                2,
                "Priority Access",
                answer: answer2
            );
            var expected = new List<CoursePromptWithAnswer> { expected1, expected2 };
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(100))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());
            var delegateCourseInfo = new DelegateCourseInfo { Answer1 = answer1, Answer2 = answer2 };

            // When
            var result = courseAdminFieldsService.GetCoursePromptsWithAnswersForCourse(delegateCourseInfo, 100);

            // Then
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateAdminFieldForCourse_calls_data_service()
        {
            // Given
            A.CallTo(() => courseAdminFieldsDataService.UpdateAdminFieldForCourse(1, 1, null)).DoesNothing();

            // When
            courseAdminFieldsService.UpdateAdminFieldForCourse(1, 1, null);

            // Then
            A.CallTo(() => courseAdminFieldsDataService.UpdateAdminFieldForCourse(1, 1, null)).MustHaveHappened();
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
        public void AddAdminFieldToCourse_adds_prompt_to_course_at_next_prompt_number()
        {
            // Given
            A.CallTo
            (
                () => courseAdminFieldsDataService.UpdateAdminFieldForCourse(100, A<int>._, A<int>._, null)
            ).DoesNothing();
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(100))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());

            // When
            var result = courseAdminFieldsService.AddAdminFieldToCourse(100, 3, null);

            // Then
            A.CallTo
            (
                () => courseAdminFieldsDataService.UpdateAdminFieldForCourse(100, 3, 3, null)
            ).MustHaveHappened();
            result.Should().BeTrue();
        }

        [Test]
        public void AddAdminFieldToCourse_does_not_add_prompt_if_course_has_all_prompts_defined()
        {
            // Given
            A.CallTo
            (
                () => courseAdminFieldsDataService.UpdateAdminFieldForCourse(100, A<int>._, A<int>._, null)
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
            var result = courseAdminFieldsService.AddAdminFieldToCourse(
                100,
                3,
                "Adding a fourth prompt"
            );

            // Then
            using (new AssertionScope())
            {
                A.CallTo(
                        () => courseAdminFieldsDataService.UpdateAdminFieldForCourse(
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
        public void RemoveAdminFieldFromCourse_calls_data_service_with_correct_values()
        {
            // Given
            A.CallTo(() => courseAdminFieldsDataService.UpdateAdminFieldForCourse(1, 1, 0, null))
                .DoesNothing();
            A.CallTo(() => courseAdminFieldsDataService.DeleteAllAnswersForCourseAdminField(1, 1)).DoesNothing();

            // When
            courseAdminFieldsService.RemoveAdminFieldFromCourse(1, 1);

            // Then
            A.CallTo(
                () => courseAdminFieldsDataService.UpdateAdminFieldForCourse(1, 1, 0, null)
            ).MustHaveHappened();
            A.CallTo(() => courseAdminFieldsDataService.DeleteAllAnswersForCourseAdminField(1, 1))
                .MustHaveHappened();
        }

        [Test]
        public void GetCoursePromptsWithAnswerCountsForCourse_returns_empty_list_with_no_admin_fields()
        {
            // Given
            const int customisationId = 1;
            const int centreId = 1;
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(customisationId))
                .Returns(new CourseAdminFieldsResult());

            // When
            var result = courseAdminFieldsService.GetCoursePromptsWithAnswerCountsForCourse(customisationId, centreId);

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEmpty();
                A.CallTo(
                    () => courseAdminFieldsDataService.GetDelegateAnswersForCourseAdminFields(customisationId, centreId)
                ).MustNotHaveHappened();
            }
        }

        [Test]
        public void GetCoursePromptsWithAnswerCountsForCourse_counts_free_text_admin_fields_correctly()
        {
            // Given
            const int customisationId = 1;
            const int centreId = 1;
            const int totalDelegatesCount = 10;
            const int numberOfDelegatesWithAnswer = 3;
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(customisationId))
                .Returns(
                    CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult(
                        "System Access Granted",
                        null,
                        null
                    )
                );

            var delegateAnswers = Builder<DelegateCourseAdminFieldAnswers>.CreateListOfSize(totalDelegatesCount)
                .TheFirst(numberOfDelegatesWithAnswer)
                .With(a => a.Answer1 = "Answer")
                .TheRest()
                .With(a => a.Answer1 = null)
                .Build();
            A.CallTo(
                    () => courseAdminFieldsDataService.GetDelegateAnswersForCourseAdminFields(customisationId, centreId)
                )
                .Returns(delegateAnswers);

            // When
            var result = courseAdminFieldsService.GetCoursePromptsWithAnswerCountsForCourse(customisationId, centreId)
                .ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().ResponseCounts.Should()
                    .BeEquivalentTo(
                        new List<ResponseCount>
                        {
                            new ResponseCount("blank", totalDelegatesCount - numberOfDelegatesWithAnswer),
                            new ResponseCount("not blank", numberOfDelegatesWithAnswer),
                        }
                    );
            }
        }

        [Test]
        public void GetCoursePromptsWithAnswerCountsForCourse_counts_configured_answers_admin_fields_correctly()
        {
            // Given
            const int customisationId = 1;
            const int centreId = 1;
            const int totalDelegatesCount = 10;
            const int numberOfDelegatesWithAnswer = 3;
            const int numberOfDelegatesWithTest = 2;
            A.CallTo(() => courseAdminFieldsDataService.GetCourseAdminFields(customisationId))
                .Returns(
                    CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult(
                        "System Access Granted",
                        "Test\r\nAnswer",
                        null
                    )
                );

            var delegateAnswers = Builder<DelegateCourseAdminFieldAnswers>.CreateListOfSize(totalDelegatesCount)
                .TheFirst(numberOfDelegatesWithAnswer)
                .With(a => a.Answer1 = "Answer")
                .TheNext(numberOfDelegatesWithTest)
                .With(a => a.Answer1 = "Test")
                .TheRest()
                .With(a => a.Answer1 = null)
                .Build();
            A.CallTo(
                    () => courseAdminFieldsDataService.GetDelegateAnswersForCourseAdminFields(customisationId, centreId)
                )
                .Returns(delegateAnswers);

            // When
            var result = courseAdminFieldsService.GetCoursePromptsWithAnswerCountsForCourse(customisationId, centreId)
                .ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().HaveCount(1);
                result.First().ResponseCounts.Should()
                    .BeEquivalentTo(
                        new List<ResponseCount>
                        {
                            new ResponseCount("Test", numberOfDelegatesWithTest),
                            new ResponseCount("Answer", numberOfDelegatesWithAnswer),
                            new ResponseCount(
                                "blank",
                                totalDelegatesCount - numberOfDelegatesWithAnswer - numberOfDelegatesWithTest
                            ),
                        }
                    );
            }
        }
    }
}
