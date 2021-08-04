namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using System;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public class GroupCourseViewModel
    {
        public GroupCourseViewModel(GroupCourse groupCourse)
        {
            GroupCustomisationId = groupCourse.GroupCustomisationId;
            Name = groupCourse.CourseName;
            Supervisor = groupCourse.SupervisorLastName != null
                ? $"{groupCourse.SupervisorFirstName} {groupCourse.SupervisorLastName}"
                : null;
            IsMandatory = groupCourse.IsMandatory ? "Mandatory" : "Not mandatory";
            IsAssessed = groupCourse.IsAssessed ? "Assessed" : "Not assessed";
            AddedToGroup = groupCourse.AddedToGroup;
            CompleteWithin = ConvertNumberToMonthsString(groupCourse.CompleteWithinMonths);
            ValidFor = ConvertNumberToMonthsString(groupCourse.ValidityMonths);
        }

        public int GroupCustomisationId { get; set; }
        public string Name { get; set; }
        public string IsMandatory { get; set; }
        public string IsAssessed { get; set; }
        public DateTime AddedToGroup { get; set; }
        public string? Supervisor { get; set; }
        public string? CompleteWithin { get; set; }
        public string? ValidFor { get; set; }

        private string? ConvertNumberToMonthsString(int numberOfMonths)
        {
            return numberOfMonths == 0 ? null : $"{numberOfMonths} months";
        }
    }
}
