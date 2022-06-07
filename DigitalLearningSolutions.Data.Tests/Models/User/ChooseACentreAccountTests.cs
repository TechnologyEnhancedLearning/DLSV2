namespace DigitalLearningSolutions.Data.Tests.Models.User
{
    using DigitalLearningSolutions.Data.Models.User;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class ChooseACentreAccountTests
    {
        // The following are not valid states, but are listed here anyway, commented out, for completeness
        // - !isAdmin && !isDelegate
        // - !isDelegate && isDelegateApproved
        // - !isDelegate && isDelegateActive

        [Test]
        // Active centre test cases
        [TestCase(true, true, true, true, true, "Active", "Choose")]
        [TestCase(true, false, true, true, true, "Active", "Choose")]
        // [TestCase(true, true, false, true, true)] - !isDelegate && isDelegateApproved
        [TestCase(true, true, true, false, true, "Delegate unapproved", "Choose")]
        [TestCase(true, true, true, true, false, "Delegate inactive", "Choose")]
        // [TestCase(true, false, false, true, true)]- !isAdmin && !isDelegate && isDelegateApproved && isDelegateActive
        [TestCase(true, false, true, false, true, "Unapproved")]
        [TestCase(true, false, true, true, false, "Inactive", "Reactivate")]
        // [TestCase(true, true, false, false, true)] - !isDelegate && isDelegateActive
        // [TestCase(true, true, false, true, false)] - !isDelegate && isDelegateApproved
        [TestCase(true, true, true, false, false, "Delegate inactive", "Choose")]
        [TestCase(true, true, false, false, false, "Active", "Choose")]
        [TestCase(true, false, true, false, false, "Inactive", "Reactivate")]
        // [TestCase(true, false, false, true, false)] - !isAdmin && !isDelegate && isDelegateApproved
        // [TestCase(true, false, false, false, true)] - !isAdmin && !isDelegate && isDelegateActive
        // [TestCase(true, false, false, false, false)] - !isAdmin && !isDelegate

        // Inactive centre test cases
        [TestCase(false, true, true, true, true, "Centre inactive")]
        [TestCase(false, false, true, true, true, "Centre inactive")]
        // [TestCase(false, true, false, true, true)] - !isDelegate && isDelegateApproved
        [TestCase(false, true, true, false, true, "Centre inactive")]
        [TestCase(false, true, true, true, false, "Centre inactive")]
        // [TestCase(false, false, false, true, true)]- !isAdmin && !isDelegate && isDelegateApproved && isDelegateActive
        [TestCase(false, false, true, false, true, "Centre inactive")]
        [TestCase(false, false, true, true, false, "Centre inactive")]
        // [TestCase(false, true, false, false, true)] - !isDelegate && isDelegateActive
        // [TestCase(false, true, false, true, false)] - !isDelegate && isDelegateApproved
        [TestCase(false, true, true, false, false, "Centre inactive")]
        [TestCase(false, true, false, false, false, "Centre inactive")]
        [TestCase(false, false, true, false, false, "Centre inactive")]
        // [TestCase(false, false, false, true, false)] - !isAdmin && !isDelegate && isDelegateApproved
        // [TestCase(false, false, false, false, true)] - !isAdmin && !isDelegate && isDelegateActive
        // [TestCase(false, false, false, false, false)] - !isAdmin && !isDelegate
        [TestCase(false, true, true, true, true, "Centre inactive")]
        // [TestCase(false, false, true, true, true)] - isAdmin && !isApproved
        [TestCase(false, true, false, true, true, "Centre inactive")]
        // [TestCase(false, true, true, false, true)] - !isDelegate && isDelegateActive
        [TestCase(false, true, true, true, false, "Centre inactive")]
        [TestCase(false, false, false, true, true, "Centre inactive")]
        // [TestCase(false, false, true, false, true)] - isAdmin && !isApproved
        // [TestCase(false, false, true, true, false)] - isAdmin && !isApproved
        // [TestCase(false, true, false, false, true)] - !isAdmin && !isDelegate && isDelegateActive
        [TestCase(false, true, false, true, false, "Centre inactive")]
        [TestCase(false, true, true, false, false, "Centre inactive")]
        // [TestCase(false, true, false, false, false)] - !isAdmin && !isDelegate
        // [TestCase(false, false, true, false, false)] - isAdmin && !isApproved
        [TestCase(false, false, false, true, false, "Centre inactive")]
        // [TestCase(false, false, false, false, true)] - !isAdmin && !isDelegate && isDelegateActive
        // [TestCase(false, false, false, false, false)] - !isAdmin && !isDelegate
        public void StatusTag_and_ActionButton_return_expected_values(
            bool isCentreActive,
            bool isAdmin,
            bool isDelegate,
            bool isDelegateApproved,
            bool isDelegateActive,
            string expectedStatus,
            string? expectedAction = null
        )
        {
            // When
            var result = new ChooseACentreAccount(
                1,
                "",
                isCentreActive,
                isAdmin,
                isDelegate,
                isDelegateApproved,
                isDelegateActive
            );

            // Then
            using (new AssertionScope())
            {
                result.StatusTag["Label"].Should().Be(expectedStatus);
                result.ActionButton.Should().Be(expectedAction);
            }
        }
    }
}
