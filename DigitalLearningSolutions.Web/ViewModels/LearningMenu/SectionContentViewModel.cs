namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Configuration;

    public class SectionContentViewModel
    {
        public string CourseTitle { get; }
        public string SectionName { get; }
        public bool ShowPercentComplete { get; }
        public string PercentComplete { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public bool ShowPostLearning { get; }
        public string PostLearningStatus { get; }
        public bool ShowDiagnostic { get; }
        public string DiagnosticCompletionStatus { get; }
        public string? ConsolidationExercisePath { get; }
        public bool ShowConsolidation { get; }
        public IEnumerable<TutorialCardViewModel> Tutorials { get; }
        public bool DisplayDiagnosticSeparator { get; }
        public bool DisplayTutorialSeparator { get; }
        public bool DisplayPostLearningSeparator { get; }

        public SectionContentViewModel(IConfiguration config, SectionContent sectionContent, int customisationId, int sectionId)
        {
            CourseTitle = sectionContent.CourseTitle;
            SectionName = sectionContent.SectionName;
            PercentComplete = FormatPercentComplete(sectionContent);
            ShowPercentComplete = sectionContent.HasLearning && sectionContent.CourseSettings.ShowPercentage;
            CustomisationId = customisationId;
            SectionId = sectionId;
            ShowPostLearning = sectionContent.PostLearningAssessmentPath != null && sectionContent.IsAssessed;
            PostLearningStatus = GetPostLearningStatus(sectionContent);
            ShowDiagnostic = sectionContent.DiagnosticAssessmentPath != null && sectionContent.DiagnosticStatus;
            DiagnosticCompletionStatus = GetDiagnosticCompletionStatus(sectionContent);
            ConsolidationExercisePath = ContentUrlHelper.GetNullableContentPath(
                config,
                sectionContent.CourseSettings.ConsolidationExercise ?? sectionContent.ConsolidationPath
            );
            ShowConsolidation = ConsolidationExercisePath != null;

            Tutorials = sectionContent.Tutorials.Select(tutorial => new TutorialCardViewModel(
                tutorial,
                sectionContent.CourseSettings.ShowTime,
                sectionContent.CourseSettings.ShowLearnStatus,
                sectionId,
                customisationId
            ));

            DisplayDiagnosticSeparator = ShowDiagnostic && (sectionContent.Tutorials.Any() || ShowPostLearning || ShowConsolidation);
            DisplayTutorialSeparator = sectionContent.Tutorials.Any() && (ShowPostLearning || ShowConsolidation);
            DisplayPostLearningSeparator = ShowConsolidation && ShowPostLearning;
        }

        private static string FormatPercentComplete(SectionContent sectionContent)
        {
            var totalStatus = sectionContent.Tutorials.Sum(tutorial => tutorial.TutorialStatus);
            var percentage =
                sectionContent.Tutorials.Count == 0 || !sectionContent.HasLearning
                    ? 0
                    : (totalStatus * 100) / (sectionContent.Tutorials.Count * 2);

            return $"{percentage:f0}% Complete";
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

            if (sectionContent.DiagnosticAttempts == 1)
            {
                return $"{sectionContent.SectionScore}/{sectionContent.MaxSectionScore} - {sectionContent.DiagnosticAttempts} attempt";
            }

            if (sectionContent.DiagnosticAttempts > 0)
            {
                return $"{sectionContent.SectionScore}/{sectionContent.MaxSectionScore} - {sectionContent.DiagnosticAttempts} attempts";
            }

            return "";
        }
    }
}
