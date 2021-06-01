namespace DigitalLearningSolutions.Data.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using Microsoft.Extensions.Logging;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    public class DelegateApprovalsServiceTests
    {
        private IUserDataService userDataService = null!;
        private ICustomPromptsService customPromptsService = null!;
        private IEmailService emailService = null!;
        private ILogger<DelegateApprovalsService> logger = null!;
        private IDelegateApprovalsService delegateApprovalsService = null!;

        [SetUp]
        public void SetUp()
        {
            userDataService = A.Fake<IUserDataService>();
            customPromptsService = A.Fake<ICustomPromptsService>();
            emailService = A.Fake<IEmailService>();
            logger = A.Fake<ILogger<DelegateApprovalsService>>();
            delegateApprovalsService = new DelegateApprovalsService(userDataService, customPromptsService, emailService, logger);
        }

        [Test]
        public void GetUnapprovedDelegatesWithCustomPromptAnswersForCentre_returns_unapproved_delegates_with_custom_prompt_answers_for_centre()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();
            var expectedUserList = new List<DelegateUser> { expectedDelegateUser };
            var expectedCustomPrompts = new List<CustomPromptWithAnswer>
                { CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1, options: null, mandatory: true, answer: "answer") };

            A.CallTo(() => userDataService.GetUnapprovedDelegateUsersByCentreId(2))
                .Returns(expectedUserList);
            A.CallTo(() => customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdForDelegateUsers(2, expectedUserList))
                .Returns(new List<(DelegateUser delegateUser, List<CustomPromptWithAnswer> prompts)>{(expectedDelegateUser, expectedCustomPrompts)});

            // When
            var result = delegateApprovalsService.GetUnapprovedDelegatesWithCustomPromptAnswersForCentre(2);

            // Then
            result.Should().BeEquivalentTo(new List<(DelegateUser, List<CustomPromptWithAnswer>)> { (expectedDelegateUser, expectedCustomPrompts) });
        }

        [Test]
        public void ApproveDelegate_approves_delegate()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser(approved: false);

            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(expectedDelegateUser);
            A.CallTo(() => userDataService.ApproveDelegateUsers(A<IEnumerable<int>>.That.IsSameSequenceAs(new[] { 2 }))).DoesNothing();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).DoesNothing();

            // When
            delegateApprovalsService.ApproveDelegate(2);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(A<IEnumerable<int>>.That.IsSameSequenceAs(new[] { 2 }))).MustHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustHaveHappened();
        }

        [Test]
        public void ApproveDelegate_throws_if_delegate_not_found()
        {
            // Given
            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(null);

            // When
            Action action = () => delegateApprovalsService.ApproveDelegate(2);

            // Then
            action.Should().Throw<UserAccountNotFoundException>().WithMessage("Delegate user id 2 not found");
            A.CallTo(() => userDataService.ApproveDelegateUsers(A<IEnumerable<int>>.That.IsSameSequenceAs(new[] { 2 }))).MustNotHaveHappened();
        }

        [Test]
        public void ApproveDelegate_does_not_approve_already_approved_delegate()
        {
            // Given
            var expectedDelegateUser = UserTestHelper.GetDefaultDelegateUser();

            A.CallTo(() => userDataService.GetDelegateUserById(2)).Returns(expectedDelegateUser);

            // When
            delegateApprovalsService.ApproveDelegate(2);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(A<IEnumerable<int>>.That.IsSameSequenceAs(new[] { 2 }))).MustNotHaveHappened();
            A.CallTo(() => emailService.SendEmail(A<Email>._)).MustNotHaveHappened();
        }

        [Test]
        public void ApproveAllUnapprovedDelegatesForCentre_approves_all_unapproved_delegates_for_centre()
        {
            // Given
            var expectedDelegateUser1 = UserTestHelper.GetDefaultDelegateUser(approved: false);
            var expectedDelegateUser2 = UserTestHelper.GetDefaultDelegateUser(id: 3, approved: false);
            var expectedUserList = new List<DelegateUser> { expectedDelegateUser1, expectedDelegateUser2 };
            var expectedUserIds = expectedUserList.Select(du => du.Id);

            A.CallTo(() => userDataService.GetUnapprovedDelegateUsersByCentreId(2))
                .Returns(expectedUserList);
            A.CallTo(() => userDataService.ApproveDelegateUsers(A<IEnumerable<int>>.That.IsSameSequenceAs(expectedUserIds)))
                .DoesNothing();
            A.CallTo(() => emailService.SendEmails(A<List<Email>>.That.Matches(s => s.Count == 2))).DoesNothing();

            // When
            delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(2);

            // Then
            A.CallTo(() => userDataService.ApproveDelegateUsers(A<IEnumerable<int>>.That.IsSameSequenceAs(expectedUserIds)))
                .MustHaveHappened();
            A.CallTo(() => emailService.SendEmails(A<List<Email>>.That.Matches(s => s.Count == 2))).MustHaveHappened();
        }
    }
}
