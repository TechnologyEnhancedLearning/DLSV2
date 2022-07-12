namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
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
        public void VerifyUsers_Returns_verified_admin_user()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);
            var adminUser = UserTestHelper.GetDefaultAdminUser(password: "Automatically Verified");

            // When
            var (verifiedAdminUser, _) = userVerificationService.VerifyUsers(
                "password",
                adminUser,
                new List<DelegateUser>()
            );

            // Then
            Assert.AreEqual(adminUser, verifiedAdminUser);
        }

        [Test]
        public void VerifyUsers_Does_not_return_unverified_admin_user()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            var adminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var (verifiedAdminUser, _) = userVerificationService.VerifyUsers(
                "password",
                adminUser,
                new List<DelegateUser>()
            );

            // Then
            Assert.IsNull(verifiedAdminUser);
        }

        [Test]
        public void VerifyUsers_Returns_verified_delegate_users()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);

            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Fails Verification", id: 2);
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var adminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var (_, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                "password",
                adminUser,
                delegateUsers
            );

            // Then
            Assert.Contains(firstDelegateUser, verifiedDelegateUsers);
            Assert.Contains(thirdDelegateUser, verifiedDelegateUsers);
        }

        [Test]
        public void VerifyUsers_Filters_out_unverified_delegate_users()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);

            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Fails Verification", id: 2);
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(password: "Automatically Verified", id: 3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var adminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var (_, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                "password",
                adminUser,
                delegateUsers
            );

            // Then
            Assert.IsFalse(verifiedDelegateUsers.Contains(secondDelegateUser));
        }

        [Test]
        public void VerifyUsers_Returns_no_delegates_if_all_unverified()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            var firstDelegateUser = UserTestHelper.GetDefaultDelegateUser(1);
            var secondDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var thirdDelegateUser = UserTestHelper.GetDefaultDelegateUser(3);

            var delegateUsers = new List<DelegateUser>
            {
                firstDelegateUser,
                secondDelegateUser,
                thirdDelegateUser
            };

            var adminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var (_, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                "password",
                adminUser,
                delegateUsers
            );

            // Then
            Assert.IsEmpty(verifiedDelegateUsers);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_delegate_email_is_empty()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: null);
            var delegateUsers = new List<DelegateUser> { delegateUser };

            // When
            var returnedAdminUser = userVerificationService.GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_no_admin_account_is_associated_with_delegate()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(null);

            // When
            var returnedAdminUser = userVerificationService.GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_admin_account_is_not_active()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser(active: false);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(associatedAdminUser);

            // When
            var returnedAdminUser = userVerificationService.GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_admin_account_is_not_approved()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser(approved: false);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(associatedAdminUser);

            // When
            var returnedAdminUser = userVerificationService.GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }
        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUser_Returns_verified_admin_account_associated_with_delegate_by_email()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var delegateUsers = new List<DelegateUser> { delegateUser };
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = userVerificationService.GetActiveApprovedVerifiedAdminUserAssociatedWithDelegateUsers(
                delegateUsers,
                "password"
            );

            // Then
            Assert.AreEqual(associatedAdminUser, returnedAdminUser);
        }

        [Test]
        public void GetVerifiedDelegateUsersAssociatedWithAdminUser_Returns_empty_list_when_admin_is_null()
        {
            // Given
            AdminUser? adminUser = null;
            const string password = "password";

            // When
            var returnedDelegates = userVerificationService.GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                password
            );

            // Then
            Assert.IsEmpty(returnedDelegates);
        }

        [Test]
        public void
            GetVerifiedDelegateUsersAssociatedWithAdminUser_Returns_empty_list_when_delegate_account_password_doesnt_match()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var associatedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(active: false) };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            // When
            var returnedDelegates = userVerificationService.GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                "password"
            );

            // Then
            Assert.IsEmpty(returnedDelegates);
        }

        [Test]
        public void
            GetVerifiedDelegateUsersAssociatedWithAdminUser_Returns_empty_list_when_delegate_account_found_is_not_active()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var associatedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(active: false) };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedDelegates = userVerificationService.GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                "password"
            );

            // Then
            Assert.IsEmpty(returnedDelegates);
        }

        [Test]
        public void
            GetVerifiedDelegateUsersAssociatedWithAdminUser_Returns_empty_list_when_delegate_account_found_is_not_approved()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var associatedDelegateUsers = new List<DelegateUser>
                { UserTestHelper.GetDefaultDelegateUser(approved: false) };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedDelegates = userVerificationService.GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                "password"
            );

            // Then
            Assert.IsEmpty(returnedDelegates);
        }

        [Test]
        public void
            GetVerifiedDelegateUsersAssociatedWithAdminUser_Returns_verified_delegate_account_associated_with_admin_by_email()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var associatedDelegateUsers = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser() };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedDelegates = userVerificationService.GetActiveApprovedVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                "password"
            );

            // Then
            Assert.AreEqual(associatedDelegateUsers, returnedDelegates);
        }
    }
}
