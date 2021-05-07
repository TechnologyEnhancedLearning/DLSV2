namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Http;

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
            CurrentProfileImage = adminUser?.ProfileImage ?? delegateUser?.ProfileImage;
        }

        [Required(ErrorMessage = "Enter a first name.")]
        [MaxLength(100, ErrorMessage = "First name must be at most 100 characters")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter a last name.")]
        [MaxLength(100, ErrorMessage = "Last name must be at most 100 characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter an email address.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(100, ErrorMessage = "Email address must be at most 100 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        [MaxLength(100, ErrorMessage = "Password must be at most 100 characters")]
        public string? Password { get; set; }

        public byte[]? CurrentProfileImage { get; set; }

        public byte[]? NewProfileImage { get; set; }

        public bool HasProfileImageBeenRemoved { get; set; }

        [AllowedExtensions(new []{".png",".tiff",".jpg",".jpeg",".bmp",".gif"})]
        public IFormFile? ProfilePicture { get; set; }
    }
}
