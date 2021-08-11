namespace DigitalLearningSolutions.Web.Tests.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Tests.NBuilderHelpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using FizzWare.NBuilder;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;

    public class GroupCoursesViewModelTests
    {
        private readonly DelegateGroupsSideNavViewModel expectedNavViewModel =
            new DelegateGroupsSideNavViewModel(1, "Group name", DelegateGroupPage.Courses);

        private GroupCourse[] groupCourses = null!;

        [SetUp]
        public void SetUp()
        {
            BuilderSetup.DisablePropertyNamingFor<GroupCourse, string>(g => g.SearchableName);
            groupCourses = Builder<GroupCourse>.CreateListOfSize(15)
                .All()
                .With(g => g.CustomisationName = "v1")
                .With(
                    (g, i) => g.ApplicationName = NBuilderAlphabeticalPropertyNamingHelper.IndexToAlphabeticalString(i)
                )
                .Build().ToArray();
        }

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
                model.GroupCourses.Count().Should().Be(BaseSearchablePageViewModel.DefaultItemsPerPage);
                model.GroupCourses.Any(groupCourse => groupCourse.Name == "K - v1").Should()
                    .BeFalse();
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
            var expectedFirstGroupCourse =
                groupCourses.Skip(BaseSearchablePageViewModel.DefaultItemsPerPage).First();

            using (new AssertionScope())
            {
                model.GroupId.Should().Be(1);
                model.NavViewModel.Should().BeEquivalentTo(expectedNavViewModel);
                model.GroupCourses.Count().Should().Be(5);
                model.GroupCourses.First().Name.Should().BeEquivalentTo(expectedFirstGroupCourse.CourseName);
            }
        }
    }
}
