namespace DigitalLearningSolutions.Web.ViewModels.LearningMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using Microsoft.Extensions.Configuration;

    public class SectionContentViewModel
    {
        public string CourseTitle { get; }
        public string? CourseDescription { get; }
        public string SectionName { get; }
        public bool ShowPercentComplete { get; }
        public string PercentComplete { get; }
        public int CustomisationId { get; }
        public int SectionId { get; }
        public bool ShowPostLearning { get; }
        public string PostLearningStatus { get; }
        public string PostLearningStatusStyling { get; }
        public bool ShowDiagnostic { get; }
        public string DiagnosticCompletionStatus { get; }
        public string? ConsolidationExercisePath { get; }
        public bool ShowConsolidation { get; }
        public string ConsolidationExerciseLabel { get; }
        public IEnumerable<TutorialCardViewModel> Tutorials { get; }
        public bool DisplayDiagnosticSeparator { get; }
        public bool DisplayTutorialSeparator { get; }
        public bool DisplayPostLearningSeparator { get; }
        public int? NextSectionId { get; }
        public bool ShowCompletionSummary { get; }
        public bool OtherSectionsExist { get; }
        public CompletionSummaryCardViewModel CompletionSummaryCardViewModel { get; }

        public SectionContentViewModel(IConfiguration config, SectionContent sectionContent, int customisationId, int sectionId)
        {
            CourseTitle = sectionContent.CourseTitle;
            CourseDescription = sectionContent.CourseDescription;
            SectionName = sectionContent.SectionName;
            PercentComplete = FormatPercentComplete(sectionContent);
            ShowPercentComplete = sectionContent.HasLearning && sectionContent.CourseSettings.ShowPercentage;
            CustomisationId = customisationId;
            SectionId = sectionId;
            ShowPostLearning = sectionContent.PostLearningAssessmentPath != null && sectionContent.IsAssessed;
            PostLearningStatus = GetPostLearningStatus(sectionContent);
            PostLearningStatusStyling = GetPostLearningStatusStyling(sectionContent);
            ShowDiagnostic = sectionContent.DiagnosticAssessmentPath != null && sectionContent.DiagnosticStatus;
            DiagnosticCompletionStatus = GetDiagnosticCompletionStatus(sectionContent);
            ConsolidationExercisePath = sectionContent.ConsolidationPath == null
                ? null
                : config.GetConsolidationPathUrl(sectionContent.ConsolidationPath);
            ShowConsolidation = ConsolidationExercisePath != null;
            ConsolidationExerciseLabel = sectionContent.CourseSettings.ConsolidationExercise ?? "Consolidation Exercise";

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
            NextSectionId = sectionContent.NextSectionId;
            ShowCompletionSummary = sectionContent.IncludeCertification && !sectionContent.OtherSectionsExist;
            OtherSectionsExist = sectionContent.OtherSectionsExist;
            CompletionSummaryCardViewModel = new CompletionSummaryCardViewModel(
                customisationId,
                sectionContent.Completed,
                sectionContent.MaxPostLearningAssessmentAttempts,
                sectionContent.IsAssessed,
                sectionContent.PostLearningAssessmentPassThreshold,
                sectionContent.DiagnosticAssessmentCompletionThreshold,
                sectionContent.TutorialsCompletionThreshold
            );
        }

        private static string FormatPercentComplete(SectionContent sectionContent)
        {
            var totalStatus = sectionContent.Tutorials.Sum(tutorial => tutorial.TutorialStatus);
            double percentage =
                sectionContent.Tutorials.Count == 0 || !sectionContent.HasLearning
                    ? 0
                    : (totalStatus * 100) / (sectionContent.Tutorials.Count * 2);

            return $"{Convert.ToInt32(Math.Floor(percentage))}% learning complete";
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

        private static string GetPostLearningStatusStyling(SectionContent sectionContent)
        {
            return sectionContent.PostLearningAttempts > 0 && sectionContent.PostLearningPassed
                ? "passed-text"
                : "not-passed-text";
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
