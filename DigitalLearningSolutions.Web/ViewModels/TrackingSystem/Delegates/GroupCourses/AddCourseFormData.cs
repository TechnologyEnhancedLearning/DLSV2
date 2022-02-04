namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;

    public class AddCourseFormData
    {
        public AddCourseFormData() { }

        protected AddCourseFormData(string groupName, CourseNameInfo courseNameInfo)
        {
            GroupName = groupName;
            CourseName = courseNameInfo.CourseName;
        }

        protected AddCourseFormData(AddCourseFormData formData)
        {
            CohortLearners = formData.CohortLearners;
            SupervisorId = formData.SupervisorId;
            GroupName = formData.GroupName;
            CourseName = formData.CourseName;
        }

        public bool CohortLearners { get; set; }
        public int? SupervisorId { get; set; }

        [WholeNumberWithinInclusiveRange(0, 36, "Enter a whole number from 0 to 36")]
        public string? MonthsToComplete { get; set; }

        public string GroupName { get; set; }
        public string CourseName { get; set; }
    }
}
