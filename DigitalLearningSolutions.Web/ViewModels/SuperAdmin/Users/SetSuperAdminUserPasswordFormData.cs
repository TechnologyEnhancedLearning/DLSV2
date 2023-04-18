namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    public class SetSuperAdminUserPasswordFormData : ConfirmPasswordViewModel
    {
        public SetSuperAdminUserPasswordFormData() { }

        protected SetSuperAdminUserPasswordFormData(SetSuperAdminUserPasswordFormData formData)
        {
            Password = formData.Password;
            ConfirmPassword = formData.ConfirmPassword;
            UserName = formData.UserName;
        }
        public string UserName { get; set; }
    }
}
