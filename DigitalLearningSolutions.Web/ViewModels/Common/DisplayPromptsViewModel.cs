namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class DisplayPromptsViewModel
    {
        public DisplayPromptsViewModel(IEnumerable<CustomPrompt> customPrompts)
        {
            CustomFields = customPrompts.Select(
                    cp =>
                        new CustomPromptManagementViewModel(
                            cp.CustomPromptNumber,
                            cp.CustomPromptText,
                            cp.Mandatory,
                            cp.Options
                        )
                )
                .ToList();
        }

        public List<CustomPromptManagementViewModel> CustomFields { get; set; }
    }
}
