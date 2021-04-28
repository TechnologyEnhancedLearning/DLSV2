namespace DigitalLearningSolutions.Data.Models.CustomPrompts
{
    public class CustomPrompt
    {
        public CustomPrompt(string? text, string? options, bool mandatory)
        {
            CustomPromptText = text;
            Options = options;
            Mandatory = mandatory;
        }
        
        public string? CustomPromptText { get; set; }
        public string? Options { get; set; }
        public bool Mandatory { get; set; }
    }
}
