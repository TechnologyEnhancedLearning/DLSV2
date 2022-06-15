namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.EditAdminField;

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

        public EditAdminFieldViewModel(EditAdminFieldData data) : base(
            data.OptionsString,
            data.Answer,
            data.IncludeAnswersTableCaption
        )
        {
            Prompt = data.Prompt;
            PromptNumber = data.PromptNumber;
        }

        public int PromptNumber { get; set; }

        public string Prompt { get; set; }
    }
}
