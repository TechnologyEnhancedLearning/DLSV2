﻿namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using System;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FluentAssertions;
    using NUnit.Framework;

    public class CentreAccountSetTests
    {
        [Test]
        // Note: the value of approvedDelegate has no effect when activeDelegate is null
        [TestCase(true, true, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, false, false, true)]
        [TestCase(true, null, false, true)]
        [TestCase(false, true, true, true)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, false, false)]
        [TestCase(false, null, false, false)]
        [TestCase(null, true, true, true)]
        [TestCase(null, false, true, false)]
        [TestCase(null, true, false, false)]
        [TestCase(null, false, false, false)]
        public void CanLogInToCentre_returns_expected_value_when_centre_is_active(
            bool? activeAdmin,
            bool? activeDelegate,
            bool approvedDelegate,
            bool expectedResult
        )
        {
            // When
            var result = new CentreAccountSet(
                2,
                activeAdmin == null ? null : UserTestHelper.GetDefaultAdminAccount(active: activeAdmin.Value),
                activeDelegate == null
                    ? null
                    : UserTestHelper.GetDefaultDelegateAccount(active: activeDelegate.Value, approved: approvedDelegate)
            );

            // Then
            result.CanLogInToCentre.Should().Be(expectedResult);
        }

        [Test]
        public void
            CentreAccountSet_constructor_throws_InvalidOperationException_if_admin_and_delegate_accounts_are_both_null()
        {
            // When
            void MethodBeingTested()
            {
                var _ = new CentreAccountSet(2);
            }

            // Then
            Assert.Throws<InvalidOperationException>(MethodBeingTested);
        }
    }
}
