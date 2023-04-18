namespace DigitalLearningSolutions.Web.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Services;
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
        private ISessionDataService sessionDataService = null!;

        [SetUp]
        public void SetUp()
        {
            userDataService = A.Fake<IUserDataService>();
            centreRegistrationPromptsService = A.Fake<ICentreRegistrationPromptsService>();
            emailService = A.Fake<IEmailService>();
            centresDataService = A.Fake<ICentresDataService>();
            sessionDataService = A.Fake<ISessionDataService>();
            logger = A.Fake<ILogger<DelegateApprovalsService>>();
            config = A.Fake<IConfiguration>();
            delegateApprovalsService = new DelegateApprovalsService(
                userDataService,
                centreRegistrationPromptsService,
                emailService,
                centresDataService,
                sessionDataService,
                logger,
                config
            );
        }

        [Test]
        public void
            GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre_returns_unapproved_delegates_with_registration_prompt_answers_for_centre()
        {
            // Given
            var expectedDelegate = UserTestHelper.GetDefaultDelegateEntity();
            var expectedDelegateList = new List<DelegateEntity> { expectedDelegate };
            var expectedRegistrationPrompts = new List<CentreRegistrationPromptWithAnswer>
            {
                PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(
                    1,
                    options: null,
                    mandatory: true,
                    answer: "answer"
                ),
            };

            A.CallTo(() => userDataService.GetUnapprovedDelegatesByCentreId(2))
                .Returns(expectedDelegateList);
            A.CallTo(
                    () => centreRegistrationPromptsService
                        .GetCentreRegistrationPromptsWithAnswersByCentreIdForDelegates(
                            2,
                            expectedDelegateList
                        )
                )
                .Returns(
                    new List<(DelegateEntity delegateEntity, List<CentreRegistrationPromptWithAnswer> prompts)>
                        { (expectedDelegate, expectedRegistrationPrompts) }
                );

            // When
            var result = delegateApprovalsService.GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre(2);

            // Then
            result.Should().BeEquivalentTo(
                new List<(DelegateEntity, List<CentreRegistrationPromptWithAnswer>)>
                    { (expectedDelegate, expectedRegistrationPrompts) }
            );
        }

        [Test]
        public void ApproveDelegate_approves_delegate()
        {
            // Given
            const int delegateId = 1;
            const int centreId = 2;
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                delegateId,
                centreId: centreId,
                approved: false
            );

            A.CallTo(() => userDataService.GetDelegateById(delegateId)).Returns(delegateEntity);
            A.CallTo(() => userDataService.ApproveDelegateUsers(delegateId)).DoesNothing();
            A.CallTo(() => emailService.SendEmails(A<IEnumerable<Email>>._)).DoesNothing();

            // When
            delegateApprovalsService.ApproveDelegate(delegateId, centreId);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(delegateId)).MustHaveHappened();
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
            const int delegateId = 1;
            const int centreId = 2;
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                delegateId,
                centreId: centreId,
                approved: true
            );

            A.CallTo(() => userDataService.GetDelegateById(delegateId)).Returns(delegateEntity);

            // When
            delegateApprovalsService.ApproveDelegate(delegateId, centreId);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(delegateId)).MustNotHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void ApproveAllUnapprovedDelegatesForCentre_approves_all_unapproved_delegates_for_centre()
        {
            // Given
            const int delegateId1 = 1;
            const int delegateId2 = 3;
            const int centreId = 2;
            var delegateEntity1 = UserTestHelper.GetDefaultDelegateEntity(
                delegateId1,
                centreId: centreId,
                approved: false
            );
            var delegateEntity2 = UserTestHelper.GetDefaultDelegateEntity(
                delegateId2,
                centreId: centreId,
                approved: false
            );
            var delegateEntities = new List<DelegateEntity> { delegateEntity1, delegateEntity2 };

            A.CallTo(() => userDataService.GetUnapprovedDelegatesByCentreId(centreId))
                .Returns(delegateEntities);
            A.CallTo(() => userDataService.ApproveDelegateUsers(delegateId1, delegateId2))
                .DoesNothing();
            A.CallTo(() => emailService.SendEmails(A<List<Email>>.That.Matches(s => s.Count == 2))).DoesNothing();

            // When
            delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(centreId);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(delegateId1, delegateId2))
                .MustHaveHappened();
            A.CallTo(() => emailService.SendEmails(A<List<Email>>.That.Matches(s => s.Count == 2))).MustHaveHappened();
        }

        [Test]
        public void RejectDelegate_with_delegate_without_sessions_deletes_delegate_and_sends_email()
        {
            // Given
            const int delegateId = 1;
            const int centreId = 2;
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                delegateId,
                centreId: centreId,
                approved: false
            );

            A.CallTo(() => userDataService.GetDelegateById(delegateId)).Returns(delegateEntity);
            A.CallTo(() => sessionDataService.HasDelegateGotSessions(delegateId)).Returns(false);
            A.CallTo(() => userDataService.RemoveDelegateAccount(delegateId)).DoesNothing();
            A.CallTo(() => userDataService.DeactivateDelegateUser(delegateId)).DoesNothing();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).DoesNothing();

            // When
            delegateApprovalsService.RejectDelegate(delegateId, centreId);

            // Then
            A.CallTo(() => userDataService.RemoveDelegateAccount(delegateId)).MustHaveHappened();
            A.CallTo(() => userDataService.DeactivateDelegateUser(delegateId)).MustNotHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappened();
        }

        [Test]
        public void RejectDelegate_with_delegate_with_sessions_deactivates_delegate_and_sends_email()
        {
            // Given
            const int delegateId = 1;
            const int centreId = 2;
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity(
                delegateId,
                centreId: centreId,
                approved: false
            );

            A.CallTo(() => userDataService.GetDelegateById(delegateId)).Returns(delegateEntity);
            A.CallTo(() => sessionDataService.HasDelegateGotSessions(delegateId)).Returns(true);
            A.CallTo(() => userDataService.RemoveDelegateAccount(delegateId)).DoesNothing();
            A.CallTo(() => userDataService.DeactivateDelegateUser(delegateId)).DoesNothing();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).DoesNothing();

            // When
            delegateApprovalsService.RejectDelegate(delegateId, centreId);

            // Then
            A.CallTo(() => userDataService.RemoveDelegateAccount(delegateId)).MustNotHaveHappened();
            A.CallTo(() => userDataService.DeactivateDelegateUser(delegateId)).MustHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappened();
        }

        [Test]
        public void RejectDelegate_does_not_reject_approved_delegate()
        {
            // Given
            const int delegateId = 1;
            const int centreId = 2;
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity();

            A.CallTo(() => userDataService.GetDelegateById(delegateId)).Returns(delegateEntity);
            A.CallTo(() => userDataService.RemoveDelegateAccount(delegateId)).DoesNothing();
            A.CallTo(() => userDataService.DeactivateDelegateUser(delegateId)).DoesNothing();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).DoesNothing();

            // When
            Action action = () => delegateApprovalsService.RejectDelegate(delegateId, centreId);

            // Then
            action.Should().Throw<UserAccountInvalidStateException>();
            A.CallTo(() => userDataService.RemoveDelegateAccount(delegateId)).MustNotHaveHappened();
            A.CallTo(() => userDataService.DeactivateDelegateUser(delegateId)).MustNotHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }
    }
}
