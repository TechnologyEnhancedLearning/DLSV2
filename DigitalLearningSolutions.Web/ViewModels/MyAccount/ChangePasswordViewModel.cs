namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class ChangePasswordViewModel : ChangePasswordFormData
    {
        public ChangePasswordViewModel(DlsSubApplication dlsSubApplication) : this(
            new ChangePasswordFormData(),
            dlsSubApplication
        )
        { }

        public ChangePasswordViewModel(ChangePasswordFormData formData, DlsSubApplication dlsSubApplication) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
