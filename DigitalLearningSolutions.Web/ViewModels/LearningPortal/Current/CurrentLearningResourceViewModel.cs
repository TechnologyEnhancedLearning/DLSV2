namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public class CurrentLearningResourceViewModel : CurrentLearningItemViewModel, ILearningResourceCardViewModel
    {
        public CurrentLearningResourceViewModel(ActionPlanResource resource) : base(resource)
        {
            LaunchResourceLink = resource.ResourceLink;
            ResourceDescription = resource.ResourceDescription;
            CatalogueName = resource.CatalogueName;
            ResourceType = resource.ResourceType;
            ResourceReferenceId = resource.ResourceReferenceId;
        }

        public string LaunchResourceLink { get; set; }
        public string ResourceDescription { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
        public int ResourceReferenceId { get; set; }
    }
}
