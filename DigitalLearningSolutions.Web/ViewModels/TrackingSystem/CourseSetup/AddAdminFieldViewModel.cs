namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.MultiPageFormData.AddAdminField;

    public class AddAdminFieldViewModel : AdminFieldAnswersViewModel
    {
        public AddAdminFieldViewModel()
        {
            IncludeAnswersTableCaption = true;
        }

        public AddAdminFieldViewModel(
            int? adminFieldId,
            string? options,
            string? answer = null
        )
        {
            AdminFieldId = adminFieldId;
            OptionsString = options;
            IncludeAnswersTableCaption = true;
            Answer = answer;
        }

        public AddAdminFieldViewModel(AddAdminFieldTempData tempData) : base(
            tempData.OptionsString,
            tempData.Answer,
            tempData.IncludeAnswersTableCaption
        )
        {
            AdminFieldId = tempData.AdminFieldId;
        }

        [Required(ErrorMessage = "Select a field name")]
        public int? AdminFieldId { get; set; }
    }
}
