namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        [Test]
        [TestCase(new string[] { }, false)]
        [TestCase(new[] { "fake" }, false)]
        [TestCase(new[] { "test@gmail.com" }, true)]
        [TestCase(new[] { "sample@admin.email" }, true)]
        [TestCase(new[] { "sample@delegate.email" }, true)]
        [TestCase(new[] { "test@gmail.com", "sample@admin.email", "sample@delegate.email", "fake" }, true)]
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
        [TestCase("vrmei.@akwnthsbcu", false)]
        [TestCase("test@gmail.com", true)]
        [TestCase("sample@admin.email", false)]
        [TestCase("sample@delegate.email", true)]
        public void EmailIsInUseByOtherUser_returns_true_if_email_is_in_use_by_other_user(
            string email,
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
            var result = userDataService.EmailIsInUseByOtherUser(8, email);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase(true, null, 1)]
        [TestCase(true, "new@admin.email", 1)]
        [TestCase(false, null, 0)]
        [TestCase(false, "new@admin.email", 1)]
        public void SetCentreEmail_sets_email_if_not_empty(bool detailsExist, string? email, int entriesCount)
        {
            using var transaction = new TransactionScope();

            // Given
            if (detailsExist)
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                        VALUES (8, 374, 'sample@admin.email')"
                );
            }

            // When
            userDataService.SetCentreEmail(8, 374, email);
            var result = connection.Query<string?>(@"SELECT Email FROM UserCentreDetails WHERE UserID = 8")
                .SingleOrDefault();
            var count = connection.Query<int>(@"SELECT COUNT(*) FROM UserCentreDetails WHERE UserID = 8");

            // Then
            result.Should().BeEquivalentTo(email);
            count.Should().Equal(entriesCount);
        }

        [Test]
        public void GetUnverifiedCentreEmailsForUser_returns_unverified_emails_for_active_centres()
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email, EmailVerified)
                        VALUES (8, 374, 'verified@centre.email', '2022-06-17 17:06:22')"
            );
            connection.Execute(
                @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                        VALUES (8, 375, 'unverified@centre.email')"
            );
            connection.Execute(
                @"INSERT INTO UserCentreDetails(UserID, CentreID, Email)
                        VALUES (8, 378, 'unverified@inactive_centre.email')"
            );

            // When
            var result = userDataService.GetUnverifiedCentreEmailsForUser(8).ToList();

            // Then
            result.Should().HaveCount(1);
            result[0].centreEmail.Should().BeEquivalentTo("unverified@centre.email");
        }

        [Test]
        [TestCase("test@gmail.com", true)]
        [TestCase("not_an_email_in_the_database", false)]
        public void PrimaryEmailIsInUse_returns_expected_value(string email, bool expectedResult)
        {
            // When
            var result = userDataService.PrimaryEmailIsInUse(email);

            // Then
            result.Should().Be(expectedResult);
        }

        [Test]
        [TestCase("centre@email.com", true)]
        [TestCase("not_an_email_in_the_database", false)]
        public void CentreSpecificEmailIsInUseAtCentre_returns_expected_value(string email, bool expectedResult)
        {
            using var transaction = new TransactionScope();

            // Given
            const int centreId = 2;

            if (expectedResult)
            {
                connection.Execute(
                    @"INSERT INTO UserCentreDetails (UserID, CentreID, Email)
                    VALUES (1, @centreId, @email)",
                    new { centreId, email }
                );
            }

            // When
            var result = userDataService.CentreSpecificEmailIsInUseAtCentre(email, centreId);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
