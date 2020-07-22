using System;

namespace DigitalLearningSolutions.Data.Models
{
    public class CurrentCourse
    {
        public string CourseName { get; set; }
        public int CustomisationID { get; set; }
        public bool HasDiagnostic { get; set; }
        public bool HasLearning { get; set; }
        public bool IsAssessed { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime LastAccessed { get; set; }
        public DateTime CompleteByDate { get; set; }
        public int? DiagnosticScore { get; set; }
        public int Passes { get; set; }
        public int Sections { get; set; }
        public int SupervisorAdminId { get; set; }
        public int GroupCustomisationId { get; set; }
    }
}
