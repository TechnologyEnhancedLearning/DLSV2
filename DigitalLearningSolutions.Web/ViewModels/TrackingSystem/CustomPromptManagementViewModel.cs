namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using System.Collections.Generic;

    public class CustomPromptManagementViewModel
    {
        public CustomPromptManagementViewModel(int fieldId, string? prompt, bool mandatory, List<string> options)
        {
            CustomFieldId = fieldId;
            CustomPrompt = prompt;
            Mandatory = mandatory;
            Options = options;
        }

        public int CustomFieldId { get; set; }

        public string? CustomPrompt { get; set; }

        public bool Mandatory { get; set; }

        public List<string> Options { get; set; }
    }
}
