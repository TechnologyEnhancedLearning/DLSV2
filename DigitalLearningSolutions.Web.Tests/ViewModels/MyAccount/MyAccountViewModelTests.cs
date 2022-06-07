namespace DigitalLearningSolutions.Web.Tests.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
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
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount();
            var customPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPromptsWithAnswers(
                new List<CentreRegistrationPromptWithAnswer>
                {
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(1),
                }
            );

            // When
            var returnedModel = new MyAccountViewModel(
                userAccount,
                delegateAccount,
                delegateAccount.CentreName,
                null,
                customPrompts,
                DlsSubApplication.Default
            );

            // Then
            using (new AssertionScope())
            {
                returnedModel.FirstName.Should().BeEquivalentTo(userAccount.FirstName);
                returnedModel.Centre.Should().BeEquivalentTo(delegateAccount.CentreName);
                returnedModel.Surname.Should().BeEquivalentTo(userAccount.LastName);
                returnedModel.ProfilePicture.Should().BeEquivalentTo(userAccount.ProfileImage);
                returnedModel.DelegateNumber.Should().BeEquivalentTo(delegateAccount.CandidateNumber);
                returnedModel.PrimaryEmail.Should().BeEquivalentTo(userAccount.PrimaryEmail);
                returnedModel.JobGroup.Should().BeEquivalentTo(userAccount.JobGroupName);
                returnedModel.DelegateRegistrationPrompts.Should().NotBeNullOrEmpty();
            }
        }

        [Test]
        public void MyAccountViewModel_AdminUser_no_DelegateUser_populates_expected_values()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var centreName = UserTestHelper.GetDefaultAdminUser().CentreName;

            // When
            var returnedModel = new MyAccountViewModel(
                userAccount,
                null,
                centreName,
                null,
                null,
                DlsSubApplication.Default
            );

            // Then
            using (new AssertionScope())
            {
                returnedModel.FirstName.Should().BeEquivalentTo(userAccount.FirstName);
                returnedModel.Centre.Should().BeEquivalentTo(centreName);
                returnedModel.Surname.Should().BeEquivalentTo(userAccount.LastName);
                returnedModel.ProfilePicture.Should().BeEquivalentTo(userAccount.ProfileImage);
                returnedModel.DelegateNumber.Should().BeNull();
                returnedModel.PrimaryEmail.Should().BeEquivalentTo(userAccount.PrimaryEmail);
                returnedModel.JobGroup.Should().BeEquivalentTo(userAccount.JobGroupName);
                returnedModel.DelegateRegistrationPrompts.Should().BeEmpty();
            }
        }

        [Test]
        public void MyAccountViewModel_CustomFields_ShouldBePopulated()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount();
            var delegateAccount = UserTestHelper.GetDefaultDelegateAccount();
            var customPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPromptsWithAnswers(
                new List<CentreRegistrationPromptWithAnswer>
                {
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(1),
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(2),
                }
            );

            // When
            var returnedModel = new MyAccountViewModel(
                userAccount,
                delegateAccount,
                delegateAccount.CentreName,
                null,
                customPrompts,
                DlsSubApplication.Default
            );

            // Then
            using (new AssertionScope())
            {
                returnedModel.DelegateRegistrationPrompts.Should().NotBeNullOrEmpty();
                returnedModel.DelegateRegistrationPrompts[0].PromptNumber.Should().Be(1);
                returnedModel.DelegateRegistrationPrompts[0].Prompt.Should()
                    .BeEquivalentTo(customPrompts.CustomPrompts[0].PromptText);
                returnedModel.DelegateRegistrationPrompts[0].Answer.Should().BeEquivalentTo(delegateAccount.Answer1);
                returnedModel.DelegateRegistrationPrompts[0].Mandatory.Should().BeFalse();

                returnedModel.DelegateRegistrationPrompts[1].PromptNumber.Should().Be(2);
                returnedModel.DelegateRegistrationPrompts[1].Prompt.Should()
                    .BeEquivalentTo(customPrompts.CustomPrompts[1].PromptText);
                returnedModel.DelegateRegistrationPrompts[1].Answer.Should().BeEquivalentTo(delegateAccount.Answer1);
                returnedModel.DelegateRegistrationPrompts[1].Mandatory.Should().BeFalse();
            }
        }

        [Test]
        public void MyAccountViewModel_where_user_has_not_been_asked_for_prn_says_not_yet_provided()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(
                hasBeenPromptedForPrn: false,
                professionalRegistrationNumber: null
            );
            var customPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPromptsWithAnswers(
                new List<CentreRegistrationPromptWithAnswer>()
            );

            // When
            var returnedModel = new MyAccountViewModel(
                userAccount,
                null,
                null,
                null,
                customPrompts,
                DlsSubApplication.Default
            );

            // Then
            using (new AssertionScope())
            {
                returnedModel.ProfessionalRegistrationNumber.Should().Be("Not yet provided");
            }
        }

        [Test]
        public void MyAccountViewModel_with_no_prn_should_show_not_registered()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(
                hasBeenPromptedForPrn: true,
                professionalRegistrationNumber: null
            );
            var customPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPromptsWithAnswers(
                new List<CentreRegistrationPromptWithAnswer>
                {
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(1),
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(2),
                }
            );

            // When
            var returnedModel = new MyAccountViewModel(
                userAccount,
                null,
                null,
                null,
                customPrompts,
                DlsSubApplication.Default
            );

            // Then
            using (new AssertionScope())
            {
                returnedModel.ProfessionalRegistrationNumber.Should().Be("Not professionally registered");
            }
        }

        [Test]
        public void MyAccountViewModel_with_prn_displays_prn()
        {
            // Given
            var userAccount = UserTestHelper.GetDefaultUserAccount(
                hasBeenPromptedForPrn: true,
                professionalRegistrationNumber: "12345678"
            );
            var customPrompts = PromptsTestHelper.GetDefaultCentreRegistrationPromptsWithAnswers(
                new List<CentreRegistrationPromptWithAnswer>
                {
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(1),
                    PromptsTestHelper.GetDefaultCentreRegistrationPromptWithAnswer(2),
                }
            );

            // When
            var returnedModel = new MyAccountViewModel(
                userAccount,
                null,
                null,
                null,
                customPrompts,
                DlsSubApplication.Default
            );

            // Then
            using (new AssertionScope())
            {
                returnedModel.ProfessionalRegistrationNumber.Should().Be(userAccount.ProfessionalRegistrationNumber);
            }
        }
    }
}
