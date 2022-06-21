namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class UserVerificationServiceTests
    {
        private ICryptoService cryptoService = null!;
        private IUserDataService userDataService = null!;
        private IUserVerificationService userVerificationService = null!;

        [SetUp]
        public void Setup()
        {
            cryptoService = A.Fake<ICryptoService>();
            userDataService = A.Fake<IUserDataService>();

            userVerificationService = new UserVerificationService(cryptoService, userDataService);
        }

        [Test]
        public void VerifyUserEntity_returns_successful_result_when_password_matches_all_accounts()
        {
            // Given
            const string password = "password";
            const string hashedPassword = "hashedpassword";
            var delegateAccounts = Builder<DelegateAccount>.CreateListOfSize(5)
                .All()
                .With(da => da.OldPassword = hashedPassword)
                .Build();
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(passwordHash: hashedPassword),
                new List<AdminAccount>(),
                delegateAccounts
            );
            A.CallTo(() => cryptoService.VerifyHashedPassword(hashedPassword, password)).Returns(true);

            // When
            var result = userVerificationService.VerifyUserEntity(password, userEntity);

            // Then
            using (new AssertionScope())
            {
                result.UserAccountPassedVerification.Should().BeTrue();
                result.FailedVerificationDelegateAccountIds.Should().BeEmpty();
                result.PassedVerificationDelegateAccountIds.Should()
                    .BeEquivalentTo(delegateAccounts.Select(da => da.Id));
                result.PasswordMatchesAllAccountPasswords.Should().BeTrue();
                result.PasswordMatchesAtLeastOneAccountPassword.Should().BeTrue();
            }
        }

        [Test]
        public void VerifyUserEntity_returns_successful_result_when_delegate_passwords_are_null()
        {
            // Given
            const string password = "password";
            const string hashedPassword = "hashedpassword";
            var delegateAccounts = Builder<DelegateAccount>.CreateListOfSize(5)
                .All()
                .With(da => da.OldPassword = null)
                .Build();
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(passwordHash: hashedPassword),
                new List<AdminAccount>(),
                delegateAccounts
            );
            A.CallTo(() => cryptoService.VerifyHashedPassword(hashedPassword, password)).Returns(true);

            // When
            var result = userVerificationService.VerifyUserEntity(password, userEntity);

            // Then
            using (new AssertionScope())
            {
                result.UserAccountPassedVerification.Should().BeTrue();
                result.FailedVerificationDelegateAccountIds.Should().BeEmpty();
                result.DelegateAccountsWithNoPassword.Should()
                    .BeEquivalentTo(delegateAccounts.Select(da => da.Id));
                result.PasswordMatchesAllAccountPasswords.Should().BeTrue();
                result.PasswordMatchesAtLeastOneAccountPassword.Should().BeTrue();
            }
        }

        [Test]
        public void VerifyUserEntity_returns_partially_successful_result_when_password_matches_some_delegate_accounts()
        {
            // Given
            const string password = "password";
            const string hashedPassword = "hashedPassword";
            const string incorrectHashedPassword = "incorrectHashedPassword";
            var delegateAccounts = Builder<DelegateAccount>.CreateListOfSize(5)
                .TheFirst(2)
                .With(da => da.OldPassword = hashedPassword)
                .TheRest()
                .With(da => da.OldPassword = incorrectHashedPassword)
                .Build();
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(passwordHash: hashedPassword),
                new List<AdminAccount>(),
                delegateAccounts
            );
            A.CallTo(() => cryptoService.VerifyHashedPassword(hashedPassword, password)).Returns(true);
            A.CallTo(() => cryptoService.VerifyHashedPassword(incorrectHashedPassword, password)).Returns(false);

            // When
            var result = userVerificationService.VerifyUserEntity(password, userEntity);

            // Then
            using (new AssertionScope())
            {
                result.UserAccountPassedVerification.Should().BeTrue();
                result.FailedVerificationDelegateAccountIds.Should().HaveCount(3);
                result.PassedVerificationDelegateAccountIds.Should().HaveCount(2);
                result.PasswordMatchesAllAccountPasswords.Should().BeFalse();
                result.PasswordMatchesAtLeastOneAccountPassword.Should().BeTrue();
            }
        }

        [Test]
        public void VerifyUserEntity_returns_unsuccessful_result_when_password_matches_no_accounts()
        {
            // Given
            const string password = "password";
            const string hashedPassword = "hashedpassword";
            var delegateAccounts = Builder<DelegateAccount>.CreateListOfSize(5)
                .All()
                .With(da => da.OldPassword = hashedPassword)
                .Build();
            var userEntity = new UserEntity(
                UserTestHelper.GetDefaultUserAccount(passwordHash: hashedPassword),
                new List<AdminAccount>(),
                delegateAccounts
            );
            A.CallTo(() => cryptoService.VerifyHashedPassword(hashedPassword, password)).Returns(false);

            // When
            var result = userVerificationService.VerifyUserEntity(password, userEntity);

            // Then
            using (new AssertionScope())
            {
                result.UserAccountPassedVerification.Should().BeFalse();
                result.FailedVerificationDelegateAccountIds.Should()
                    .BeEquivalentTo(delegateAccounts.Select(da => da.Id));
                result.PassedVerificationDelegateAccountIds.Should().BeEmpty();
                result.PasswordMatchesAllAccountPasswords.Should().BeFalse();
                result.PasswordMatchesAtLeastOneAccountPassword.Should().BeFalse();
            }
        }

        [Test]
        public void IsPasswordValid_Returns_true_when_password_and_user_id_match()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);
            var user = UserTestHelper.GetDefaultUserAccount();

            // When
            var isPasswordValid = userVerificationService.IsPasswordValid(user.PasswordHash, user.Id);

            // Then
            isPasswordValid.Should().BeTrue();
        }

        [Test]
        public void IsPasswordValid_Returns_false_when_password_and_user_id_do_not_match()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            var user = UserTestHelper.GetDefaultUserAccount();

            // When
            var isPasswordValid = userVerificationService.IsPasswordValid(user.PasswordHash, user.Id);

            // Then
            isPasswordValid.Should().BeFalse();
        }

        [Test]
        public void IsPasswordValid_Returns_false_when_password_is_null()
        {
            // When
            var isPasswordValid = userVerificationService.IsPasswordValid(null, 1);

            // Then
            isPasswordValid.Should().BeFalse();
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).MustNotHaveHappened();
        }

        [Test]
        public void IsPasswordValid_Returns_false_when_user_id_is_null()
        {
            // When
            var isPasswordValid = userVerificationService.IsPasswordValid("password", null);

            // Then
            isPasswordValid.Should().BeFalse();
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).MustNotHaveHappened();
        }
    }
}
