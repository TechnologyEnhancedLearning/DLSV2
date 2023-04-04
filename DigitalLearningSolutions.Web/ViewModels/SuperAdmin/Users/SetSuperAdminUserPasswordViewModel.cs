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
        public string? SearchString { get; set; }

        public string? ExistingFilterString { get; set; }

        public int Page { get; set; }
        public int UserId { get; set; }
        public new string UserName { get; set; }
    }

}
