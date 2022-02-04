namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    public class AddCourseConfirmationViewModel
    {
        public AddCourseConfirmationViewModel(
            int groupId,
            string groupName,
            string courseName,
            int completeWithinMonths
        )
        {
            GroupId = groupId;
            GroupName = groupName;
            CourseName = courseName;
            CompleteWithinMonths = completeWithinMonths;
        }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string CourseName { get; set; }

        public int CompleteWithinMonths { get; set; }
    }
}
