namespace DigitalLearningSolutions.Data.Models.CourseDelegates
{
    public class CourseDelegateForExport : CourseDelegate
    {
        public string? RegistrationAnswer1 { get; set; }
        public string? RegistrationAnswer2 { get; set; }
        public string? RegistrationAnswer3 { get; set; }
        public string? RegistrationAnswer4 { get; set; }
        public string? RegistrationAnswer5 { get; set; }
        public string? RegistrationAnswer6 { get; set; }
        public int Duration { get; set; }

        public string?[] DelegateRegistrationPrompts =>
            new[]
            {
                RegistrationAnswer1,
                RegistrationAnswer2,
                RegistrationAnswer3,
                RegistrationAnswer4,
                RegistrationAnswer5,
                RegistrationAnswer6,
            };

        public string?[] DelegateCourseAdminFields =>
            new[] { Answer1, Answer2, Answer3 };
    }
}
