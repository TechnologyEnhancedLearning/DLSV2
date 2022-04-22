namespace DigitalLearningSolutions.Data.Models
{
    using System.Text.RegularExpressions;

    public class ProgressCompletionData
    {
        public int CentreId { get; set; }
        public string CourseName { get; set; }
        public string? AdminEmail { get; set; }
        public int SessionId { get; set; }

        public string GetCourseNameWithoutAverageTimeInfo()
        {
            return Regex.Replace(CourseName, @"\([^()]*\)", string.Empty).Trim();
        }
    }
}
