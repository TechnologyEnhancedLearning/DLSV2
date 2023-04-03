namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Factories;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class EmailGenerationServiceTests
    {
        private EmailGenerationService emailGenerationService = null!;

        [SetUp]
        public void Setup()
        {
            emailGenerationService = new EmailGenerationService();
        }

        [TestCase(true, true, true, true, true, true, true, true)]
        [TestCase(false, false, false, false, false, false, false, false)]
        [TestCase(true, false, true, false, true, false, true, false)]
        [TestCase(false, true, false, false, false, true, false, true)]
        [TestCase(false, false, false, false, true, true, true, true)]
        [TestCase(true, true, true, true, false, false, false, false)]
        public void GenerateDelegateAdminRolesNotificationEmail_Returned_Populated_Email(
            bool isCentreAdmin,
            bool isCentreManager,
            bool isSupervisor,
            bool isNominatedSupervisor,
            bool isTrainer,
            bool isContentCreator,
            bool isCmsAdministrator,
            bool isCmsManager
            )
        {
            // Given
            const string delegateFirstName = "TestDelegateFirstName";
            const string delegateEmail = "delegate@example.com";

            const string supervisorFirstName = "TestAdminFirstName";
            const string supervisorLastName = "TestAdminFirstName";
            const string supervisorEmail = "admin@example.com";

            const string centreName = "Test Centre Name";

            const string emailHeader = "New Digital Learning Solutions permissions granted";

            // When
            Email returnedEmail = emailGenerationService.GenerateDelegateAdminRolesNotificationEmail(
                delegateFirstName,
                supervisorFirstName,
                supervisorLastName,
                supervisorEmail,
                isCentreAdmin,
                isCentreManager,
                isSupervisor,
                isNominatedSupervisor,
                isTrainer,
                isContentCreator,
                isCmsAdministrator,
                isCmsManager,
                delegateEmail,
                centreName
            );

            // Then
            returnedEmail.Subject.Should().Be(emailHeader);
            returnedEmail.To.Should().BeEquivalentTo(delegateEmail);
            returnedEmail.Cc.Should().BeEquivalentTo(supervisorEmail);

            returnedEmail.Body.HtmlBody.Should().Contain("has granted you new access permissions for the centre " + centreName + " in the Digital Learning Solutions system.");
            returnedEmail.Body.TextBody.Should().Contain("has granted you new access permissions for the centre " + centreName + " in the Digital Learning Solutions system.");

            if (isCentreAdmin)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Centre administrator</li>");
                returnedEmail.Body.TextBody.Should().Contain("Centre administrator");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Centre administrator</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Centre administrator");
            }

            if (isCentreManager)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Centre manager</li>");
                returnedEmail.Body.TextBody.Should().Contain("Centre manager");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Centre manager</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Centre manager");
            }

            if (isSupervisor)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Supervisor</li>");
                returnedEmail.Body.TextBody.Should().Contain("Supervisor");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Supervisor</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Supervisor");
            }

            if (isNominatedSupervisor)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Nominated supervisor</li>");
                returnedEmail.Body.TextBody.Should().Contain("Nominated supervisor");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Nominated supervisor</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Nominated supervisor");
            }

            if (isTrainer)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Trainer</li>");
                returnedEmail.Body.TextBody.Should().Contain("Trainer");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Trainer</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Trainer");
            }


            if (isContentCreator)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Content Creator licence</li>");
                returnedEmail.Body.TextBody.Should().Contain("Content Creator licence");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Content Creator licence</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Content Creator licence");
            }

            if (isCmsAdministrator)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>CMS administrator</li>");
                returnedEmail.Body.TextBody.Should().Contain("CMS administrator");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>CMS administrator</li>");
                returnedEmail.Body.TextBody.Should().NotContain("CMS administrator");
            }

            if (isCmsManager)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>CMS manager</li>");
                returnedEmail.Body.TextBody.Should().Contain("CMS manager");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>CMS manager</li>");
                returnedEmail.Body.TextBody.Should().NotContain("CMS manager");
            }

            returnedEmail.Body.HtmlBody.Should().Contain("the next time you log in to " + centreName + ".</body>");
            returnedEmail.Body.TextBody.Should().Contain("the next time you log in to " + centreName + ".");
        }
    }
}
