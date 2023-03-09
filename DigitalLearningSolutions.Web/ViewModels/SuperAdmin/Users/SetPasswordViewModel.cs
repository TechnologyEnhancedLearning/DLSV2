namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Web.Models.Enums;
    public class SetPasswordViewModel : SetPasswordFormData
    {
        public SetPasswordViewModel(DlsSubApplication dlsSubApplication) : this(
            new SetPasswordFormData(),
            dlsSubApplication
        )
        { }

        public SetPasswordViewModel(SetPasswordFormData formData, DlsSubApplication dlsSubApplication) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
