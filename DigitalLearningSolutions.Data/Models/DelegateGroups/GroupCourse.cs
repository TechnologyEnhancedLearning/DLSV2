namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    using System;

    public class GroupCourse
    {
        public int GroupCustomisationId { get; set; }
        public int GroupId { get; set; }
        public int CustomisationId { get; set; }
        public int CourseCategoryId { get; set; }
        public string ApplicationName { get; set; }
        public string? CustomisationName { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsAssessed { get; set; }
        public DateTime AddedToGroup { get; set; }
        public int CurrentVersion { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? SupervisorFirstName { get; set; }
        public string? SupervisorLastName { get; set; }
        public int CompleteWithinMonths { get; set; }
        public int ValidityMonths { get; set; }

        public string CourseName => string.IsNullOrWhiteSpace(CustomisationName)
            ? ApplicationName
            : ApplicationName + " - " + CustomisationName;
    }
}
