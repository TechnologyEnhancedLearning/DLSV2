namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Dapper;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Mappers;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public partial class UserDataServiceTests
    {
        private IUserDataService userDataService = null!;
        private SqlConnection connection = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            MapperHelper.SetUpFluentMapper();
        }

        [SetUp]
        public void Setup()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
        }

        [Test]
        [TestCase(new string[] { }, false)]
        [TestCase(new[] { "fake" }, false)]
        [TestCase(new[] { "test@gmail.com" }, true)]
        [TestCase(new[] { "sample@admin.email" }, true)]
        [TestCase(new[] { "sample@delegate.email" }, true)]
        [TestCase(new[] { "", null }, true)]
        [TestCase(new[] { "test@gmail.com", "sample@admin.email", "sample@delegate.email" }, true)]
        [TestCase(new[] { "sample@admin.email", "", null }, true)]
        public void AnyEmailsInSetAreAlreadyInUse_returns_true_if_and_only_if_emails_are_in_use(
            IEnumerable<string?> emails,
            bool expectedResult
        )
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"UPDATE AdminAccounts SET Email = 'sample@admin.email' WHERE ID = 13
                    UPDATE DelegateAccounts SET Email = 'sample@delegate.email' WHERE ID = 12"
            );

            // When
            var result = userDataService.AnyEmailsInSetAreAlreadyInUse(emails);

            // Then
            result.Should().Be(expectedResult);
        }
    }
}
