namespace DigitalLearningSolutions.Web.Tests.ViewModels.VerifyEmail
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.ViewModels.VerifyEmail;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class VerifyYourEmailViewModelTests
    {
        private static readonly object[] SourceParams =
        {
            new object?[]
            {
                null,
                new List<(int centreId, string centreName, string centreEmail)> { (1, "centre1", "centre@1.email") },
                1,
                true,
                new List<(int centreId, string centreName, string centreEmail)>(),
            },
            new object?[]
            {
                "primary@email.com",
                new List<(int centreId, string centreName, string centreEmail)>(),
                1,
                true,
                new List<(int centreId, string centreName, string centreEmail)>(),
            },
            new object?[]
            {
                "primary@email.com",
                new List<(int centreId, string centreName, string centreEmail)> { (1, "centre1", "centre@1.email") },
                2,
                false,
                new List<(int centreId, string centreName, string centreEmail)> { (1, "centre1", "centre@1.email") },
            },
            new object?[]
            {
                null,
                new List<(int centreId, string centreName, string centreEmail)>
                    { (1, "centre1", "centre@1.email"), (2, "centre2", "centre@2.email") },
                2,
                false,
                new List<(int centreId, string centreName, string centreEmail)> { (2, "centre2", "centre@2.email") },
            },
        };

        [Test]
        [TestCaseSource(nameof(SourceParams))]
        public void VerifyEmailViewModel_populates_expected_values(
            string? primaryEmail,
            List<(int centreId, string centreName, string centreEmail)> centreSpecificEmails,
            int unverifiedEmailsCount,
            bool singleUnverifiedEmail,
            List<(int centreId, string centreName, string centreEmail)> centreEmailsExcludingFirstParagraph
        )
        {
            // When
            var model = new VerifyYourEmailViewModel(
                EmailVerificationReason.EmailNotVerified,
                primaryEmail,
                centreSpecificEmails
            );

            // Then
            using (new AssertionScope())
            {
                model.EmailVerificationReason.Should().BeEquivalentTo(EmailVerificationReason.EmailNotVerified);
                model.PrimaryEmail.Should().BeEquivalentTo(primaryEmail);
                model.CentreSpecificEmails.Should().BeEquivalentTo(centreSpecificEmails);
                model.UnverifiedEmailsCount.Should().Be(unverifiedEmailsCount);
                model.SingleUnverifiedEmail.Should().Be(singleUnverifiedEmail);
                model.CentreEmailsExcludingFirstParagraph.Should().BeEquivalentTo(centreEmailsExcludingFirstParagraph);
            }
        }
    }
}
