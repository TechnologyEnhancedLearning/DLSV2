namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Web.Models.Enums;
    public class SetAdminUserPasswordViewModel : SetPasswordFormData
    {
        public SetAdminUserPasswordViewModel(DlsSubApplication dlsSubApplication) : this(
             new SetPasswordFormData(),
             dlsSubApplication
         )
        { }

        public SetAdminUserPasswordViewModel(SetPasswordFormData formData, DlsSubApplication dlsSubApplication) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public DlsSubApplication DlsSubApplication { get; set; }
    }

}
