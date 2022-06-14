namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Web.Helpers;

    public class EditAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public EditAdminFieldViewModel() { }

        public EditAdminFieldViewModel(CourseAdminField courseAdminField)
        {
            PromptNumber = courseAdminField.PromptNumber;
            Prompt = courseAdminField.PromptText;
            OptionsString = NewlineSeparatedStringListHelper.JoinNewlineSeparatedList(courseAdminField.Options);
            IncludeAnswersTableCaption = true;
        }

        public EditAdminFieldViewModel(
            int customPromptNumber,
            string text,
            string? options
        )
        {
            PromptNumber = customPromptNumber;
            Prompt = text;
            OptionsString = options;
            IncludeAnswersTableCaption = true;
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }
    }
}
