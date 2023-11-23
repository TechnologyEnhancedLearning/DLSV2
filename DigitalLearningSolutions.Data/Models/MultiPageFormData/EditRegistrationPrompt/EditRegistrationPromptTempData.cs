namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.EditRegistrationPrompt
{
    public class EditRegistrationPromptTempData
    {
        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public bool Mandatory { get; set; }

        public string? OptionsString { get; set; }

        public string? Answer { get; set; }

        public bool IncludeAnswersTableCaption { get; set; }
    }
}
