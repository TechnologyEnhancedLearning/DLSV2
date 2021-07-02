namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Helpers;
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

            // When
            var result = FilterableTagHelper.GetCurrentTagsForAdminUser(adminUser).ToList();

            // Then
            using (new AssertionScope())
            {
                result.Count.Should().Be(7);
                result.Should().Contain(("Locked", nameof(AdminUser.IsLocked) + "|true"));
                result.Should().Contain(("Centre administrator", nameof(AdminUser.IsCentreAdmin) + "|true"));
                result.Should().Contain(("Supervisor", nameof(AdminUser.IsSupervisor) + "|true"));
                result.Should().Contain(("Trainer", nameof(AdminUser.IsTrainer) + "|true"));
                result.Should().Contain(("CMS manager", nameof(AdminUser.IsContentManager) + "|true"));
                result.Should().Contain(("Content Creator license", nameof(AdminUser.IsContentCreator) + "|true"));
                result.Should().Contain(("CMS administrator", nameof(AdminUser.IsCmsAdministrator) + "|true"));
            }
        }
    }
}
