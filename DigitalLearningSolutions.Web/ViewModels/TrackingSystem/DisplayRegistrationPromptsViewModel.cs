namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DisplayRegistrationPromptsViewModel
    {
        public DisplayRegistrationPromptsViewModel(CentreCustomPrompts? customPrompts)
        {
            CustomFields = new List<CustomPromptManagementViewModel>();

            TryAddCustomPromptManagementViewModelToList(1, customPrompts?.CustomField1);
            TryAddCustomPromptManagementViewModelToList(2, customPrompts?.CustomField2);
            TryAddCustomPromptManagementViewModelToList(3, customPrompts?.CustomField3);
            TryAddCustomPromptManagementViewModelToList(4, customPrompts?.CustomField4);
            TryAddCustomPromptManagementViewModelToList(5, customPrompts?.CustomField5);
            TryAddCustomPromptManagementViewModelToList(6, customPrompts?.CustomField6);
        }

        private void TryAddCustomPromptManagementViewModelToList(int fieldNumber, CustomPrompt? customPrompt)
        {
            if (customPrompt != null)
            {
                CustomFields.Add(new CustomPromptManagementViewModel(fieldNumber, customPrompt.CustomPromptText, customPrompt.Mandatory, customPrompt.Options));
            }
        }

        public List<CustomPromptManagementViewModel> CustomFields { get; set; }
    }
}
