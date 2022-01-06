namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    public class CompletedActionPlanResource : CompletedLearningItem
    {
        public CompletedActionPlanResource() { }

        public CompletedActionPlanResource(ActionPlanResource resource)
        {
            Name = resource.Name;
            Id = resource.Id;
            StartedDate = resource.StartedDate;
            Completed = resource.Completed!.Value;
            LastAccessed = resource.LastAccessed;
            ResourceDescription = resource.ResourceDescription;
            ResourceType = resource.ResourceType;
            CatalogueName = resource.CatalogueName;
            ResourceLink = resource.ResourceLink;
        }

        public string ResourceDescription { get; set; }
        public string ResourceLink { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
    }
}
