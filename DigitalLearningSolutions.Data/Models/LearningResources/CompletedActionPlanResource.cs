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
            ResourceReferenceId = resource.ResourceReferenceId;
            AbsentInLearningHub = resource.AbsentInLearningHub;
        }

        public string ResourceDescription { get; set; } = string.Empty;
        public string ResourceLink { get; set; } = string.Empty;
        public string CatalogueName { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public int ResourceReferenceId { get; set; }
        public bool AbsentInLearningHub { get; set; }
    }
}
