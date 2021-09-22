﻿namespace DigitalLearningSolutions.Web.Tests.ViewModels.MyAccount
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
            var adminUser = UserTestHelper.GetDefaultAdminUser();
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var customPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPromptsWithAnswers(
                new List<CustomPromptWithAnswer>
                {
                    CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1)
                });

            // When
            var returnedModel = new MyAccountViewModel(adminUser, delegateUser, customPrompts, ApplicationType.Default);

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
                returnedModel.CustomFields.Should().NotBeNullOrEmpty();
            }
        }

        [Test]
        public void MyAccountViewModel_AdminUser_no_DelegateUser_populates_expected_values()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser();

            // When
            var returnedModel = new MyAccountViewModel(adminUser, null, null, ApplicationType.Default);

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
                returnedModel.CustomFields.Should().BeEmpty();
            }
        }

        [Test]
        public void MyAccountViewModel_DelegateUser_no_AdminUser_populates_expected_values()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var customPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPromptsWithAnswers(
                new List<CustomPromptWithAnswer>
                {
                    CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1)
                });

            // When
            var returnedModel = new MyAccountViewModel(null, delegateUser, customPrompts, ApplicationType.Default);

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
                returnedModel.CustomFields.Should().NotBeNullOrEmpty();
            }
        }

        [Test]
        public void MyAccountViewModel_CustomFields_ShouldBePopulated()
        {
            // Given
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();
            var customPrompts = CustomPromptsTestHelper.GetDefaultCentreCustomPromptsWithAnswers(
                new List<CustomPromptWithAnswer>
                {
                    CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(1),
                    CustomPromptsTestHelper.GetDefaultCustomPromptWithAnswer(2)
                });

            // When
            var returnedModel = new MyAccountViewModel(null, delegateUser, customPrompts, ApplicationType.Default);

            // Then
            using (new AssertionScope())
            {
                returnedModel.CustomFields.Should().NotBeNullOrEmpty();
                returnedModel.CustomFields[0].CustomFieldId.Should().Be(1);
                returnedModel.CustomFields[0].CustomPrompt.Should()
                    .BeEquivalentTo(customPrompts.CustomPrompts[0].CustomPromptText);
                returnedModel.CustomFields[0].Answer.Should().BeEquivalentTo(delegateUser.Answer1);
                returnedModel.CustomFields[0].Mandatory.Should().BeFalse();

                returnedModel.CustomFields[1].CustomFieldId.Should().Be(2);
                returnedModel.CustomFields[1].CustomPrompt.Should()
                    .BeEquivalentTo(customPrompts.CustomPrompts[1].CustomPromptText);
                returnedModel.CustomFields[1].Answer.Should().BeEquivalentTo(delegateUser.Answer1);
                returnedModel.CustomFields[1].Mandatory.Should().BeFalse();
            }
        }
    }
}
