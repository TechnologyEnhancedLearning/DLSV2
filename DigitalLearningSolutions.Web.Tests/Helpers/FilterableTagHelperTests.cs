namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class FilterableTagHelperTests
    {
        [Test]
        public void GetCurrentTagsForAdmin_should_return_correct_tags()
        {
            // Given
            var admin = UserTestHelper.GetDefaultAdminEntity(failedLoginCount: 5, isContentCreator: true);
            var expectedTags = new List<SearchableTagViewModel>
            {
                new SearchableTagViewModel(AdminAccountStatusFilterOptions.IsLocked),
                new SearchableTagViewModel(AdminRoleFilterOptions.CentreAdministrator),
                new SearchableTagViewModel(AdminRoleFilterOptions.CentreManager),
                new SearchableTagViewModel(AdminRoleFilterOptions.Supervisor),
                new SearchableTagViewModel(AdminRoleFilterOptions.Trainer),
                new SearchableTagViewModel(AdminRoleFilterOptions.CmsAdministrator),
                new SearchableTagViewModel(AdminRoleFilterOptions.ContentCreatorLicense)
            };

            // When
            var result = FilterableTagHelper.GetCurrentTagsForAdmin(admin).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedTags);
            }
        }

        [Test]
        public void GetCurrentTagsForDelegateUser_should_return_correct_tags()
        {
            // Given
            var delegateUser = new DelegateUserCard
            { Active = true, AdminId = 1, Password = "some password", SelfReg = true };
            var expectedTags = new List<SearchableTagViewModel>
            {
                new SearchableTagViewModel(DelegateActiveStatusFilterOptions.IsActive),
                new SearchableTagViewModel(DelegateAdminStatusFilterOptions.IsAdmin),
                new SearchableTagViewModel(DelegatePasswordStatusFilterOptions.PasswordSet),
                new SearchableTagViewModel(DelegateRegistrationTypeFilterOptions.SelfRegistered)
            };

            // When
            var result = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedTags);
            }
        }

        [Test]
        public void GetCurrentTagsForDelegateUser_negative_tags_should_return_correct_tags()
        {
            // Given
            var delegateUser = new DelegateUserCard();
            var expectedTags = new List<SearchableTagViewModel>
            {
                new SearchableTagViewModel(DelegateActiveStatusFilterOptions.IsNotActive),
                new SearchableTagViewModel(DelegateAdminStatusFilterOptions.IsNotAdmin, true),
                new SearchableTagViewModel(DelegatePasswordStatusFilterOptions.PasswordNotSet),
                new SearchableTagViewModel(DelegateRegistrationTypeFilterOptions.RegisteredByCentre)
            };

            // When
            var result = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedTags);
            }
        }

        [Test]
        public void GetCurrentTagsForDelegateUser_self_registered_should_return_correct_tags()
        {
            // Given
            var delegateUser = new DelegateUserCard
            { SelfReg = true, ExternalReg = true };
            var expectedTags = new List<SearchableTagViewModel>
            {
                new SearchableTagViewModel(DelegateActiveStatusFilterOptions.IsNotActive),
                new SearchableTagViewModel(DelegateAdminStatusFilterOptions.IsNotAdmin, true),
                new SearchableTagViewModel(DelegatePasswordStatusFilterOptions.PasswordNotSet),
                new SearchableTagViewModel(DelegateRegistrationTypeFilterOptions.SelfRegisteredExternal)
            };

            // When
            var result = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedTags);
            }
        }
    }
}
