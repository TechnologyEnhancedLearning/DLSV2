namespace DigitalLearningSolutions.Data.Models.DiagnosticAssessment
{
    using System.Collections.Generic;

    public class DiagnosticContent
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string DiagnosticAssessmentPath { get; }
        public bool CanSelectTutorials { get; }
        public List<int> Tutorials { get; } = new List<int>();

        public DiagnosticContent(
            string applicationName,
            string customisationName,
            string sectionName,
            string diagAssessPath,
            bool diagObjSelect
        )
        {
            CourseTitle = $"{applicationName} - {customisationName}";
            SectionName = sectionName;
            DiagnosticAssessmentPath = diagAssessPath;
            CanSelectTutorials = diagObjSelect;
        }
    }
}
