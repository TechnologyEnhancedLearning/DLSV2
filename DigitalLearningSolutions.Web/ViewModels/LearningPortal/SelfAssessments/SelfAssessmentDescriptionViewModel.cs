namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;

    public class SelfAssessmentDescriptionViewModel
    {
        public readonly string Description;
        public readonly int Id;
        public readonly bool IncludesSignposting;
        public readonly bool IsSupervised;
        public readonly bool IsSupervisorResultsReviewed;
        public readonly bool LinearNavigation;
        public readonly string Name;
        public readonly bool UnprocessedUpdates;
        public readonly string? UserBookmark;
        public readonly string VocabPlural;
        public readonly string? Vocabulary;

        public SelfAssessmentDescriptionViewModel(
            CurrentSelfAssessment selfAssessment,
            List<SelfAssessmentSupervisor> supervisors
        )
        {
            Id = selfAssessment.Id;
            Name = selfAssessment.Name;
            Description = selfAssessment.Description;
            IncludesSignposting = selfAssessment.IncludesSignposting;
            UserBookmark = selfAssessment.UserBookmark;
            UnprocessedUpdates = selfAssessment.UnprocessedUpdates;
            LinearNavigation = selfAssessment.LinearNavigation;
            IsSupervised = selfAssessment.IsSupervised;
            IsSupervisorResultsReviewed = selfAssessment.IsSupervisorResultsReviewed;
            Supervisors = supervisors;
            Vocabulary = selfAssessment.Vocabulary;
            VocabPlural = FrameworkVocabularyHelper.VocabularyPlural(selfAssessment.Vocabulary);
        }

        public List<SelfAssessmentSupervisor> Supervisors { get; set; }
    }
}
