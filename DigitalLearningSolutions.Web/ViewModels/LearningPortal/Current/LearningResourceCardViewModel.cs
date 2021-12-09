namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public class LearningResourceCardViewModel : CurrentLearningItemViewModel
    {
        public LearningResourceCardViewModel(ActionPlanResource item) : base(item)
        {
            LaunchResourceLink = item.ResourceLink;
            ResourceDescription = item.ResourceDescription;
            CatalogueName = item.CatalogueName;
            ResourceType = item.ResourceType;
        }

        public string LaunchResourceLink { get; set; }
        public string ResourceDescription { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
    }
}
