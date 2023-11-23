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
            var delegateEntity = UserTestHelper.GetDefaultDelegateEntity();
            var customPrompts = new List<CentreRegistrationPromptWithAnswer>
            {
                PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(1),
            };

            // When
            var returnedModel = new UnapprovedDelegate(delegateEntity, customPrompts);

            // Then
            using (new AssertionScope())
            {
                returnedModel.Id.Should().Be(delegateEntity.DelegateAccount.Id);
                returnedModel.CandidateNumber.Should().Be(delegateEntity.DelegateAccount.CandidateNumber);
                returnedModel.TitleName.Should().Be(
                    $"{delegateEntity.UserAccount.FirstName} {delegateEntity.UserAccount.LastName}"
                );
                returnedModel.Email.Should().Be(delegateEntity.UserAccount.PrimaryEmail);
                returnedModel.DateRegistered.Should().Be(delegateEntity.DelegateAccount.DateRegistered);
                returnedModel.JobGroup.Should().Be(delegateEntity.UserAccount.JobGroupName);
                returnedModel.DelegateRegistrationPrompts.Should().NotBeNullOrEmpty();

                var promptModel = returnedModel.DelegateRegistrationPrompts.First();
                var promptData = customPrompts.First();
                promptModel.Answer.Should().Be(promptData.Answer);
                promptModel.Prompt.Should().Be(promptData.PromptText);
                promptModel.PromptNumber.Should().Be(promptData.RegistrationField.Id);
                promptModel.Mandatory.Should().Be(promptData.Mandatory);
            }
        }
    }
}
