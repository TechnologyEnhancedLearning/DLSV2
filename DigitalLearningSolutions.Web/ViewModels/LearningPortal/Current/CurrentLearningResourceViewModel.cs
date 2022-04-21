namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class CurrentLearningResourceViewModel : CurrentLearningItemViewModel, ILearningResourceCardViewModel
    {
        public CurrentLearningResourceViewModel(ActionPlanResource resource, ReturnPageQuery returnPageQuery) : base(resource, returnPageQuery)
        {
            LaunchResourceLink = resource.ResourceLink;
            ResourceDescription = resource.ResourceDescription;
            CatalogueName = resource.CatalogueName;
            ResourceType = resource.ResourceType;
            AbsentInLearningHub = resource.AbsentInLearningHub;
            ResourceReferenceId = resource.ResourceReferenceId;
        }

        public string LaunchResourceLink { get; set; }
        public string ResourceDescription { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
        public bool AbsentInLearningHub { get; set; }
        public int ResourceReferenceId { get; set; }
    }
}
