namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public class CurrentLearningResourceViewModel : CurrentLearningItemViewModel, ILearningResourceCardViewModel
    {
        public CurrentLearningResourceViewModel(ActionPlanResource resource, bool isLearningHubAccountLinked) : base(
            resource
        )
        {
            LaunchResourceLink = resource.ResourceLink;
            ResourceDescription = resource.ResourceDescription;
            CatalogueName = resource.CatalogueName;
            ResourceType = resource.ResourceType;
            AbsentInLearningHub = resource.AbsentInLearningHub;
            ResourceReferenceId = resource.ResourceReferenceId;
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
