namespace DigitalLearningSolutions.Web.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Validates whether an property of type <see cref="IFormFile"/> has an accepted
    /// file type
    /// </summary>
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;
        private readonly string? errorMessage;

        public AllowedExtensionsAttribute(string[] allowedExtensions, string? errorMessage = null)
        {
            this.extensions = allowedExtensions;
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(errorMessage ?? GetErrorMessage());
                }
            }
            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            var baseString = "File must be in one of the following formats: ";
            return baseString + string.Join(", ", extensions);
        }
    }
}
