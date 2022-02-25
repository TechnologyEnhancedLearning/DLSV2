namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using FakeItEasy;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;

    public class DelegateApprovalsServiceTests
    {
        private ICentreRegistrationPromptsService centreRegistrationPromptsService = null!;
        private ICentresDataService centresDataService = null!;
        private IConfiguration config = null!;
        private IDelegateApprovalsService delegateApprovalsService = null!;
        private IEmailService emailService = null!;
        private ILogger<DelegateApprovalsService> logger = null!;
        private IUserDataService userDataService = null!;

        [SetUp]
        public void SetUp()
        {
            userDataService = A.Fake<IUserDataService>();
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            emailService = A.Fake<IEmailService>();
            centresDataService = A.Fake<ICentresDataService>();
            logger = A.Fake<ILogger<DelegateApprovalsService>>();
            config = A.Fake<IConfiguration>();
            delegateApprovalsService = new DelegateApprovalsService(
                userDataService,
                centreRegistrationPromptsService,
                emailService,
                centresDataService,
                logger,
                config
            );
        }

        [Test]
        public void
            GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre_returns_unapproved_delegates_with_custom_prompt_answers_for_centre()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var expectedUserList = new List<DelegateUser> { expectedDelegateUser };
            var expectedCustomPrompts = new List<CentreRegistrationPromptWithAnswer>
            {
                PromptsTestHelper.GetDefaultCustomPromptWithAnswer(
                    1,
                    options: null,
                    mandatory: true,
                    answer: "answer"
                )
            };

            A.CallTo(() => userDataService.GetUnapprovedDelegateUsersByCentreId(2))
                .Returns(expectedUserList);
            A.CallTo(
                    () => centreRegistrationPromptsService.GetCentreRegistrationPromptsWithAnswersByCentreIdForDelegateUsers(
                        2,
                        expectedUserList
                    )
                )
                .Returns(
                    new List<(DelegateUser delegateUser, List<CentreRegistrationPromptWithAnswer> prompts)>
                        { (expectedDelegateUser, expectedCustomPrompts) }
                );

            // When
            var result = delegateApprovalsService.GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre(2);

            // Then
            result.Should().BeEquivalentTo(
                new List<(DelegateUser, List<CentreRegistrationPromptWithAnswer>)> { (expectedDelegateUser, expectedCustomPrompts) }
            );
        }

        [Test]
        public void ApproveDelegate_approves_delegate()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser(approved: false);

            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(expectedDelegateUser);
            A.CallTo(() => userDataService.ApproveDelegateUsers(2)).DoesNothing();
            A.CallTo(() => emailService.SendEmails(A<IEnumerable<Email>>._)).DoesNothing();

            // When
            delegateApprovalsService.ApproveDelegate(2, 2);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(2)).MustHaveHappened();
            A.CallTo(() => emailService.SendEmails(A<IEnumerable<Email>>._)).MustHaveHappened();
        }

        [Test]
        public void ApproveDelegate_throws_if_delegate_not_found()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(null);

            // When
            Action action = () => delegateApprovalsService.ApproveDelegate(2, 2);

            // Then
            action.Should().Throw<UserAccountNotFoundException>()
                .WithMessage("Delegate user id 2 not found at centre id 2.");
            A.CallTo(() => userDataService.ApproveDelegateUsers(2)).MustNotHaveHappened();
        }

        [Test]
        public void ApproveDelegate_does_not_approve_already_approved_delegate()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(expectedDelegateUser);

            // When
            delegateApprovalsService.ApproveDelegate(2, 2);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(2)).MustNotHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void ApproveAllUnapprovedDelegatesForCentre_approves_all_unapproved_delegates_for_centre()
        {
            // Given
            var expectedDelegateUser1 = UserTestHelper.GetDefaultDelegateUser(approved: false);
            var expectedDelegateUser2 = UserTestHelper.GetDefaultDelegateUser(3, approved: false);
            var expectedUserList = new List<DelegateUser> { expectedDelegateUser1, expectedDelegateUser2 };

            A.CallTo(() => userDataService.GetUnapprovedDelegateUsersByCentreId(2))
                .Returns(expectedUserList);
            A.CallTo(() => userDataService.ApproveDelegateUsers(2, 3))
                .DoesNothing();
            A.CallTo(() => emailService.SendEmails(A<List<Email>>.That.Matches(s => s.Count == 2))).DoesNothing();

            // When
            delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(2);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(2, 3))
                .MustHaveHappened();
            A.CallTo(() => emailService.SendEmails(A<List<Email>>.That.Matches(s => s.Count == 2))).MustHaveHappened();
        }

        [Test]
        public void RejectDelegate_deletes_delegate_and_sends_email()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser(approved: false);

            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(expectedDelegateUser);
            A.CallTo(() => userDataService.RemoveDelegateUser(2)).DoesNothing();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).DoesNothing();

            // When
            delegateApprovalsService.RejectDelegate(2, 2);

            // Then
            A.CallTo(() => userDataService.RemoveDelegateUser(2)).MustHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappened();
        }

        [Test]
        public void RejectDelegate_does_not_reject_approved_delegate()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(expectedDelegateUser);
            A.CallTo(() => userDataService.RemoveDelegateUser(2)).DoesNothing();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).DoesNothing();

            // When
            Action action = () => delegateApprovalsService.RejectDelegate(2, 2);

            // Then
            action.Should().Throw<UserAccountInvalidStateException>();
            A.CallTo(() => userDataService.RemoveDelegateUser(2)).MustNotHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }
    }
}
