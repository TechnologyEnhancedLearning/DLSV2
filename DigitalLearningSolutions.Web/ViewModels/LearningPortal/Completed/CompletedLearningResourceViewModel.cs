namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public class CompletedLearningResourceViewModel : CompletedLearningItemViewModel, ILearningResourceCardViewModel
    {
        public CompletedLearningResourceViewModel(CompletedActionPlanResource resource) : base(resource)
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
