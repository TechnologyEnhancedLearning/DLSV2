namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class DelegateCourseInfoViewModel
    {
        public DelegateCourseInfoViewModel(DelegateCourseInfo info)
        {
            CustomisationId = info.CustomisationId;
            ApplicationName = info.ApplicationName;
            CustomisationName = info.CustomisationName;
            Supervisor = info.Supervisor;
            Enrolled = info.Enrolled.ToString("dd/MM/yyyy");
            LastUpdated = info.LastUpdated.ToString("dd/MM/yyyy");
            if (info.CompleteBy.HasValue)
            {
                CompleteBy = info.CompleteBy.Value.ToString("dd/MM/yyyy");
            }
            if (info.Completed.HasValue)
            {
                Completed = info.Completed.Value.ToString("dd/MM/yyyy");
            }
            if (info.Evaluated.HasValue)
            {
                Evaluated = info.Evaluated.Value.ToString("dd/MM/yyyy");
            }
            EnrollmentMethod = info.EnrollmentMethodId switch
            {
                1 => "Self enrolled",
                2 => "Administrator",
                3 => "Group",
                4 => "System",
                _ => ""
            };
            LoginCount = info.LoginCount;
            LearningTime = info.LearningTime + " mins";
            DiagnosticScore = info.DiagnosticScore;
            IsAssessed = info.IsAssessed;
        }

        public int CustomisationId { get; set; }
        public string ApplicationName { get; set; }
        public string CustomisationName { get; set; }
        public string? Supervisor { get; set; }
        public string Enrolled { get; set; }
        public string LastUpdated { get; set; }
        public string? CompleteBy { get; set; }
        public string? Completed { get; set; }
        public string? Evaluated { get; set; }
        public string EnrollmentMethod { get; set; }
        public int LoginCount { get; set; }
        public string LearningTime { get; set; }
        public int? DiagnosticScore { get; set; }
        public bool IsAssessed { get; set; }
    }
}
