namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class RemoveGroupCourseViewModel : IValidatableObject
    {
        public RemoveGroupCourseViewModel() { }

        public RemoveGroupCourseViewModel(int groupCourseId, string courseName, string groupName)
        {
            GroupCourseId = groupCourseId;
            CourseName = courseName;
            GroupName = groupName;
        }

        public int GroupCourseId { get; set; }
        public string CourseName { get; set; }
        public string GroupName { get; set; }
        public bool Confirm { get; set; }
        public bool DeleteStartedEnrolments { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Confirm == false)
            {
                yield return new ValidationResult(
                    "Confirm you wish to remove this course",
                    new[] { nameof(Confirm) }
                );
            }
        }
    }
}
