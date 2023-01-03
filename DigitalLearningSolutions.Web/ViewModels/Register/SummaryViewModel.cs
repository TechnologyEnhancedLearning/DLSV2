namespace DigitalLearningSolutions.Web.ViewModels.Register
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class SummaryViewModel : IValidatableObject
    {
        public SummaryViewModel() { }

        public SummaryViewModel(RegistrationData data)
        {
            FirstName = data.FirstName;
            LastName = data.LastName;
            PrimaryEmail = data.PrimaryEmail;
            CentreSpecificEmail = data.CentreSpecificEmail;
            ProfessionalRegistrationNumber = data.ProfessionalRegistrationNumber ?? "Not professionally registered";
            HasProfessionalRegistrationNumber = data.HasProfessionalRegistrationNumber;
            IsPasswordSet = string.IsNullOrEmpty(data.PasswordHash) ? false:true;
        }

        public SummaryViewModel(DelegateRegistrationData data) : this((RegistrationData)data)
        {
            IsCentreSpecificRegistration = data.IsCentreSpecificRegistration;
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? CentreSpecificEmail { get; set; }
        public string? Centre { get; set; }
        public string? JobGroup { get; set; }
        public bool Terms { get; set; }
        public IEnumerable<DelegateRegistrationPrompt> DelegateRegistrationPrompts { get; set; } = new List<DelegateRegistrationPrompt>();
        public bool IsCentreSpecificRegistration { get; set; }
        public string? ProfessionalRegistrationNumber { get; set; }
        public bool? HasProfessionalRegistrationNumber { get; set; }
        public bool IsPasswordSet { get; set; }

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
