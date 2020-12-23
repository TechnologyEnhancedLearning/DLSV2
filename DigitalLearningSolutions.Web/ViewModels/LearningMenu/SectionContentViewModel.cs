namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SectionContent;

    public class SectionContentViewModel
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public string PercentComplete { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public bool ShowPostLearning { get; }
        public string PostLearningStatus { get; }
        public bool ShowDiagnostic { get; }
        public string DiagnosticCompletionStatus { get; }
        public IEnumerable<TutorialCardViewModel> Tutorials { get; }
        public bool DisplayDiagnosticSeparator { get; }
        public bool DisplayTutorialSeparator { get; }

        public SectionContentViewModel(SectionContent sectionContent, int customisationId, int sectionId)
        {
            CourseTitle = sectionContent.CourseTitle;
            SectionName = sectionContent.SectionName;
            PercentComplete = sectionContent.HasLearning ? $"{GetPercentComplete(sectionContent):f0}% Complete" : "";
            CustomisationId = customisationId;
            SectionId = sectionId;
            ShowPostLearning = sectionContent.PostLearningAssessmentPath != null && sectionContent.IsAssessed;
            PostLearningStatus = GetPostLearningStatus(sectionContent);
            ShowDiagnostic = sectionContent.DiagnosticAssessmentPath != null && sectionContent.DiagnosticStatus;
            DiagnosticCompletionStatus = GetDiagnosticCompletionStatus(sectionContent);
            Tutorials = sectionContent.Tutorials.Select(tutorial => new TutorialCardViewModel(tutorial, sectionId, customisationId));
            DisplayDiagnosticSeparator = ShowDiagnostic && (sectionContent.Tutorials.Any() || ShowPostLearning);
            DisplayTutorialSeparator = sectionContent.Tutorials.Any() && ShowPostLearning;
        }

        private static double GetPercentComplete(SectionContent sectionContent)
        {
            var totalStatus = sectionContent.Tutorials.Sum(tutorial => tutorial.TutorialStatus);
            return sectionContent.Tutorials.Count == 0 || !sectionContent.HasLearning
                ? 0
                : (totalStatus * 100) / (sectionContent.Tutorials.Count * 2);
        }

        private static string GetPostLearningStatus(SectionContent sectionContent)
        {
            if (sectionContent.PostLearningAttempts == 0)
            {
                return "Not Attempted";
            }

            if (sectionContent.PostLearningAttempts > 0 && sectionContent.PostLearningPassed)
            {
                return "Passed";
            }

            if (sectionContent.PostLearningAttempts > 0 && !sectionContent.PostLearningPassed)
            {
                return "Failed";
            }

            return "";
        }

        private static string GetDiagnosticCompletionStatus(SectionContent sectionContent)
        {
            if (sectionContent.DiagnosticAttempts == 0)
            {
                return "Not Attempted";
            }

            if (sectionContent.DiagnosticAttempts > 0)
            {
                return $"{sectionContent.SectionScore}/{sectionContent.MaxSectionScore} - {sectionContent.DiagnosticAttempts} attempts";
            }

            return "";
        }
    }
}
