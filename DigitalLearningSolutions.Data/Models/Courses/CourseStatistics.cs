namespace DigitalLearningSolutions.Data.Models.Courses
{
    using System;

    public class CourseStatistics
    {
        public int CustomisationId { get; set; }
        public int CentreId { get; set; }
        public bool Active { get; set; }
        public bool AllCentres { get; set; }
        public bool AspMenu { get; set; }
        public DateTime? ArchivedDate { get; set; }
        public string ApplicationName { get; set; }
        public string? CustomisationName { get; set; }
        public string CourseName => string.IsNullOrWhiteSpace(CustomisationName)
            ? ApplicationName
            : ApplicationName + " - " + CustomisationName;
        public int DelegateCount { get; set; }
        public int CompletedCount { get; set; }
        public int AllAttempts { get; set; }
        public int AttemptsPassed { get; set; }
        public double PassRate => AllAttempts == 0 ? 0 : 100 * AttemptsPassed / (double)AllAttempts;
    }
}
