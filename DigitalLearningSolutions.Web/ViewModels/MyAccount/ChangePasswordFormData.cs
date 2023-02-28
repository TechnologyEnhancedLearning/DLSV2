namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class ChangePasswordFormData : ConfirmPasswordViewModel
    {
        public ChangePasswordFormData() { }

        protected ChangePasswordFormData(ChangePasswordFormData formData)
        {
            CurrentPassword = formData.CurrentPassword;
            Password = formData.Password;
            ConfirmPassword = formData.ConfirmPassword;
        }

        [Required(ErrorMessage = "Enter your password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
    }
}
