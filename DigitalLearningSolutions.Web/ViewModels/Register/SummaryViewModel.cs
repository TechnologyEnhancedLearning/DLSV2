namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class SummaryViewModel : IValidatableObject
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Centre { get; set; }
        public string? JobGroup { get; set; }
        public bool Terms { get; set; }
        public IEnumerable<CustomFieldViewModel> CustomFields { get; set; } = new List<CustomFieldViewModel>();
        public bool IsCentreSpecificRegistration { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Terms == false)
            {
                yield return new ValidationResult(
                    "Read and agree to the Terms and Conditions",
                    new[] { "Terms" }
                );
            }
        }
    }
}
