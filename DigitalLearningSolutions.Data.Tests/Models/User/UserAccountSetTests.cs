namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserAccountSetTests
    {
        [Test]
        public void Any_returns_false_if_adminUser_null_and_delegateUsers_empty()
        {
            // When
            var result = new UserAccountSet().Any();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void Any_returns_true_if_adminUser_not_null()
        {
            // Given
            var adminUser = Builder<AdminUser>.CreateNew().Build();
            var delegates = new List<DelegateUser>();

            // When
            var result = new UserAccountSet(adminUser, delegates).Any();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Any_returns_true_if_delegateUsers_not_empty()
        {
            // Given
            var delegates = new List<DelegateUser> { Builder<DelegateUser>.CreateNew().Build() };

            // When
            var result = new UserAccountSet(null, delegates).Any();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Getting_user_refs_returns_refs_for_exact_users_in_set()
        {
            // Given
            var delegateIds = new[] { 1, 8, 27, 64 };
            var adminId = 234;

            var adminAccount = Builder<AdminUser>.CreateNew().With(user => user.Id = adminId).Build();
            var delegateAccounts = delegateIds.Select(
                id => Builder<DelegateUser>.CreateNew().With(user => user.Id = id).Build()
            );

            // When
            var userRefs = new UserAccountSet(adminAccount, delegateAccounts).GetUserRefs();

            // Then
            userRefs.Should().Contain(delegateIds.Select(id => new UserReference(id, UserType.DelegateUser)));
            userRefs.Should().Contain(new UserReference(adminId, UserType.AdminUser));
            userRefs.Count.Should().Be(5);
        }

        [Test]
        public void Getting_user_refs_succeeds_when_no_admin()
        {
            // Given
            var delegateIds = new[] { 1, 8, 27, 64 };

            var delegateAccounts = delegateIds.Select(
                id => Builder<DelegateUser>.CreateNew().With(user => user.Id = id).Build()
            );

            // When
            var userRefs = new UserAccountSet(null, delegateAccounts).GetUserRefs();

            // Then
            userRefs.Should().Contain(delegateIds.Select(id => new UserReference(id, UserType.DelegateUser)));
            userRefs.Count.Should().Be(4);
        }

        [Test]
        public void Getting_user_refs_succeeds_when_no_delegates()
        {
            // Given
            var adminId = 234;
            var adminAccount = Builder<AdminUser>.CreateNew().With(user => user.Id = adminId).Build();

            // When
            var userRefs = new UserAccountSet(adminAccount, new DelegateUser[] { }).GetUserRefs();

            // Then
            userRefs.Should().Contain(new UserReference(adminId, UserType.AdminUser));
            userRefs.Count.Should().Be(1);
        }
    }
}
