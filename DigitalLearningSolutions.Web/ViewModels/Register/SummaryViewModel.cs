namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class SummaryViewModel: IValidatableObject
    {
        public SummaryViewModel() { }

        public SummaryViewModel(string firstName, string lastName, string email, string centre, string jobGroup)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Centre = centre;
            JobGroup = jobGroup;
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Centre { get; set; }
        public string? JobGroup { get; set; }
        public bool Terms { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Terms == false)
            {
                yield return new ValidationResult(
                    "Read and agree to the Terms and Conditions",
                    new[] { "Terms" });
            }
        }
    }
}
