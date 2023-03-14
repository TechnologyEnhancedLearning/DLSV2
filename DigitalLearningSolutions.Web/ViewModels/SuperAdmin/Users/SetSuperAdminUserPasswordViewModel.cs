namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Web.Models.Enums;
    public class SetSuperAdminUserPasswordViewModel : SetSuperAdminUserPasswordFormData
    {
        public SetSuperAdminUserPasswordViewModel(DlsSubApplication dlsSubApplication) : this(
             new SetSuperAdminUserPasswordFormData(),
             dlsSubApplication
         )
        { }

        public SetSuperAdminUserPasswordViewModel(SetSuperAdminUserPasswordFormData formData, DlsSubApplication dlsSubApplication) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public DlsSubApplication DlsSubApplication { get; set; }
    }

}
