namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.ComponentModel.DataAnnotations;

    public class AddAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public AddAdminFieldViewModel()
        {
            IncludeAnswersTableCaption = true;
        }

        public AddAdminFieldViewModel(int customisationId)
        {
            CustomisationId = customisationId;
            IncludeAnswersTableCaption = true;
        }

        public AddAdminFieldViewModel(
            int customisationId,
            int? adminFieldId,
            string? options,
            string? answer = null
        )
        {
            CustomisationId = customisationId;
            AdminFieldId = adminFieldId;
            OptionsString = options;
            IncludeAnswersTableCaption = true;
            Answer = answer;
        }

        [Required(ErrorMessage = "Select a field name")]
        public int? AdminFieldId { get; set; }
    }
}
