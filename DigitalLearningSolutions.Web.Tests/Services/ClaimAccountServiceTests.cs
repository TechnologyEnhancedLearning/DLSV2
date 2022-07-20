namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Collections.Generic;
    using System.Data;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class ClaimAccountServiceTests
    {
        private const string DefaultEmail = "test@email.com";
        private const int DefaultUserId = 2;
        private const int DefaultCentreId = 7;
        private const string DefaultCentreName = "Centre";
        private const string DefaultPasswordHash = "hash";
        private const string DefaultCandidateNumber = "CN777";
        private const string DefaultSupportEmail = "support@email.com";
        private IUserDataService userDataService = null!;
        private IConfigDataService configDataService = null!;
        private ClaimAccountService claimAccountService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            configDataService = A.Fake<IConfigDataService>();
            claimAccountService = new ClaimAccountService(userDataService, configDataService);
        }

        [Test]
        [TestCase(false, false, "")]
        [TestCase(false, false, DefaultPasswordHash)]
        [TestCase(true, false, "")]
        [TestCase(true, false, DefaultPasswordHash)]
        [TestCase(true, true, "")]
        [TestCase(true, true, DefaultPasswordHash)]
        public void GetViewModelForClaimAccountJourney_returns_expected_model(
            bool emailIsTaken,
            bool emailIsTakenByActiveUser,
            string passwordHash
        )
        {
            // Given
            var existingUserOwningEmail =
                emailIsTaken ? UserTestHelper.GetDefaultUserAccount(active: emailIsTakenByActiveUser) : null;
            var userAccountToBeClaimed = UserTestHelper.GetDefaultUserAccount(passwordHash: passwordHash);
            var delegateAccountToBeClaimed = UserTestHelper.GetDefaultDelegateAccount(
                candidateNumber: DefaultCandidateNumber,
                centreId: DefaultCentreId
            );
            A.CallTo(() => userDataService.GetUserAccountByPrimaryEmail(DefaultEmail)).Returns(existingUserOwningEmail);
            A.CallTo(() => userDataService.GetUserAccountById(DefaultUserId)).Returns(userAccountToBeClaimed);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId))
                .Returns(new List<DelegateAccount> { delegateAccountToBeClaimed });
            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.SupportEmail))
                .Returns(DefaultSupportEmail);

            // When
            var result = claimAccountService.GetViewModelForClaimAccountJourney(
                DefaultUserId,
                DefaultCentreId,
                DefaultCentreName,
                DefaultEmail
            );

            // Then
            result.Should().BeEquivalentTo(
                new ClaimAccountViewModel
                {
                    UserId = DefaultUserId,
                    CentreId = DefaultCentreId,
                    CentreName = DefaultCentreName,
                    CentreSpecificEmail = DefaultEmail,
                    CandidateNumber = DefaultCandidateNumber,
                    SupportEmail = DefaultSupportEmail,
                    EmailIsTaken = emailIsTaken,
                    EmailIsTakenByActiveUser = emailIsTakenByActiveUser,
                    PasswordSet = !string.IsNullOrWhiteSpace(passwordHash),
                }
            );
        }

        [Test]
        public void ConvertTemporaryUserToConfirmedUser_sets_expected_data()
        {
            // When
            claimAccountService.ConvertTemporaryUserToConfirmedUser(DefaultUserId, DefaultCentreId, DefaultEmail);

            // Then
            A.CallTo(() => userDataService.SetPrimaryEmailAndActivate(DefaultUserId, DefaultEmail))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetCentreEmail(DefaultUserId, DefaultCentreId, null, A<IDbTransaction?>._))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetRegistrationConfirmationHash(DefaultUserId, DefaultCentreId, null))
                .MustHaveHappenedOnceExactly();
        }
    }
}
