namespace DigitalLearningSolutions.Data.Services
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICustomPromptsService { }

    public class CustomPromptsService
    {
        public static CustomPrompt? PopulateCustomPrompt(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory
        )
        {
            return prompt != null ? new CustomPrompt(promptNumber, prompt, options, mandatory) : null;
        }

        public static CustomPromptWithAnswer? PopulateCustomPromptWithAnswer(
            int promptNumber,
            string? prompt,
            string? options,
            bool mandatory,
            string? answer
        )
        {
            return prompt != null ? new CustomPromptWithAnswer(promptNumber, prompt, options, mandatory, answer) : null;
        }
    }
}
