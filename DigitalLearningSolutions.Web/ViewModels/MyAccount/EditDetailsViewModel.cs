namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.User;

    public class EditDetailsViewModel
    {
        public EditDetailsViewModel() { }

        public EditDetailsViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser)
        {
            FirstName = adminUser?.FirstName ?? delegateUser?.FirstName;
            LastName = adminUser?.LastName ?? delegateUser?.LastName;
            Email = adminUser?.EmailAddress ?? delegateUser?.EmailAddress;
        }

        [Required(ErrorMessage = "Enter a first name.")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name.")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter an email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
