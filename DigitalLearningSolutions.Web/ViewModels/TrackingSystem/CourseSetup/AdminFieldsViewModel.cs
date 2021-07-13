namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class AdminFieldsViewModel : DisplayPromptsViewModel
    {
        public AdminFieldsViewModel(CourseCustomPrompts courseCustomPrompts) : base(courseCustomPrompts.CourseAdminFields)
        {
        }
    }
}
