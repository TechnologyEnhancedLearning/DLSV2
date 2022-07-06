namespace DigitalLearningSolutions.Data.Tests.DataServices
{
    using System;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Auth;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;

    public class RegistrationConfirmationDataServiceTests
    {
        private SqlConnection connection = null!;
        private RegistrationConfirmationDataService service = null!;
        private UserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            connection = ServiceTestHelper.GetDatabaseConnection();
            userDataService = new UserDataService(connection);
            service = new RegistrationConfirmationDataService(connection);
        }

        [Test]
        public void
            SetRegistrationConfirmation_sets_DelegateAccount_RegistrationConfirmationHash_and_RegistrationConfirmationHashCreationDateTime()
        {
            using var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Given
            var createTime = new DateTime(2021, 1, 1);
            const int delegateAccountId = 2;
            var delegateAccount = userDataService.GetDelegateAccountById(delegateAccountId);
            var registrationConfirmationModel = new RegistrationConfirmationModel(
                createTime,
                "RegistrationConfirmationHash",
                delegateAccountId
            );

            // When
            service.SetRegistrationConfirmation(registrationConfirmationModel);

            var updated = userDataService.GetDelegateAccountById(delegateAccountId);

            // Then
            using (new AssertionScope())
            {
                delegateAccount!.RegistrationConfirmationHash.Should().BeNull();
                delegateAccount.RegistrationConfirmationHashCreationDateTime.Should().BeNull();

                updated!.RegistrationConfirmationHash.Should().Be(registrationConfirmationModel.Hash);
                updated.RegistrationConfirmationHashCreationDateTime.Should()
                    .Be(registrationConfirmationModel.CreateTime);
            }
        }
    }
}
