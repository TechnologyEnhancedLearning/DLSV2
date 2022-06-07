namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentreAnswersDataTests
    {
        private readonly CentreAnswersData testAnswers = new CentreAnswersData(
            1,
            2,
            "answer 1",
            "answer 2",
            "answer 3",
            "answer 4",
            "answer 5",
            "answer 6"
        );

        private static IEnumerable<object[]> GetTestCases()
        {
            yield return new object[] { RegistrationField.CentreRegistrationField1, "answer 1" };
            yield return new object[] { RegistrationField.CentreRegistrationField2, "answer 2" };
            yield return new object[] { RegistrationField.CentreRegistrationField3, "answer 3" };
            yield return new object[] { RegistrationField.CentreRegistrationField4, "answer 4" };
            yield return new object[] { RegistrationField.CentreRegistrationField5, "answer 5" };
            yield return new object[] { RegistrationField.CentreRegistrationField6, "answer 6" };
        }

        [Test]
        [TestCaseSource(nameof(GetTestCases))]
        public void GettingAnswersByPromptNumber_returns_correct_answer(RegistrationField field, string expectedAnswer)
        {
            // When
            var answer = testAnswers.GetAnswerForRegistrationPromptNumber(field);

            // Then
            answer.Should().BeEquivalentTo(expectedAnswer);
        }
    }
}
