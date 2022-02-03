namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public class CompletedLearningResourceViewModel : CompletedLearningItemViewModel, ILearningResourceCardViewModel
    {
        public CompletedLearningResourceViewModel(
            CompletedActionPlanResource resource,
            bool isLearningHubAccountLinked
        ) : base(resource)
        {
            LaunchResourceLink = resource.ResourceLink;
            ResourceDescription = resource.ResourceDescription;
            CatalogueName = resource.CatalogueName;
            ResourceType = resource.ResourceType;
            ResourceReferenceId = resource.ResourceReferenceId;
            AbsentInLearningHub = resource.AbsentInLearningHub;
            IsLearningHubAccountLinked = isLearningHubAccountLinked;
        }

        public int ResourceReferenceId { get; set; }
        public bool IsLearningHubAccountLinked { get; set; }

        public string LaunchResourceLink { get; set; }
        public string ResourceDescription { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
        public bool AbsentInLearningHub { get; set; }
    }
}
