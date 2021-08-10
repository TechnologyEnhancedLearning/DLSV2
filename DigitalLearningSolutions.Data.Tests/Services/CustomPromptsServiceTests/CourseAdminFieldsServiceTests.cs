namespace DigitalLearningSolutions.Data.Tests.Services.CustomPromptsServiceTests
{
    using System.Collections.Generic;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public partial class CustomPromptsServiceTests
    {
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
            A.CallTo(() => customPromptsDataService.GetCourseAdminFields(27920, 101, 0))
                .Returns(CustomPromptsTestHelper.GetDefaultCourseAdminFieldsResult());
            var delegateCourseInfo = new DelegateCourseInfo { Answer1 = answer1, Answer2 = answer2 };

            // When
            var result = customPromptsService.GetCustomPromptsWithAnswersForCourse(delegateCourseInfo, 27920, 101);

            // Then
            result.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void UpdateCustomPromptForCourse_call_data_service()
        {
            // Given
            A.CallTo(() => customPromptsDataService.UpdateCustomPromptForCourse(1, 1, true, null)).DoesNothing();

            // When
            customPromptsService.UpdateCustomPromptForCourse(1, 1, true, null);

            // Then
            A.CallTo(() => customPromptsDataService.UpdateCustomPromptForCourse(1, 1, true, null)).MustHaveHappened();
        }

        [Test]
        public void RemoveCustomPromptFromCourse_calls_data_service_with_correct_values()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                A.CallTo(() => customPromptsDataService.UpdateCustomPromptForCourse(1, 1, 0, false, null))
                    .DoesNothing();
                A.CallTo(() => userDataService.DeleteAllAnswersForAdminField(1, 1)).DoesNothing();

                // When
                customPromptsService.RemoveCustomPromptFromCourse(1, 1);

                // Then
                A.CallTo(
                    () => customPromptsDataService.UpdateCustomPromptForCourse(1, 1, 0, false, null)
                ).MustHaveHappened();
                A.CallTo(() => userDataService.DeleteAllAnswersForAdminField(1, 1)).MustHaveHappened();
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}
