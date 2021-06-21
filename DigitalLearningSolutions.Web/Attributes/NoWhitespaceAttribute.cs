namespace DigitalLearningSolutions.Web.Attributes
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    ///     Specifies that no whitespace characters are allowed
    /// </summary>
    public class NoWhitespaceAttribute : ValidationAttribute
    {
        private readonly string errorMessage;

        public NoWhitespaceAttribute(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            switch (value)
            {
                case null:
                case string strValue when !strValue.Any(char.IsWhiteSpace):
                    return ValidationResult.Success;
                default:
                    return new ValidationResult(errorMessage);
            }
        }
    }
}
