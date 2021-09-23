namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class EditDetailsViewModel
    {
        public EditDetailsViewModel(EditDetailsFormData formData, ApplicationType application)
        {
            FormData = formData;
            Application = application;
        }

        public EditDetailsFormData FormData { get; set; }

        public ApplicationType Application { get; set; }
    }
}
