namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class DelegateCourseAdminFieldAnswers
    {
        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string?[] AdminFieldAnswers =>
            new[] { Answer1, Answer2, Answer3 };
    }
}
