namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using System.Collections.Generic;

    public class EditAdminFieldViewModel
    {

        public EditAdminFieldViewModel() { }

        public EditAdminFieldViewModel(IEnumerable<CustomPrompt> customPrompts, int customisationId)
        {
            CustomisationId = customisationId;
        }

        public int CustomisationId { get; set; }
    }
}
