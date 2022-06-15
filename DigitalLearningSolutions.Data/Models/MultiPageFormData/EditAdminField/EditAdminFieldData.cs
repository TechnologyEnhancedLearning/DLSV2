namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.EditAdminField
{
    public class EditAdminFieldData
    {
        public int PromptNumber { get; set; }

        public string Prompt { get; set; }

        public string? OptionsString { get; set; }

        public string? Answer { get; set; }

        public bool IncludeAnswersTableCaption { get; set; }
    }
}
