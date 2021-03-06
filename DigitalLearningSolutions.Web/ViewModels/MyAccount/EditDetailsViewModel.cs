namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using Microsoft.AspNetCore.Http;

    public class EditDetailsViewModel
    {
        public EditDetailsViewModel() { }

        public EditDetailsViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            List<(int id, string name)> jobGroups)
        {
            FirstName = adminUser?.FirstName ?? delegateUser?.FirstName;
            LastName = adminUser?.LastName ?? delegateUser?.LastName;
            Email = adminUser?.EmailAddress ?? delegateUser?.EmailAddress;
            ProfileImage = adminUser?.ProfileImage ?? delegateUser?.ProfileImage;

            IsDelegateUser = delegateUser != null;
            JobGroupId = jobGroups.Where(jg => jg.name == delegateUser?.JobGroupName).Select(jg => jg.id)
                .SingleOrDefault();

            Answer1 = delegateUser?.Answer1;
            Answer2 = delegateUser?.Answer2;
            Answer3 = delegateUser?.Answer3;
            Answer4 = delegateUser?.Answer4;
            Answer5 = delegateUser?.Answer5;
            Answer6 = delegateUser?.Answer6;
        }

        [Required(ErrorMessage = "Enter your first name")]
        [MaxLength(250, ErrorMessage = "First name must be 250 characters or fewer")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        [MaxLength(250, ErrorMessage = "Last name must be 250 characters or fewer")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        [MaxLength(250, ErrorMessage = "Email address must be 250 characters or fewer")]
        [EmailAddress(ErrorMessage = "Enter an email address in the correct format, like name@example.com")]
        [NoWhitespace("Email address must not contain any whitespace characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter your current password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public byte[]? ProfileImage { get; set; }

        [AllowedExtensions(new []{".png",".tiff",".jpg",".jpeg",".bmp",".gif"})]
        public IFormFile? ProfileImageFile { get; set; }

        public bool IsDelegateUser { get; set; }

        public int? JobGroupId { get; set; }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }
    }
}
