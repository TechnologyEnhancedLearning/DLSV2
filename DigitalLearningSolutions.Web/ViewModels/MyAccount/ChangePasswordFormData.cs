namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class ChangePasswordFormData : ConfirmPasswordViewModel
    {
        [Required(ErrorMessage = "Enter your password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
    }
}
