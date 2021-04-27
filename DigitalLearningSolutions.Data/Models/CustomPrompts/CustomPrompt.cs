namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class CustomPrompt
    {
        public CustomPrompt(int id, string? text, string? options, bool mandatory)
        {
            CustomPromptId = id;
            CustomPromptText = text;
            Options = options;
            Mandatory = mandatory;
        }

        public int CustomPromptId { get; set; }
        public string? CustomPromptText { get; set; }
        public string? Options { get; set; }
        public bool Mandatory { get; set; }
    }
}
