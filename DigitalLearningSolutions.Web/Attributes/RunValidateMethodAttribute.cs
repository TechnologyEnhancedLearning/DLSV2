namespace DigitalLearningSolutions.Web.Attributes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class RunValidateMethodAttribute : ValidationAttribute
    {
        public RunValidateMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; set; }

        public string GetErrorMessage() => string.Empty;

        protected override ValidationResult IsValid(object value,
            ValidationContext validationContext)
        {
            var method = validationContext.ObjectType.GetMethod(MethodName);
            var results = ((IEnumerable<ValidationResult>)method.Invoke(validationContext.ObjectInstance, null)).ToList();

            if (results.Any())
            {
                return results.First();
            }

            return ValidationResult.Success;
        }
    }
}
