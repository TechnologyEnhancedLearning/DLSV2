namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class LinkedFieldHelperTests
    {
        private IJobGroupsDataService jobGroupsDataService = null!;

        [SetUp]
        public void SetUp()
        {
            jobGroupsDataService = A.Fake<IJobGroupsDataService>();
        }

        [Test]
        public void Changed_answer_1_maps_to_linked_field_1()
        {
            // Given
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(answer1: "old answer");
            var newAnswers = UserTestHelper.GetDefaultCentreAnswersData(answer1: "new answer");
            var expectedResult = new List<LinkedFieldNumberWithValues>
                { new LinkedFieldNumberWithValues(1, "old answer", "new answer") };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newAnswers,
                jobGroupsDataService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Changed_answer_2_maps_to_linked_field_2()
        {
            // Given
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(answer2: "old answer");
            var newAnswers = UserTestHelper.GetDefaultCentreAnswersData(answer2: "new answer");
            var expectedResult = new List<LinkedFieldNumberWithValues>
                { new LinkedFieldNumberWithValues(2, "old answer", "new answer") };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newAnswers,
                jobGroupsDataService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Changed_answer_3_maps_to_linked_field_3()
        {
            // Given
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(answer3: "old answer");
            var newAnswers = UserTestHelper.GetDefaultCentreAnswersData(answer3: "new answer");
            var expectedResult = new List<LinkedFieldNumberWithValues>
                { new LinkedFieldNumberWithValues(3, "old answer", "new answer") };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newAnswers,
                jobGroupsDataService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Changed_JobGroup_maps_to_linked_field_4()
        {
            // Given
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(jobGroupId: 1);
            var newAnswers = UserTestHelper.GetDefaultCentreAnswersData(jobGroupId: 2);
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(1)).Returns("old job group");
            A.CallTo(() => jobGroupsDataService.GetJobGroupName(2)).Returns("new job group");
            var expectedResult = new List<LinkedFieldNumberWithValues>
                { new LinkedFieldNumberWithValues(4, "old job group", "new job group") };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newAnswers,
                jobGroupsDataService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Changed_answer_4_maps_to_linked_field_5()
        {
            // Given
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(answer4: "old answer");
            var newAnswers = UserTestHelper.GetDefaultCentreAnswersData(answer4: "new answer");
            var expectedResult = new List<LinkedFieldNumberWithValues>
                { new LinkedFieldNumberWithValues(5, "old answer", "new answer") };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newAnswers,
                jobGroupsDataService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Changed_answer_5_maps_to_linked_field_6()
        {
            // Given
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(answer5: "old answer");
            var newAnswers = UserTestHelper.GetDefaultCentreAnswersData(answer5: "new answer");
            var expectedResult = new List<LinkedFieldNumberWithValues>
                { new LinkedFieldNumberWithValues(6, "old answer", "new answer") };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newAnswers,
                jobGroupsDataService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Test]
        public void Changed_answer_6_maps_to_linked_field_7()
        {
            // Given
            var oldDelegateDetails = UserTestHelper.GetDefaultDelegateUser(answer6: "old answer");
            var newAnswers = UserTestHelper.GetDefaultCentreAnswersData(answer6: "new answer");
            var expectedResult = new List<LinkedFieldNumberWithValues>
                { new LinkedFieldNumberWithValues(7, "old answer", "new answer") };

            // When
            var result = LinkedFieldHelper.GetLinkedFieldNumbersWithValuesFromChangedAnswers(
                oldDelegateDetails,
                newAnswers,
                jobGroupsDataService
            );

            // Then
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
