namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupDelegateViewModelTests
    {
        [Test]
        public void GroupDelegateViewModel_populates_expected_values_with_both_names()
        {
            // Given
            var groupDelegate = GroupTestHelper.GetDefaultGroupDelegate(firstName: "Test", lastName: "Name");

            // When
            var result = new GroupDelegateViewModel(groupDelegate);

            // Then
            using (new AssertionScope())
            {
                result.GroupDelegateId.Should().Be(62);
                result.Name.Should().Be("Test Name");
                result.EmailAddress.Should().Be("gslectik.m@vao");
                result.CandidateNumber.Should().Be("KT553");
            }
        }

        [Test]
        public void GroupDelegateViewModel_populates_expected_values_with_only_last_name()
        {
            // Given
            var groupDelegate = GroupTestHelper.GetDefaultGroupDelegate(firstName: null, lastName: "Name");

            // When
            var result = new GroupDelegateViewModel(groupDelegate);

            // Then
            using (new AssertionScope())
            {
                result.GroupDelegateId.Should().Be(62);
                result.Name.Should().Be("Name");
                result.EmailAddress.Should().Be("gslectik.m@vao");
                result.CandidateNumber.Should().Be("KT553");
            }
        }
    }
}
