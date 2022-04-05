namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Shared;

    public class GroupCoursesViewModel : BasePaginatedViewModel<GroupCourse>
    {
        public GroupCoursesViewModel(
            int groupId,
            string groupName,
            PaginationResult<GroupCourse> result
        ) : base(result)
        {
            GroupId = groupId;
            NavViewModel = new DelegateGroupsSideNavViewModel(groupId, groupName, DelegateGroupPage.Courses);
            GroupCourses = result.ItemsToDisplay.Select(
                groupCourse => new GroupCourseViewModel(
                    groupCourse,
                    result.GetReturnPageQuery($"{groupCourse.GroupCustomisationId}-card")
                )
            );
        }

        public int GroupId { get; set; }

        public DelegateGroupsSideNavViewModel NavViewModel { get; set; }

        public IEnumerable<GroupCourseViewModel> GroupCourses { get; }
    }
}
