namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Http;

    public class EditDetailsFormData: IValidatableObject
    {
        public EditDetailsFormData() { }

        public EditDetailsFormData(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            List<(int id, string name)> jobGroups
        )
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
            
            if (IsDelegateUser)
            {
               HasBeenPromptedForPrn = delegateUser!.HasBeenPromptedForPrn;
               ProfessionalRegistrationNumber = delegateUser?.ProfessionalRegistrationNumber!;
            }
        }

        protected EditDetailsFormData(EditDetailsFormData formData)
        {
            FirstName = formData.FirstName;
            LastName = formData.LastName;
            Email = formData.Email;
            ProfileImageFile = formData.ProfileImageFile;
            ProfileImage = formData.ProfileImage;
            IsDelegateUser = formData.IsDelegateUser;
            JobGroupId = formData.JobGroupId;
            Answer1 = formData.Answer1;
            Answer2 = formData.Answer2;
            Answer3 = formData.Answer3;
            Answer4 = formData.Answer4;
            Answer5 = formData.Answer5;
            Answer6 = formData.Answer6;
        }

        [Required(ErrorMessage = "Enter your first name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongFirstName)]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Enter your last name")]
        [MaxLength(250, ErrorMessage = CommonValidationErrorMessages.TooLongLastName)]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Enter your email address")]
        [MaxLength(255, ErrorMessage = CommonValidationErrorMessages.TooLongEmail)]
        [EmailAddress(ErrorMessage = CommonValidationErrorMessages.InvalidEmail)]
        [NoWhitespace(CommonValidationErrorMessages.WhitespaceInEmail)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter your current password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public byte[]? ProfileImage { get; set; }

        [AllowedExtensions(new[] { ".png", ".tiff", ".jpg", ".jpeg", ".bmp", ".gif" })]
        public IFormFile? ProfileImageFile { get; set; }

        public bool IsDelegateUser { get; set; }

        public int? JobGroupId { get; set; }

        public string? Answer1 { get; set; }

        public string? Answer2 { get; set; }

        public string? Answer3 { get; set; }

        public string? Answer4 { get; set; }

        public string? Answer5 { get; set; }

        public string? Answer6 { get; set; }

        public bool HasBeenPromptedForPrn { get; set; }

        public string ProfessionalRegistrationNumber { get; set; }

        public string SelectedProfessionalRegOption { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsDelegateUser)
            {
                if (string.IsNullOrEmpty(SelectedProfessionalRegOption))
                {
                    yield return new ValidationResult("Must select professional registration number option");
                }

            }
            
        }
    }
}

