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
        [TestCase(false, true, false, true, false, true, false, true)]
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
                delegateEmail
            );

            // Then
            returnedEmail.Subject.Should().Be(emailHeader);
            returnedEmail.To.Should().BeEquivalentTo(delegateEmail);
            returnedEmail.Cc.Should().BeEquivalentTo(supervisorEmail);

            if (isCentreAdmin)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Centre Admin</li>");
                returnedEmail.Body.TextBody.Should().Contain("Centre Admin");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Centre Administrator</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Centre Administrator");
            }

            if (isCentreManager)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Centre Manager</li>");
                returnedEmail.Body.TextBody.Should().Contain("Centre Manager");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Centre Manager</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Centre Manager");
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
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Nominated Supervisor</li>");
                returnedEmail.Body.TextBody.Should().Contain("Nominated Supervisor");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Nominated Supervisor</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Nominated Supervisor");
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
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Content Creator</li>");
                returnedEmail.Body.TextBody.Should().Contain("Content Creator");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Content Creator</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Content Creator");
            }

            if (isCmsAdministrator)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Cms Administrator</li>");
                returnedEmail.Body.TextBody.Should().Contain("Cms Administrator");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Cms Administrator</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Cms Administrator");
            }

            if (isCmsManager)
            {
                returnedEmail.Body.HtmlBody.Should().Contain("<li>Cms Manager</li>");
                returnedEmail.Body.TextBody.Should().Contain("Cms Manager");
            }
            else
            {
                returnedEmail.Body.HtmlBody.Should().NotContain("<li>Cms Manager</li>");
                returnedEmail.Body.TextBody.Should().NotContain("Cms Manager");
            }
        }
    }
}
