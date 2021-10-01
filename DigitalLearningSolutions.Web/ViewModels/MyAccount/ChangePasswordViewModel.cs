namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class ChangePasswordViewModel : ChangePasswordFormData
    {
        public ChangePasswordViewModel(ApplicationType application) : this(
            new ChangePasswordFormData(),
            application
        ) { }

        public ChangePasswordViewModel(ChangePasswordFormData formData, ApplicationType application) : base(formData)
        {
            Application = application;
        }

        public ApplicationType Application { get; set; }
    }
}
