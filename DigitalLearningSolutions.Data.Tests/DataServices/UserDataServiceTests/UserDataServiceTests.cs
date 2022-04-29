namespace DigitalLearningSolutions.Data.Tests.DataServices.UserDataServiceTests
{
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
        public void GetAllExistingEmails_gets_all_existing_emails()
        {
            using var transaction = new TransactionScope();

            // Given
            connection.Execute(
                @"UPDATE AdminAccounts SET Email = 'sample@admin.email' WHERE ID = 13
                    UPDATE DelegateAccounts SET Email = 'sample@delegate.email' WHERE ID = 12"
            );

            // When
            var emails = userDataService.GetAllExistingEmails().ToList();

            // Then
            using (new AssertionScope())
            {
                emails.Count.Should().Be(281058);
                emails.Should().Contain("l@rpahd.fcubionwgsk");
                emails.Should().Contain("sample@admin.email");
                emails.Should().Contain("sample@delegate.email");
                emails.Distinct().Count().Should().Be(emails.Count);
            };
        }
    }
}
