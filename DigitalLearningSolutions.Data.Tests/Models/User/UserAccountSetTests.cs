namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserAccountSetTests
    {
        [Test]
        public void Any_returns_false_if_adminUser_null_and_delegateUsers_empty()
        {
            // Given
            AdminUser? adminUser = null;
            var delegates = new List<DelegateUser>();

            // When
            var result = new UserAccountSet(adminUser, delegates).Any();

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
            AdminUser? adminUser = null;
            var delegates = new List<DelegateUser> { Builder<DelegateUser>.CreateNew().Build() };

            // When
            var result = new UserAccountSet(adminUser, delegates).Any();

            // Then
            result.Should().BeTrue();
        }
    }
}
