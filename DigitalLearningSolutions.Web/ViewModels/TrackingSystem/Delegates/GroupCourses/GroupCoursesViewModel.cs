namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;

    public class GroupCoursesViewModel : BasePaginatedViewModel
    {
        public GroupCoursesViewModel(
            int groupId,
            string groupName,
            IEnumerable<GroupCourse> groupCourses,
            int page
        ) : base(page)
        {
            GroupId = groupId;
            NavViewModel = new DelegateGroupsSideNavViewModel(groupId, groupName, DelegateGroupPage.Courses);

            var sortedItems = groupCourses.OrderBy(gc => gc.CourseName).ToList();

            MatchingSearchResults = sortedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(sortedItems);
            GroupCourses = paginatedItems.Select(groupCourse => new GroupCourseViewModel(groupCourse));
        }

        public int GroupId { get; set; }

        public DelegateGroupsSideNavViewModel NavViewModel { get; set; }

        public IEnumerable<GroupCourseViewModel> GroupCourses { get; }
    }
}
