namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using NUnit.Framework;

    public class UserAccountsHelperTests
    {
        [Test]
        public void Any_returns_false_if_adminUser_null_and_candidateUsers_empty()
        {
            // Given
            AdminUser? adminUser = null;
            var candidates = new List<DelegateUser>();

            // When
            var result = (adminUser, candidates).Any();

            // Then
            result.Should().BeFalse();
        }

        [Test]
        public void Any_returns_true_if_adminUser_not_null()
        {
            // Given
            var adminUser = Builder<AdminUser>.CreateNew().Build();
            var candidates = new List<DelegateUser>();

            // When
            var result = (adminUser, candidates).Any();

            // Then
            result.Should().BeTrue();
        }

        [Test]
        public void Any_returns_true_if_candidateUsers_not_empty()
        {
            // Given
            AdminUser? adminUser = null;
            var candidates = new List<DelegateUser> { Builder<DelegateUser>.CreateNew().Build() };

            // When
            var result = (adminUser, candidates).Any();

            // Then
            result.Should().BeTrue();
        }
    }
}
