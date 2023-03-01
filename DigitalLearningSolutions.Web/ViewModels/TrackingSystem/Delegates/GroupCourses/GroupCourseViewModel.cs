namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;

    public class GroupCourseViewModel
    {
        public GroupCourseViewModel(GroupCourse groupCourse, ReturnPageQuery returnPageQuery)
        {
            GroupCustomisationId = groupCourse.GroupCustomisationId;
            Name = groupCourse.CourseName;
            Supervisor = DisplayStringHelper.GetPotentiallyInactiveAdminName(
                groupCourse.SupervisorFirstName,
                groupCourse.SupervisorLastName,
                groupCourse.SupervisorAdminActive
            );
            IsMandatory = groupCourse.IsMandatory ? "Mandatory" : "Not mandatory";
            IsAssessed = groupCourse.IsAssessed ? "Assessed" : "Not assessed";
            AddedToGroup = groupCourse.AddedToGroup.ToString(DateHelper.StandardDateFormat);
            CompleteWithin = DisplayStringHelper.ConvertNumberToMonthsString(groupCourse.CompleteWithinMonths);
            ValidFor = DisplayStringHelper.ConvertNumberToMonthsString(groupCourse.ValidityMonths);
            ReturnPageQuery = returnPageQuery;
        }

        public int GroupCustomisationId { get; set; }
        public string Name { get; set; }
        public string IsMandatory { get; set; }
        public string IsAssessed { get; set; }
        public string AddedToGroup { get; set; }
        public string? Supervisor { get; set; }
        public string? CompleteWithin { get; set; }
        public string? ValidFor { get; set; }
        public ReturnPageQuery ReturnPageQuery { get; set; }
    }
}
