﻿namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class LinkedFieldHelperTests
    {
        private const string OldAnswer = "old answer";
        private const string NewAnswer = "new answer";
        private const string OldJobGroupName = "old job group";
        private const string NewJobGroupName = "new job group";
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private IJobGroupsDataService jobGroupsDataService = null!;

        [SetUp]
        public void SetUp()
        {
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();

            A.CallTo(() => jobGroupsDataService.GetJobGroupName(1)).Returns(OldJobGroupName);
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(2)).Returns(NewJobGroupName);
        }

        [Test]
        [TestCaseSource(nameof(GetTestItems))]
        public void Changed_answer_maps_to_linked_field_correctly(
            RegistrationFieldAnswers oldAnswers,
            RegistrationFieldAnswers newAnswers,
            string expectedLinkedFieldName,
            int expectedLinkedFieldNumber,
            string expectedOldValue,
            string expectedNewValue
        )
        {
            // Given
            A.CallTo(
                    () => centreRegistrationPromptsService.GetCentreRegistrationPromptNameAndNumber(A<int>._, A<int>._)
                )
                .Returns(expectedLinkedFieldName);

            var expectedResult = new List<LinkedFieldChange>
            {
                new LinkedFieldChange(
                    expectedLinkedFieldNumber,
                    expectedLinkedFieldName,
                    expectedOldValue,
                    expectedNewValue
                ),
            };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldChanges(
                oldAnswers,
                newAnswers,
                jobGroupsDataService,
                centreRegistrationPromptsService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        private static IEnumerable<object[]> GetTestItems()
        {
            yield return new object[]
            {
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: OldAnswer),
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer1: NewAnswer),
                "prompt 1", 1, OldAnswer, NewAnswer,
            };
            yield return new object[]
            {
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer2: OldAnswer),
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer2: NewAnswer),
                "prompt 2", 2, OldAnswer, NewAnswer,
            };
            yield return new object[]
            {
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer3: OldAnswer),
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer3: NewAnswer),
                "prompt 3", 3, OldAnswer, NewAnswer,
            };
            yield return new object[]
            {
                UserTestHelper.GetDefaultRegistrationFieldAnswers(jobGroupId: 1),
                UserTestHelper.GetDefaultRegistrationFieldAnswers(jobGroupId: 2),
                "Job group", 4, OldJobGroupName, NewJobGroupName,
            };
            yield return new object[]
            {
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer4: OldAnswer),
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer4: NewAnswer),
                "prompt 4", 5, OldAnswer, NewAnswer,
            };
            yield return new object[]
            {
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer5: OldAnswer),
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer5: NewAnswer),
                "prompt 5", 6, OldAnswer, NewAnswer,
            };
            yield return new object[]
            {
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer6: OldAnswer),
                UserTestHelper.GetDefaultRegistrationFieldAnswers(answer6: NewAnswer),
                "prompt 6", 7, OldAnswer, NewAnswer,
            };
        }
    }
}
