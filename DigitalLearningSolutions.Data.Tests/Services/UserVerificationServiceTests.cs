﻿namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
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
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            A.CallTo(() => cryptoService.VerifyHashedPassword("Automatically Verified", A<string>._)).Returns(true);
            var adminUser = UserTestHelper.GetDefaultAdminUser(password: "Automatically Verified");
            var adminUsers = new List<AdminUser> { adminUser };

            //When
            var (verifiedAdminUsers, _) = userVerificationService.VerifyUsers(
                "password",
                adminUsers,
                new List<DelegateUser>()
            );

            // Then
            Assert.AreEqual(adminUser, verifiedAdminUsers.First());
        }

        [Test]
        public void VerifyUsers_Does_not_return_unverified_admin_user()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var adminUsers = new List<AdminUser> { adminUser };

            //When
            var (verifiedAdminUsers, _) = userVerificationService.VerifyUsers(
                "password",
                adminUsers,
                new List<DelegateUser>()
            );

            // Then
            Assert.IsEmpty(verifiedAdminUsers);
        }

        [Test]
        public void VerifyUsers__Returns_verified_delegate_users()
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

            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                "password",
                adminUsers,
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

            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                "password",
                adminUsers,
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

            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, verifiedDelegateUsers) = userVerificationService.VerifyUsers(
                "password",
                adminUsers,
                delegateUsers
            );

            // Then
            Assert.IsEmpty(verifiedDelegateUsers);
        }

        [Test]
        public void VerifyUsers_Returns_no_delegate_users_when_delegate_stored_password_is_empty()
        {
            // Given
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);
            var delegateUserWithoutPassword = UserTestHelper.GetDefaultDelegateUser(password: string.Empty);
            var adminUsers = new List<AdminUser> { UserTestHelper.GetDefaultAdminUser() };

            //When
            var (_, returnedDelegateList) = userVerificationService.VerifyUsers(
                "password",
                adminUsers,
                new List<DelegateUser> { delegateUserWithoutPassword }
            );

            // Then
            Assert.IsEmpty(returnedDelegateList);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_delegate_email_is_empty()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(emailAddress: null);

            // When
            var returnedAdminUser = userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUser(
                delegateUser,
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
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(A<string>._)).Returns(null);

            // When
            var returnedAdminUser = userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUser(
                delegateUser,
                "password"
            );

            // Then
            Assert.IsNull(returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedAdminUserAssociatedWithDelegateUsers_Returns_nothing_when_admin_account_found_is_at_different_centre()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser(centreId: 2);
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser(centreId: 5);
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUser(
                delegateUser,
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
            var associatedAdminUser = UserTestHelper.GetDefaultAdminUser();
            A.CallTo(() => userDataService.GetAdminUserByEmailAddress(delegateUser.EmailAddress!))
                .Returns(associatedAdminUser);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedAdminUser = userVerificationService.GetVerifiedAdminUserAssociatedWithDelegateUser(
                delegateUser,
                "password"
            );

            // Then
            Assert.AreEqual(associatedAdminUser, returnedAdminUser);
        }

        [Test]
        public void
            GetVerifiedDelegateUsersAssociatedWithAdminUser_Returns_empty_list_when_admin_is_null()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedDelegates = userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                "password"
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
            var associatedDelegateUsers = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(active: false) };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(false);

            // When
            var returnedDelegates = userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
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
            var associatedDelegateUsers = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(active: false) };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedDelegates = userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
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
            var associatedDelegateUsers = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(approved: false) };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedDelegates = userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                "password"
            );

            // Then
            Assert.IsEmpty(returnedDelegates);
        }

        [Test]
        public void
            GetVerifiedDelegateUsersAssociatedWithAdminUser_Returns_empty_list_when_delegate_account_found_is_at_different_centre()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(centreId: 2);
            var associatedDelegateUsers = new List<DelegateUser> { UserTestHelper.GetDefaultDelegateUser(centreId: 5) };
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedDelegates = userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
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
            var associatedDelegateUsers = new List<DelegateUser>{ UserTestHelper.GetDefaultDelegateUser()};
            A.CallTo(() => userDataService.GetDelegateUsersByEmailAddress(adminUser.EmailAddress!))
                .Returns(associatedDelegateUsers);
            A.CallTo(() => cryptoService.VerifyHashedPassword(A<string>._, A<string>._)).Returns(true);

            // When
            var returnedDelegates = userVerificationService.GetVerifiedDelegateUsersAssociatedWithAdminUser(
                adminUser,
                "password"
            );

            // Then
            Assert.AreEqual(associatedDelegateUsers, returnedDelegates);
        }
    }
}
