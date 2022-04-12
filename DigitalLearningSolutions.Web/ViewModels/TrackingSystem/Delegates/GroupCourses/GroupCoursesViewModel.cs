namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class GroupCoursesViewModel : BasePaginatedViewModel<GroupCourse>
    {
        public GroupCoursesViewModel(
            int groupId,
            string groupName,
            PaginationResult<GroupCourse> result
        ) : base(result)
        {
            GroupId = groupId;
            GroupName = groupName;
            var routeData = new Dictionary<string, string> { { "groupId", groupId.ToString() } };
            TabsNavLinks = new TabsNavViewModel(DelegateGroupTab.Courses, routeData);
            GroupCourses = result.ItemsToDisplay.Select(
                groupCourse => new GroupCourseViewModel(
                    groupCourse,
                    result.GetReturnPageQuery($"{groupCourse.GroupCustomisationId}-card")
                )
            );
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public TabsNavViewModel TabsNavLinks { get; set; }

        public IEnumerable<GroupCourseViewModel> GroupCourses { get; }
    }
}
