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
        public string? ProgrammeCourse { get; set; }
        public string? Organisation { get; set; }
        public string? DepartmentTeam { get; set; }
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
    }
}
