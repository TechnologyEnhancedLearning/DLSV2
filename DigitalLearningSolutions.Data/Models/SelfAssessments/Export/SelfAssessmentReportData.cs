namespace DigitalLearningSolutions.Data.Models.SelfAssessments.Export
{
    using System;
    public class SelfAssessmentReportData
    {
        public string? SelfAssessment { get; set; }
        public string? Learner { get; set; }
        public bool LearnerActive { get; set; }
        public string? PRN { get; set; }
        public string? JobGroup { get; set; }
        public string? RegistrationAnswer1 { get; set; }
        public string? RegistrationAnswer2 { get; set; }
        public string? RegistrationAnswer3 { get; set; }
        public string? RegistrationAnswer4 { get; set; }
        public string? RegistrationAnswer5 { get; set; }
        public string? RegistrationAnswer6 { get; set; }
        public string? OtherCentres { get; set; }
        public string? DLSRole { get; set; }
        public DateTime? Registered { get; set; }
        public DateTime? Started { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int? OptionalProficienciesAssessed { get; set; }
        public int? SelfAssessedAchieved { get; set; }
        public int? ConfirmedResults { get; set; }
        public DateTime? SignOffRequested { get; set; }
        public bool SignOffAchieved { get; set; }
        public DateTime? ReviewedDate { get; set; }

        // we need this for iteration across the registration answers from Delegate Accounts which match the custom fields of Centres.
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
