namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class CustomPromptWithAnswer : CustomPrompt
    {
        public CustomPromptWithAnswer(int customPromptNumber, string text, string? options, bool mandatory, string? answer)
            : base(customPromptNumber, text, options, mandatory)
        {
            Answer = answer;
        }

        public string? Answer { get; set; }
    }
}
