namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class ChangePasswordViewModel
    {
        public ChangePasswordViewModel(ApplicationType application) : this(
            new ChangePasswordFormData(),
            application
        ) { }

        public ChangePasswordViewModel(ChangePasswordFormData formData, ApplicationType application)
        {
            FormData = formData;
            Application = application;
        }

        public ChangePasswordFormData FormData { get; set; }

        public ApplicationType Application { get; set; }
    }
}
