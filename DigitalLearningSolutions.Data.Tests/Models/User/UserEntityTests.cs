namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserEntityTests
    {
        [Test]
        [TestCase(0, false)]
        [TestCase(4, false)]
        [TestCase(5, true)]
        [TestCase(10, true)]
        public void IsLocked_returns_expected_values(int failedLoginCount, bool expectedValue)
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(failedLoginCount: failedLoginCount);

            // When
            var result = new UserEntity(
                userAccount,
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount() },
                new List<DelegateAccount>()
            );

            // Then
            result.IsLocked.Should().Be(expectedValue);
        }

        [Test]
        public void IsLocked_returns_false_with_no_admin_accounts()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(failedLoginCount: 5);

            // When
            var result = new UserEntity(
                userAccount,
                new List<AdminAccount>(),
                new List<DelegateAccount>()
            );

            // Then
            result.IsLocked.Should().BeFalse();
        }

        [Test]
        public void IsSingleCentreAccount_returns_false_with_multiple_delegate_accounts()
        {
            // Given
            var delegateAccounts = Builder<DelegateAccount>.CreateListOfSize(5).Build();
            var userEntity = new UserEntity(new UserAccount(), new List<AdminAccount>(), delegateAccounts);

            // When
            var result = userEntity.IsSingleCentreAccount();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsSingleCentreAccount_returns_false_with_multiple_admin_accounts()
        {
            // Given
            var adminAccounts = Builder<AdminAccount>.CreateListOfSize(5).Build();
            var userEntity = new UserEntity(new UserAccount(), adminAccounts, new List<DelegateAccount>());

            // When
            var result = userEntity.IsSingleCentreAccount();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsSingleCentreAccount_returns_false_with_single_admin_and_delegate_at_different_centres()
        {
            // Given
            var userEntity = new UserEntity(
                new UserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount(centreId: 101) },
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount() }
            );

            // When
            var result = userEntity.IsSingleCentreAccount();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void IsSingleCentreAccount_returns_true_with_single_admin_and_delegate_at_the_same_centre()
        {
            // Given
            var userEntity = new UserEntity(
                new UserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount() },
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount() }
            );

            // When
            var result = userEntity.IsSingleCentreAccount();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void IsSingleCentreAccount_returns_true_with_single_admin_and_no_delegates()
        {
            // Given
            var userEntity = new UserEntity(
                new UserAccount(),
                new List<AdminAccount> { UserTestHelper.GetDefaultAdminAccount() },
                new List<DelegateAccount>()
            );

            // When
            var result = userEntity.IsSingleCentreAccount();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void IsSingleCentreAccount_returns_true_with_single_delegate_and_no_admins()
        {
            // Given
            var userEntity = new UserEntity(
                new UserAccount(),
                new List<AdminAccount>(),
                new List<DelegateAccount> { UserTestHelper.GetDefaultDelegateAccount() }
            );

            // When
            var result = userEntity.IsSingleCentreAccount();

            // Then
            result.Should().BeTrue();
        }
    }
}
