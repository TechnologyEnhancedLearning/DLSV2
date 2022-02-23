namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    internal class DelegateApprovalsViewModelTests
    {
        [Test]
        public void UnapprovedDelegate_delegateUser_and_customPrompts_populate_expected_values()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var customPrompts = new List<CustomPromptWithAnswer>
            {
                CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1),
            };

            // When
            var returnedModel = new UnapprovedDelegate(delegateUser, customPrompts);

            // Then
            using (new AssertionScope())
            {
                returnedModel.Id.Should().Be(delegateUser.Id);
                returnedModel.CandidateNumber.Should().Be(delegateUser.CandidateNumber);
                returnedModel.TitleName.Should().Be(
                    $"{delegateUser.FirstName} {delegateUser.LastName} ({delegateUser.EmailAddress})"
                );
                returnedModel.DateRegistered.Should().Be(delegateUser.DateRegistered);
                returnedModel.JobGroup.Should().Be(delegateUser.JobGroupName);
                returnedModel.CustomPrompts.Should().NotBeNullOrEmpty();

                var promptModel = returnedModel.CustomPrompts.First();
                var promptData = customPrompts.First();
                promptModel.Answer.Should().Be(promptData.Answer);
                promptModel.CustomPrompt.Should().Be(promptData.CustomPromptText);
                promptModel.CustomFieldId.Should().Be(promptData.CustomPromptNumber);
                promptModel.Mandatory.Should().Be(promptData.Mandatory);
            }
        }
    }
}
