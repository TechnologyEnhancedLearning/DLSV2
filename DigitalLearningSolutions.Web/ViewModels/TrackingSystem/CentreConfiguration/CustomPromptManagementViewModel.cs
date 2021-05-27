namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using System.Collections.Generic;

    public class CustomPromptManagementViewModel
    {
        public CustomPromptManagementViewModel(int fieldId, string promptName, bool mandatory, List<string> options)
        {
            CustomFieldId = fieldId;
            CustomPromptName = promptName;
            Mandatory = mandatory;
            Options = options;
        }

        public int CustomFieldId { get; set; }

        public string CustomPromptName { get; set; }

        public bool Mandatory { get; set; }

        public List<string> Options { get; set; }
    }
}
