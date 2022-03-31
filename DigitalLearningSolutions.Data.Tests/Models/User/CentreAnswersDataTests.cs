namespace DigitalLearningSolutions.Data.Tests.Models.User
{
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

        [Test]
        [TestCase(1, "answer 1")]
        [TestCase(2, "answer 2")]
        [TestCase(3, "answer 3")]
        [TestCase(4, "answer 4")]
        [TestCase(5, "answer 5")]
        [TestCase(6, "answer 6")]
        public void GettingAnswersByPromptNumber_returns_correct_answer(int promptNumber, string expectedAnswer)
        {
            // When
            var answer = testAnswers.GetAnswerForRegistrationPromptNumber(promptNumber);

            // Then
            answer.Should().BeEquivalentTo(expectedAnswer);
        }
    }
}
