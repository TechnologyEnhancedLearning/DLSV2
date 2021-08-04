namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class GroupCoursesViewModel : BaseSearchablePageViewModel
    {
        public GroupCoursesViewModel(
            int groupId,
            string groupName,
            IEnumerable<GroupCourse> groupCourses,
            int page
        ) : base(null, page, false)
        {
            GroupId = groupId;
            NavViewModel = new DelegateGroupsSideNavViewModel(groupId, groupName, DelegateGroupPage.Courses);

            var sortedItems = GenericSortingHelper.SortAllItems(
                groupCourses.AsQueryable(),
                DefaultSortByOptions.Name.PropertyName,
                Ascending
            ).ToList();

            MatchingSearchResults = sortedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(sortedItems);
            GroupCourses = paginatedItems.Select(groupCourse => new GroupCourseViewModel(groupCourse));
        }

        public int GroupId { get; set; }

        public DelegateGroupsSideNavViewModel NavViewModel { get; set; }

        public IEnumerable<GroupCourseViewModel> GroupCourses { get; }

        public override IEnumerable<(string, string)> SortOptions { get; } = new[]
        {
            DefaultSortByOptions.Name
        };
    }
}
