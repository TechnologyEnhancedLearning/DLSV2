namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.AllDelegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using FluentAssertions;
    using NUnit.Framework;

    public class SearchableDelegateViewModelTests
    {
        [Test]
        public void SearchableDelegateViewModel_sets_regstatus_tag_name_correctly()
        {
            // Given
            var selfRegUser = new DelegateUserCard { SelfReg = true, ExternalReg = false };
            var selfRegExternalUser = new DelegateUserCard { SelfReg = true, ExternalReg = true };
            var centreRegUser = new DelegateUserCard { SelfReg = false };

            // When
            var selfRegModel = new SearchableDelegateViewModel(
                selfRegUser,
                new List<CustomFieldViewModel>(),
                new List<CustomPrompt>()
            );
            var selfRegExternalModel =
                new SearchableDelegateViewModel(
                    selfRegExternalUser,
                    new List<CustomFieldViewModel>(),
                    new List<CustomPrompt>()
                );
            var centreRegModel =
                new SearchableDelegateViewModel(
                    centreRegUser,
                    new List<CustomFieldViewModel>(),
                    new List<CustomPrompt>()
                );

            // Then
            selfRegModel.RegStatusTagName.Should().Be("Self registered");
            selfRegExternalModel.RegStatusTagName.Should().Be("Self registered (External)");
            centreRegModel.RegStatusTagName.Should().Be("Registered by centre");
        }
    }
}
