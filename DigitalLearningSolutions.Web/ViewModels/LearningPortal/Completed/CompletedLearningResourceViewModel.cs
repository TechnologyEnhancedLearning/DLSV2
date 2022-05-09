namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed
{
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Web.Helpers;

    public class CompletedLearningResourceViewModel : CompletedLearningItemViewModel, ILearningResourceCardViewModel
    {
        public CompletedLearningResourceViewModel(CompletedActionPlanResource resource) : base(resource)
        {
            LaunchResourceLink = resource.ResourceLink;
            ResourceDescription = resource.ResourceDescription;
            CatalogueName = resource.CatalogueName;
            ResourceType = DisplayStringHelper.AddSpacesToPascalCaseString(resource.ResourceType);
            ResourceReferenceId = resource.ResourceReferenceId;
            AbsentInLearningHub = resource.AbsentInLearningHub;
        }

        public int ResourceReferenceId { get; set; }

        public string LaunchResourceLink { get; set; }
        public string ResourceDescription { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
        public bool AbsentInLearningHub { get; set; }
    }
}
