namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users
{
    using DigitalLearningSolutions.Web.ViewModels.Common;
    public class SetPasswordFormData : ConfirmPasswordViewModel
    {
        public SetPasswordFormData() { }

        protected SetPasswordFormData(SetPasswordFormData formData)
        {
            Password = formData.Password;
            ConfirmPassword = formData.ConfirmPassword;
        }
    }
}
