namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public class AdminFieldsViewModel
    {
        public AdminFieldsViewModel(CourseCustomPrompts courseCustomPrompts)
        {
            AdminFields = courseCustomPrompts.CourseAdminFields;
        }

        public IEnumerable<CustomPrompt> AdminFields { get; set; }
    }
}
