namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;

    public class DiagnosticAssessmentViewModel
    {

        public string CourseTitle { get; }
        public string SectionName { get; }
        public string DiagnosticAssessmentPath { get; }
        public bool CanSelectTutorials { get; }
        public string AttemptsInformation { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public IEnumerable<DiagnosticTutorial> Tutorials { get; }

        public DiagnosticAssessmentViewModel(DiagnosticAssessment diagnosticAssessment, int customisationId, int sectionId)
        {
            CourseTitle = diagnosticAssessment.CourseTitle;
            SectionName = diagnosticAssessment.SectionName;
            DiagnosticAssessmentPath = diagnosticAssessment.DiagnosticAssessmentPath;
            CanSelectTutorials = diagnosticAssessment.CanSelectTutorials && diagnosticAssessment.Tutorials.Any();
            AttemptsInformation = diagnosticAssessment.DiagnosticAttempts switch
            {
                0 => "Not attempted",
                1 => $"{diagnosticAssessment.SectionScore}/{diagnosticAssessment.MaxSectionScore} " +
                     $"- {diagnosticAssessment.DiagnosticAttempts} attempt",
                _ => $"{diagnosticAssessment.SectionScore}/{diagnosticAssessment.MaxSectionScore} " +
                     $"- {diagnosticAssessment.DiagnosticAttempts} attempts"
            };
            CustomisationId = customisationId;
            SectionId = sectionId;
            Tutorials = diagnosticAssessment.Tutorials;
        }
    }
}
