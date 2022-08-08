namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount;
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
        private IPasswordService passwordService = null!;
        private ClaimAccountService claimAccountService = null!;

        [SetUp]
        public void Setup()
        {
            userDataService = A.Fake<IUserDataService>();
            configDataService = A.Fake<IConfigDataService>();
            passwordService = A.Fake<IPasswordService>();
            claimAccountService = new ClaimAccountService(userDataService, configDataService, passwordService);
        }

        [Test]
        public void GetAccountDetailsForCompletingRegistration_returns_expected_model()
        {
            // Given
            var delegateAccountToBeClaimed = UserTestHelper.GetDefaultDelegateAccount(
                candidateNumber: DefaultCandidateNumber,
                centreId: DefaultCentreId
            );

            A.CallTo(() => userDataService.GetUserAccountByPrimaryEmail(DefaultEmail)).Returns(null);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId))
                .Returns(new List<DelegateAccount> { delegateAccountToBeClaimed });
            A.CallTo(() => configDataService.GetConfigValue(ConfigDataService.SupportEmail))
                .Returns(DefaultSupportEmail);

            // When
            var result = claimAccountService.GetAccountDetailsForClaimAccount(
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
                    Email = DefaultEmail,
                    CandidateNumber = DefaultCandidateNumber,
                    SupportEmail = DefaultSupportEmail,
                    IdOfUserMatchingEmailIfAny = null,
                    UserMatchingEmailIsActive = false,
                    WasPasswordSetByAdmin = false,
                }
            );
        }

        [Test]
        [TestCase(null, true)]
        [TestCase(null, false)]
        [TestCase(DefaultUserId, true)]
        [TestCase(DefaultUserId, false)]
        [TestCase(DefaultUserId + 1, true)]
        [TestCase(DefaultUserId + 1, false)]
        public void
            GetAccountDetailsForCompletingRegistration_returns_model_with_correct_EmailIsTaken(
                int? loggedInUserId,
                bool otherUserWithEmailExists
            )
        {
            // Given
            var userAccountMatchingEmail = otherUserWithEmailExists ? UserTestHelper.GetDefaultUserAccount() : null;
            var delegateAccountToBeClaimed = UserTestHelper.GetDefaultDelegateAccount(
                candidateNumber: DefaultCandidateNumber,
                centreId: DefaultCentreId
            );

            A.CallTo(() => userDataService.GetUserAccountByPrimaryEmail(DefaultEmail))
                .Returns(userAccountMatchingEmail);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId))
                .Returns(new List<DelegateAccount> { delegateAccountToBeClaimed });

            // When
            var result = claimAccountService.GetAccountDetailsForClaimAccount(
                DefaultUserId,
                DefaultCentreId,
                DefaultCentreName,
                DefaultEmail,
                loggedInUserId
            );

            // Then
            result.IdOfUserMatchingEmailIfAny.Should().Be(userAccountMatchingEmail?.Id);
        }

        [Test]
        [TestCase(true, true)]
        [TestCase(false, false)]
        public void
            GetAccountDetailsForCompletingRegistration_returns_model_with_correct_EmailIsTakenByActiveUser(
                bool otherUserWithEmailIsActive,
                bool expectedUserMatchingEmailIsActive
            )
        {
            // Given
            var userAccountMatchingEmail = UserTestHelper.GetDefaultUserAccount(active: otherUserWithEmailIsActive);
            var delegateAccountToBeClaimed = UserTestHelper.GetDefaultDelegateAccount(
                candidateNumber: DefaultCandidateNumber,
                centreId: DefaultCentreId
            );

            A.CallTo(() => userDataService.GetUserAccountByPrimaryEmail(DefaultEmail))
                .Returns(userAccountMatchingEmail);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId))
                .Returns(new List<DelegateAccount> { delegateAccountToBeClaimed });

            // When
            var result = claimAccountService.GetAccountDetailsForClaimAccount(
                DefaultUserId,
                DefaultCentreId,
                DefaultCentreName,
                DefaultEmail
            );

            // Then
            result.UserMatchingEmailIsActive.Should().Be(expectedUserMatchingEmailIsActive);
        }

        [Test]
        [TestCase(DefaultPasswordHash, true)]
        [TestCase("", false)]
        public void GetAccountDetailsForCompletingRegistration_returns_model_with_correct_PasswordSet(
            string passwordHash,
            bool expectedPasswordSet
        )
        {
            // Given
            var userAccountToBeClaimed = UserTestHelper.GetDefaultUserAccount(passwordHash: passwordHash);
            var delegateAccountToBeClaimed = UserTestHelper.GetDefaultDelegateAccount(
                candidateNumber: DefaultCandidateNumber,
                centreId: DefaultCentreId
            );

            A.CallTo(() => userDataService.GetUserAccountById(DefaultUserId)).Returns(userAccountToBeClaimed);
            A.CallTo(() => userDataService.GetDelegateAccountsByUserId(DefaultUserId))
                .Returns(new List<DelegateAccount> { delegateAccountToBeClaimed });

            // When
            var result = claimAccountService.GetAccountDetailsForClaimAccount(
                DefaultUserId,
                DefaultCentreId,
                DefaultCentreName,
                DefaultEmail
            );

            // Then
            result.WasPasswordSetByAdmin.Should().Be(expectedPasswordSet);
        }

        [Test]
        public async Task ConvertTemporaryUserToConfirmedUser_calls_expected_services_when_password_is_null()
        {
            // When
            await claimAccountService.ConvertTemporaryUserToConfirmedUser(
                DefaultUserId,
                DefaultCentreId,
                DefaultEmail,
                null
            );

            // Then
            A.CallTo(() => userDataService.SetPrimaryEmailAndActivate(DefaultUserId, DefaultEmail))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                    () => userDataService.SetCentreEmail(
                        DefaultUserId,
                        DefaultCentreId,
                        null,
                        A<IDbTransaction?>._
                    )
                )
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetRegistrationConfirmationHash(DefaultUserId, DefaultCentreId, null))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => passwordService.ChangePasswordAsync(A<int>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task ConvertTemporaryUserToConfirmedUser_calls_expected_services_when_password_is_not_null()
        {
            // Given
            const string password = "password";

            // When
            await claimAccountService.ConvertTemporaryUserToConfirmedUser(
                DefaultUserId,
                DefaultCentreId,
                DefaultEmail,
                password
            );

            // Then
            A.CallTo(() => userDataService.SetPrimaryEmailAndActivate(DefaultUserId, DefaultEmail))
                .MustHaveHappenedOnceExactly();
            A.CallTo(
                    () => userDataService.SetCentreEmail(
                        DefaultUserId,
                        DefaultCentreId,
                        null,
                        A<IDbTransaction?>._
                    )
                )
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.SetRegistrationConfirmationHash(DefaultUserId, DefaultCentreId, null))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => passwordService.ChangePasswordAsync(DefaultUserId, password)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void LinkAccount_calls_data_services()
        {
            // Given
            var newUserId = DefaultUserId + 1;

            // When
            claimAccountService.LinkAccount(DefaultUserId, newUserId, DefaultCentreId);

            // Then
            A.CallTo(() => userDataService.LinkDelegateAccountToNewUser(DefaultUserId, newUserId, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.LinkUserCentreDetailsToNewUser(DefaultUserId, newUserId, DefaultCentreId))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => userDataService.DeleteUser(DefaultUserId)).MustHaveHappenedOnceExactly();
        }
    }
}
