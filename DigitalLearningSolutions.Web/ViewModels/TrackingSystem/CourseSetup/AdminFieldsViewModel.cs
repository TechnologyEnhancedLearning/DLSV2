namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class AdminFieldsViewModel : DisplayPromptsViewModel
    {
        public AdminFieldsViewModel(IEnumerable<CustomPrompt> customPrompts, int customisationId) : base(customPrompts)
        {
            CustomisationId = customisationId;
        }

        public int CustomisationId { get; set; }
    }
}
