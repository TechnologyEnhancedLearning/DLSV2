﻿namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System.Threading.Tasks;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class RegistrationDataServiceTests
    {
        private SqlConnection connection = null!;
        private RegistrationDataService service = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            service = new RegistrationDataService(connection);
        }

        [Ignore("Clashes with existing test data")]
        [Test]
        public async Task Sets_all_fields_correctly_on_registration()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var delegateRegistrationModel = UserTestHelper.GetDefaultDelegateRegistrationModel();

            // When
            var candidateNumber = service.RegisterDelegate(delegateRegistrationModel);
            var user = await connection.GetDelegateUserByCandidateNumberAsync(candidateNumber);

            // Then
            user.FirstName.Should().Be(delegateRegistrationModel.FirstName);
            user.LastName.Should().Be(delegateRegistrationModel.LastName);
            user.EmailAddress.Should().Be(delegateRegistrationModel.Email);
            user.CentreId.Should().Be(delegateRegistrationModel.Centre);
            user.Answer1.Should().Be(delegateRegistrationModel.Answer1);
            user.Answer2.Should().Be(delegateRegistrationModel.Answer2);
            user.Answer3.Should().Be(delegateRegistrationModel.Answer3);
            user.Answer4.Should().Be(delegateRegistrationModel.Answer4);
            user.Answer5.Should().Be(delegateRegistrationModel.Answer5);
            user.Answer6.Should().Be(delegateRegistrationModel.Answer6);
        }
    }
}
