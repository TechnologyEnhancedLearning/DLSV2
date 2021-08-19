namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FluentAssertions;
    using NUnit.Framework;

    public class GroupDelegatesRemoveViewModelTests
    {
        [Test]
        public void GroupDelegatesRemoveViewModel_populates_expected_values()
        {
            // Given
            const string groupName = "Group name";
            const int groupId = 1;
            var delegateUser = UserTestHelper.GetDefaultDelegateUser();

            // When
            var result = new GroupDelegatesRemoveViewModel(delegateUser, groupName, groupId);

            // Then
            result.GroupName.Should().Be(groupName);
            result.GroupId.Should().Be(groupId);
            result.DelegateName.Should().Be("Firstname Test");
        }
    }
}
