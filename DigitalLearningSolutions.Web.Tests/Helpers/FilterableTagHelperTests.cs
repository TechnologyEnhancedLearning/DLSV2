namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
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
                new SearchableTagViewModel(AdminFilterOptions.IsLocked),
                new SearchableTagViewModel(AdminFilterOptions.CentreAdministrator),
                new SearchableTagViewModel(AdminFilterOptions.Supervisor),
                new SearchableTagViewModel(AdminFilterOptions.Trainer),
                new SearchableTagViewModel(AdminFilterOptions.CmsAdministrator),
                new SearchableTagViewModel(AdminFilterOptions.ContentCreatorLicense),
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
