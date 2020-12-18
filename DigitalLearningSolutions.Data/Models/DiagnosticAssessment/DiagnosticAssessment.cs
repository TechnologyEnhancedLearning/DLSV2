namespace DigitalLearningSolutions.Data.Models.DiagnosticAssessment
{
    using System.Collections.Generic;

    public class DiagnosticAssessment
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public int DiagnosticAttempts { get; set; }
        public int SectionScore { get; set; }
        public int MaxSectionScore { get; set; }
        public string DiagnosticAssessmentPath { get; }
        public bool SelectTutorials { get; }
        public List<DiagnosticTutorial> Tutorials { get; } = new List<DiagnosticTutorial>();

        public DiagnosticAssessment(
            string applicationName,
            string customisationName,
            string sectionName,
            int diagAttempts,
            int diagLast,
            int diagAssessOutOf,
            string diagAssessPath,
            bool diagObjSelect
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            DiagnosticAttempts = diagAttempts;
            SectionScore = diagLast;
            MaxSectionScore = diagAssessOutOf;
            DiagnosticAssessmentPath = diagAssessPath;
            SelectTutorials = diagObjSelect;
        }
    }
}
