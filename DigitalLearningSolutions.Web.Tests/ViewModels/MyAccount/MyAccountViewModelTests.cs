namespace DigitalLearningSolutions.Web.Tests.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Data.Tests.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class MyAccountViewModelTests
    {
        [Test]
        public void MyAccountViewModel_AdminUser_and_DelegateUser_populates_expected_values()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var customPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPrompt1: CustomPromptsTestHelper.GetDefaultCustomPrompt());
            
            // When
            var returnedModel = new MyAccountViewModel(adminUser, delegateUser, customPrompts);

            // Then
            using (new AssertionScope())
            {
                returnedModel.FirstName.Should().BeEquivalentTo(adminUser.FirstName);
                returnedModel.Centre.Should().BeEquivalentTo(adminUser.CentreName);
                returnedModel.Surname.Should().BeEquivalentTo(adminUser.LastName);
                returnedModel.ProfilePicture.Should().BeEquivalentTo(adminUser.ProfileImage);
                returnedModel.DelegateNumber.Should().BeEquivalentTo(delegateUser.CandidateNumber);
                returnedModel.User.Should().BeEquivalentTo(adminUser.EmailAddress);
                returnedModel.JobGroup.Should().BeEquivalentTo(delegateUser.JobGroupName);
                returnedModel.CustomField1?.CustomPrompt.Should().BeEquivalentTo(customPrompts.CustomField1?.CustomPromptText);
                returnedModel.CustomField1?.Answer.Should().BeEquivalentTo(delegateUser.Answer1);
                returnedModel.CustomField1?.Mandatory.Should().BeFalse();
                returnedModel.CustomField2.Should().BeNull();
            }
        }

        [Test]
        public void MyAccountViewModel_AdminUser_no_DelegateUser_populates_expected_values()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedModel = new MyAccountViewModel(adminUser, null, null);

            // Then
            using (new AssertionScope())
            {
                returnedModel.FirstName.Should().BeEquivalentTo(adminUser.FirstName);
                returnedModel.Centre.Should().BeEquivalentTo(adminUser.CentreName);
                returnedModel.Surname.Should().BeEquivalentTo(adminUser.LastName);
                returnedModel.ProfilePicture.Should().BeEquivalentTo(adminUser.ProfileImage);
                returnedModel.DelegateNumber.Should().BeNull();
                returnedModel.User.Should().BeEquivalentTo(adminUser.EmailAddress);
                returnedModel.JobGroup.Should().BeNull();
                returnedModel.CustomField1.Should().BeNull();
            }
        }

        [Test]
        public void MyAccountViewModel_DelegateUser_no_AdminUser_populates_expected_values()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var customPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPrompts(customPrompt1: CustomPromptsTestHelper.GetDefaultCustomPrompt());

            // When
            var returnedModel = new MyAccountViewModel(null, delegateUser, customPrompts);

            // Then
            using (new AssertionScope())
            {
                returnedModel.FirstName.Should().BeEquivalentTo(delegateUser.FirstName);
                returnedModel.Centre.Should().BeEquivalentTo(delegateUser.CentreName);
                returnedModel.Surname.Should().BeEquivalentTo(delegateUser.LastName);
                returnedModel.ProfilePicture.Should().BeEquivalentTo(delegateUser.ProfileImage);
                returnedModel.DelegateNumber.Should().BeEquivalentTo(delegateUser.CandidateNumber);
                returnedModel.User.Should().BeEquivalentTo(delegateUser.EmailAddress);
                returnedModel.JobGroup.Should().BeEquivalentTo(delegateUser.JobGroupName);
                returnedModel.CustomField1?.CustomPrompt.Should().BeEquivalentTo(customPrompts.CustomField1?.CustomPromptText);
                returnedModel.CustomField1?.Answer.Should().BeEquivalentTo(delegateUser.Answer1);
                returnedModel.CustomField1?.Mandatory.Should().BeFalse();
                returnedModel.CustomField2.Should().BeNull();
            }
        }
    }
}
