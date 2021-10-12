namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class SelfAssessmentDescriptionViewModel
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Description;
        public readonly bool UseFilteredApi;
        public readonly string? UserBookmark;
        public readonly bool UnprocessedUpdates;
        public readonly bool LinearNavigation;
        public readonly bool IsSupervised;
        public readonly string? Vocabulary;
        public readonly string VocabPlural;
        public List<SelfAssessmentSupervisor> Supervisors { get; set; }
        public SelfAssessmentDescriptionViewModel(CurrentSelfAssessment selfAssessment, List<SelfAssessmentSupervisor> supervisors)
        {
            Id = selfAssessment.Id;
            Name = selfAssessment.Name;
            Description = selfAssessment.Description;
            UseFilteredApi = selfAssessment.UseFilteredApi;
            UserBookmark = selfAssessment.UserBookmark;
            UnprocessedUpdates = selfAssessment.UnprocessedUpdates;
            LinearNavigation = selfAssessment.LinearNavigation;
            IsSupervised = selfAssessment.IsSupervised;
            Supervisors = supervisors;
            Vocabulary = selfAssessment.Vocabulary;
            VocabPlural = FrameworkVocabularyHelper.VocabularyPlural(selfAssessment.Vocabulary);
        }
    }
}
