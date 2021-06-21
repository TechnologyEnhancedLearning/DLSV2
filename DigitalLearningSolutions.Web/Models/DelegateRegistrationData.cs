namespace DigitalLearningSolutions.Web.Models
{
    using System;

    public class DelegateRegistrationData
    {
        public DelegateRegistrationData()
        {
            Id = new Guid();
        }

        public Guid Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int? Centre { get; set; }
        public bool IsCentreSpecificRegistration { get; set; }

        public int? JobGroup { get; set; }
        public string? Answer1 { get; set; }
        public string? Answer2 { get; set; }
        public string? Answer3 { get; set; }
        public string? Answer4 { get; set; }
        public string? Answer5 { get; set; }
        public string? Answer6 { get; set; }

        public string? PasswordHash { get; set; }
    }
}
