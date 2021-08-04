namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Tests.TestHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupCoursesViewModelTests
    {
        private readonly DelegateGroupsSideNavViewModel expectedNavViewModel =
            new DelegateGroupsSideNavViewModel(1, "Group name", DelegateGroupPage.Courses);

        private readonly GroupCourse[] groupCourses =
        {
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "a", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "b", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "c", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "d", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "e", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "f", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "g", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "h", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "i", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "j", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "k", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "l", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "m", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "n", customisationName: "v1"),
            GroupTestHelper.GetDefaultGroupCourse(applicationName: "o", customisationName: "v1")
        };

        [Test]
        public void GroupCoursesViewModel_should_default_to_returning_the_first_ten_Course()
        {
            var model = new GroupCoursesViewModel(
                1,
                "Group name",
                groupCourses,
                1
            );

            using (new AssertionScope())
            {
                model.GroupId.Should().Be(1);
                model.NavViewModel.Should().BeEquivalentTo(expectedNavViewModel);
                model.GroupCourses.Count().Should().Be(10);
                model.GroupCourses.FirstOrDefault(groupCourse => groupCourse.Name == "k - v1").Should()
                    .BeNull();
            }
        }

        [Test]
        public void GroupCoursesViewModel_should_correctly_return_the_second_page_of_Course()
        {
            var model = new GroupCoursesViewModel(
                1,
                "Group name",
                groupCourses,
                2
            );

            using (new AssertionScope())
            {
                model.GroupId.Should().Be(1);
                model.NavViewModel.Should().BeEquivalentTo(expectedNavViewModel);
                model.GroupCourses.Count().Should().Be(5);
                model.GroupCourses.First().Name.Should().BeEquivalentTo("k - v1");
            }
        }
    }
}
