namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
    using System.Transactions;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        public void UpdateDelegateUserCentrePrompts_updates_user()
        {
            using var transaction = new TransactionScope();
            try
            {
                // Given
                const string? answer1 = "Answer1";
                const string? answer2 = "Answer2";
                const string? answer3 = "Answer3";
                const string? answer4 = "Answer4";
                const string? answer5 = "Answer5";
                const string? answer6 = "Answer6";

                // When
                userDataService.UpdateDelegateUserCentrePrompts(
                    2,
                    answer1,
                    answer2,
                    answer3,
                    answer4,
                    answer5,
                    answer6,
                    DateTime.Now
                );
                var updatedUser = userDataService.GetDelegateUserById(2)!;

                // Then
                using (new AssertionScope())
                {
                    updatedUser.Answer1.Should().BeEquivalentTo(answer1);
                    updatedUser.Answer2.Should().BeEquivalentTo(answer2);
                    updatedUser.Answer3.Should().BeEquivalentTo(answer3);
                    updatedUser.Answer4.Should().BeEquivalentTo(answer4);
                    updatedUser.Answer5.Should().BeEquivalentTo(answer5);
                    updatedUser.Answer6.Should().BeEquivalentTo(answer6);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        [Test]
        public void GetDelegateCountWithAnswerForPrompt_returns_expected_count()
        {
            // When
            var count = userDataService.GetDelegateCountWithAnswerForPrompt(101, 1);

            // Then
            count.Should().Be(124);
        }
    }
}
