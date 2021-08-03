namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.AspNetCore.Hosting;

    public class SelfAssessmentDescriptionViewModel
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Description;
        public readonly bool UseFilteredApi;
        public readonly string? UserBookmark;
        public readonly bool UnprocessedUpdates;
        public readonly bool LinearNavigation;
        public SelfAssessmentDescriptionViewModel(CurrentSelfAssessment selfAssessment)
        {
            Id = selfAssessment.Id;
            Name = selfAssessment.Name;
            Description = selfAssessment.Description;
            UseFilteredApi = selfAssessment.UseFilteredApi;
            UserBookmark = selfAssessment.UserBookmark;
            UnprocessedUpdates = selfAssessment.UnprocessedUpdates;
            LinearNavigation = selfAssessment.LinearNavigation;
        }
    }
}
