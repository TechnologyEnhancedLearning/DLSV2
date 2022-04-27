namespace DigitalLearningSolutions.Data.Models
{
    public class ProgressCompletionData
    {
        public int CentreId { get; set; }
        public string CourseName { get; set; }
        public string? AdminEmail { get; set; }
        public string? CourseNotificationEmail { get; set; }
        public int SessionId { get; set; }
    }
}
