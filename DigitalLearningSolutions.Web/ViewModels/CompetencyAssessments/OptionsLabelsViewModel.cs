namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class OptionsLabelsViewModel
    {
        public OptionsLabelsViewModel()
        {
        }
        public OptionsLabelsViewModel(OptionsLabelsViewModel optionsLabels)
        {
            CompetencyAssessmentID = optionsLabels.CompetencyAssessmentID;
            CurrentStep = optionsLabels.CurrentStep;
            IncludeLearnerDeclarationPrompt = optionsLabels.IncludeLearnerDeclarationPrompt;
            IncludesSignposting = optionsLabels.IncludesSignposting;
            LinearNavigation = optionsLabels.LinearNavigation;
            UseDescriptionExpanders = optionsLabels.UseDescriptionExpanders;
            QuestionLabel = optionsLabels.QuestionLabel;
            QuestionLabelText = optionsLabels.QuestionLabelText;
            ReviewerCommentsLabel = optionsLabels.ReviewerCommentsLabel;
            ReviewerCommentsLabelText = optionsLabels.ReviewerCommentsLabelText;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(optionsLabels.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(optionsLabels.Vocabulary);
            SelfAssessmentOptionsTaskStatus = optionsLabels.SelfAssessmentOptionsTaskStatus;
            IsSupervisionSwitchedOn = optionsLabels.IsSupervisionSwitchedOn;
            IsSignpostedLearning = optionsLabels.IsSignpostedLearning;
            UserRole = optionsLabels.UserRole;
        }

        public int FrameworkId { get; set; }
        public int CompetencyAssessmentID { get; set; }
        public int CurrentStep { get; set; } = 1;
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public bool IncludeLearnerDeclarationPrompt { get; set; }
        public bool IncludesSignposting { get; set; }
        public bool LinearNavigation { get; set; }
        public bool UseDescriptionExpanders { get; set; }
        public bool QuestionLabel { get; set; }
        public string? QuestionLabelText { get; set; }
        public bool ReviewerCommentsLabel { get; set; }
        public string? ReviewerCommentsLabelText { get; set; }
        public bool IsSupervisionSwitchedOn { get; set; }
        public bool IsSignpostedLearning { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public string? Vocabulary { get; set; }
        public bool? SelfAssessmentOptionsTaskStatus { get; set; }
        public bool Error { get; set; }
        public int UserRole { get; set; }

    }
    public enum OptionLabel
    {
        Declaration = 1,
        Signposting,
        LinearNavigation,
        DescriptionExpanders,
        QuestionLabels,
        CommentsLabel,
        Summary
    }
}
