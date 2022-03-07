namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class CourseAdminFieldWithAnswer : CourseAdminField
    {
        public CourseAdminFieldWithAnswer(int coursePromptNumber, string text, string? options, string? answer)
            : base(coursePromptNumber, text, options)
        {
            Answer = answer;
        }

        public string? Answer { get; set; }
    }
}
