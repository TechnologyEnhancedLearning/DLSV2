namespace DigitalLearningSolutions.Web.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Specifies that no whitespace characters are allowed
    /// </summary>
    public class NoWhitespaceAttribute : ValidationAttribute
    {
        private readonly string errorMessage;

        public NoWhitespaceAttribute(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value.ToString().Any(char.IsWhiteSpace))
            {
                return new ValidationResult(errorMessage); 
            }
            return ValidationResult.Success;
        }
    }
}
