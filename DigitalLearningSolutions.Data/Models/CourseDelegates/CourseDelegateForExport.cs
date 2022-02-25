namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    public class CourseDelegateForExport : CourseDelegate
    {
        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }

        public string? CourseAnswer1 { get; set; }

        public string? CourseAnswer2 { get; set; }

        public string? CourseAnswer3 { get; set; }

        public int LoginCount { get; set; }

        public int Duration { get; set; }

        public int? DiagnosticScore { get; set; }

        public string?[] CentreRegistrationPromptAnswers =>
            new[] { Answer1, Answer2, Answer3, Answer4, Answer5, Answer6 };

        public string?[] CourseAdminFieldAnswers =>
            new[] { CourseAnswer1, CourseAnswer2, CourseAnswer3 };
    }
}
