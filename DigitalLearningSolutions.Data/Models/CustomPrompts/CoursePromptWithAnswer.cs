namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class CoursePromptWithAnswer : CoursePrompt
    {
        public CoursePromptWithAnswer(int coursePromptNumber, string text, string? options, bool mandatory, string? answer)
            : base(coursePromptNumber, text, options, mandatory)
        {
            Answer = answer;
        }

        public string? Answer { get; set; }
    }
}
