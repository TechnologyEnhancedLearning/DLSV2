namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.Current
{
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;

    public class CurrentLearningResourceViewModel : CurrentLearningItemViewModel, ILearningResourceCardViewModel
    {
        public CurrentLearningResourceViewModel(ActionPlanResource resource, ReturnPageQuery returnPageQuery) : base(
            resource,
            returnPageQuery
        )
        {
            LaunchResourceLink = resource.ResourceLink;
            ResourceDescription = resource.ResourceDescription;
            CatalogueName = resource.CatalogueName;
            ResourceType = DisplayStringHelper.AddSpacesToPascalCaseString(resource.ResourceType);
            AbsentInLearningHub = resource.AbsentInLearningHub;
            ResourceReferenceId = resource.ResourceReferenceId;
        }

        public int ResourceReferenceId { get; set; }
        public string LaunchResourceLink { get; set; }
        public string ResourceDescription { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
        public bool AbsentInLearningHub { get; set; }
    }
}
