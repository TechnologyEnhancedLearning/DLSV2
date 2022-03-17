namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class CentreRegistrationPromptWithAnswer : CentreRegistrationPrompt
    {
        public CentreRegistrationPromptWithAnswer(
            int customPromptNumber,
            int promptId,
            string text,
            string? options,
            bool mandatory,
            string? answer
        )
            : base(customPromptNumber, promptId, text, options, mandatory)
        {
            Answer = answer;
        }

        public string? Answer { get; set; }
    }
}
