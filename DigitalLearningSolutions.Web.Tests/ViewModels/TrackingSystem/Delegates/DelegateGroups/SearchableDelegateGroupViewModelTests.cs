namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class SearchableDelegateGroupViewModelTests
    {
        [Test]
        public void SearchableDelegateGroupViewModel_populates_expected_values()
        {
            // Given
            var group = new Group
            {
                GroupId = 1,
                GroupLabel = "Group name",
                GroupDescription = null,
                DelegateCount = 5,
                CoursesCount = 10,
                AddedByFirstName = "Test",
                AddedByLastName = "Person",
                AddedByAdminActive = true,
                LinkedToField = 0,
                LinkedToFieldName = "None",
                ShouldAddNewRegistrantsToGroup = false,
                ChangesToRegistrationDetailsShouldChangeGroupMembership = false,
            };

            // When
            var result = new SearchableDelegateGroupViewModel(group, new ReturnPageQuery(3, "1-card"));

            // Then
            using (new AssertionScope())
            {
                result.Id.Should().Be(1);
                result.Name.Should().Be("Group name");
                result.Description.Should().BeNull();
                result.DelegateCount.Should().Be(5);
                result.CourseCount.Should().Be(10);
                result.AddedBy.Should().Be("Test Person");
                result.LinkedToField.Should().Be(0);
                result.LinkedField.Should().Be("None");
                result.ShouldAddNewRegistrantsToGroup.Should().Be("No");
                result.ChangesToRegistrationDetailsShouldChangeGroupMembership.Should().Be("No");
            }
        }

        [Test]
        public void SearchableDelegateGroupViewModel_populates_expected_values_for_bool_fields()
        {
            // Given
            var group = new Group
            {
                ShouldAddNewRegistrantsToGroup = true,
                ChangesToRegistrationDetailsShouldChangeGroupMembership = true,
            };

            // When
            var result = new SearchableDelegateGroupViewModel(group, new ReturnPageQuery(3, "1-card"));

            // Then
            using (new AssertionScope())
            {
                result.ShouldAddNewRegistrantsToGroup.Should().Be("Yes");
                result.ChangesToRegistrationDetailsShouldChangeGroupMembership.Should().Be("Yes");
            }
        }
    }
}
