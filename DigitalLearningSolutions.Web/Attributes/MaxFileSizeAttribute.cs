namespace DigitalLearningSolutions.Web.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    ///     Validates whether a property of type <see cref="IFormFile" /> has
    ///     file size smaller than a maximum file size
    /// </summary>
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly string? errorMessage;
        private readonly int maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize, string? errorMessage = null)
        {
            this.maxFileSize = maxFileSize;
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file && file.Length > maxFileSize)
            {
                return new ValidationResult(errorMessage ?? GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Maximum allowed file size is {maxFileSize} bytes.";
        }
    }
}
