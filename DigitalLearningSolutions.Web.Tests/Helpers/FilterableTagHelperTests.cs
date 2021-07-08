namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
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
        public void GetCurrentTagsForAdminUser_should_return_correct_tags()
        {
            // Given
            var adminUser = UserTestHelper.GetDefaultAdminUser(failedLoginCount: 5, isContentCreator: true);
            var expectedTags = new List<SearchableTagViewModel>
            {
                new SearchableTagViewModel(AdminAccountStatusFilterOptions.IsLocked),
                new SearchableTagViewModel(AdminRoleFilterOptions.CentreAdministrator),
                new SearchableTagViewModel(AdminRoleFilterOptions.Supervisor),
                new SearchableTagViewModel(AdminRoleFilterOptions.Trainer),
                new SearchableTagViewModel(AdminRoleFilterOptions.CmsAdministrator),
                new SearchableTagViewModel(AdminRoleFilterOptions.ContentCreatorLicense)
            };

            // When
            var result = FilterableTagHelper.GetCurrentTagsForAdminUser(adminUser).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(expectedTags);
            }
        }
    }
}
