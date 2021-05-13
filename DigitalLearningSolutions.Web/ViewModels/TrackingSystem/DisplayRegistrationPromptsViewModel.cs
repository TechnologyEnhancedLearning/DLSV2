namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DisplayRegistrationPromptsViewModel
    {
        public DisplayRegistrationPromptsViewModel(CentreCustomPrompts? customPrompts)
        {
            CustomFields = new List<CustomPromptManagementViewModel>();

            if (customPrompts != null)
            {
                CustomFields = customPrompts.CustomPrompts.Select(cp =>
                        new CustomPromptManagementViewModel(cp.CustomPromptNumber, cp.CustomPromptText, cp.Mandatory,cp.Options))
                    .ToList();
            }
        }

        public List<CustomPromptManagementViewModel> CustomFields { get; set; }
    }
}
