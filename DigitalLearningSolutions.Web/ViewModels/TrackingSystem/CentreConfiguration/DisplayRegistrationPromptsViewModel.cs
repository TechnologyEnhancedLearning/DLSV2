namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CentreConfiguration
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class DisplayRegistrationPromptsViewModel : DisplayPromptsViewModel
    {
        public DisplayRegistrationPromptsViewModel(CentreCustomPrompts customPrompts) : base(customPrompts.CustomPrompts)
        {
        }
    }
}
