namespace DigitalLearningSolutions.Data.Models.SelfAssessments.Export
{
    using System;
    public class DCSADelegateCompletionStatus
    {
        public int? EnrolledMonth { get; set; }
        public int? EnrolledYear { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? RegistrationAnswer1 { get; set; }
        public string? RegistrationAnswer2 { get; set; }
        public string? RegistrationAnswer3 { get; set; }
        public string? RegistrationAnswer4 { get; set; }
        public string? RegistrationAnswer5 { get; set; }
        public string? RegistrationAnswer6 { get; set; }
        public string? Status { get; set; }


        public string?[] CentreRegistrationPrompts =>
            new[]
            {
                RegistrationAnswer1,
                RegistrationAnswer2,
                RegistrationAnswer3,
                RegistrationAnswer4,
                RegistrationAnswer5,
                RegistrationAnswer6,
            };
    }
}
