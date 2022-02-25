namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;

    public class DisplayPromptsViewModel
    {
        public DisplayPromptsViewModel(IEnumerable<CentreRegistrationPrompt> centreRegistrationPrompts)
        {
            CustomFields = centreRegistrationPrompts.Select(
                    cp =>
                        new CentreRegistrationPromptManagementViewModel(
                            cp.RegistrationField.Id,
                            cp.PromptText,
                            cp.Mandatory,
                            cp.Options
                        )
                )
                .ToList();
        }

        public List<CentreRegistrationPromptManagementViewModel> CustomFields { get; set; }
    }
}
