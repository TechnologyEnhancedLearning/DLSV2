namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System;
    using System.Collections.Generic;
    using System.Transactions;
    using Dapper;
    using FluentAssertions;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        [TestCase(new string[] { }, false)]
        [TestCase(new[] { "fake", null }, false)]
        [TestCase(new[] { "test@gmail.com" }, true)]
        [TestCase(new[] { "sample@admin.email" }, true)]
        [TestCase(new[] { "sample@delegate.email" }, true)]
        [TestCase(new[] { "test@gmail.com", "sample@admin.email", "sample@delegate.email", "fake" }, true)]
        [TestCase(new[] { "sample@admin.email", null }, true)]
        public void AnyEmailsInSetAreAlreadyInUse_returns_true_if_and_only_if_emails_are_in_use(
            IEnumerable<string?> emails,
            bool expectedResult
        )
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                    VALUES (8, 374, 'sample@admin.email'), (225773, 101, 'sample@delegate.email')"
                );

            // When
            var result = userDataService.AnyEmailsInSetAreAlreadyInUse(emails);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(null, false)]
        [TestCase("vrmei.@akwnthsbcu", false)]
        [TestCase("test@gmail.com", true)]
        [TestCase("sample@admin.email", false)]
        [TestCase("sample@delegate.email", true)]
        public void EmailIsInUseByOtherUser_throws_if_email_is_in_use_by_other_user(string email, bool expectedResult)
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                    VALUES (8, 374, 'sample@admin.email'), (225773, 101, 'sample@delegate.email')"
            );

            // When
            var result = userDataService.EmailIsInUseByOtherUser(8, email);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
