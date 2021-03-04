namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;

    public class DiagnosticAssessmentViewModel
    {

        public string CourseTitle { get; }
        public string? CourseDescription { get; }
        public string SectionName { get; }
        public string DiagnosticAssessmentPath { get; }
        public bool CanSelectTutorials { get; }
        public string AttemptsInformation { get; }
        public bool HasPostLearningAssessment { get; }
        public int? NextTutorialId { get; }
        public int? NextSectionId { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public IEnumerable<DiagnosticTutorial> Tutorials { get; }
        public bool OnlyItemInOnlySection { get; }
        public bool OnlyItemInThisSection { get; }
        public bool ShowCompletionSummary { get; }
        public CompletionSummaryCardViewModel CompletionSummaryCardViewModel { get; }
        public string DiagnosticStartButtonAdditionalStyling { get; }
        public string DiagnosticStartButtonText { get; }
        public bool ShowNextButton { get; }

        public DiagnosticAssessmentViewModel(DiagnosticAssessment diagnosticAssessment, int customisationId, int sectionId)
        {
            CourseTitle = diagnosticAssessment.CourseTitle;
            CourseDescription = diagnosticAssessment.CourseDescription;
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
            HasPostLearningAssessment =
                diagnosticAssessment.PostLearningAssessmentPath != null && diagnosticAssessment.IsAssessed;
            NextTutorialId = diagnosticAssessment.NextTutorialId;
            NextSectionId = diagnosticAssessment.NextSectionId;
            CustomisationId = customisationId;
            SectionId = sectionId;
            Tutorials = diagnosticAssessment.Tutorials;
            OnlyItemInOnlySection = !diagnosticAssessment.OtherItemsInSectionExist && !diagnosticAssessment.OtherSectionsExist;
            OnlyItemInThisSection = !diagnosticAssessment.OtherItemsInSectionExist;
            ShowCompletionSummary = OnlyItemInOnlySection && diagnosticAssessment.IncludeCertification;
            CompletionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                diagnosticAssessment.Completed,
                diagnosticAssessment.MaxPostLearningAssessmentAttempts,
                diagnosticAssessment.IsAssessed,
                diagnosticAssessment.PostLearningAssessmentPassThreshold,
                diagnosticAssessment.DiagnosticAssessmentCompletionThreshold,
                diagnosticAssessment.TutorialsCompletionThreshold
            );
            DiagnosticStartButtonAdditionalStyling = diagnosticAssessment.DiagnosticAttempts > 0 ? "nhsuk-button--secondary" : "";
            DiagnosticStartButtonText = diagnosticAssessment.DiagnosticAttempts > 0 ? "Restart assessment" : "Start assessment";
            ShowNextButton = diagnosticAssessment.DiagnosticAttempts > 0 && !OnlyItemInOnlySection;
        }
    }
}
