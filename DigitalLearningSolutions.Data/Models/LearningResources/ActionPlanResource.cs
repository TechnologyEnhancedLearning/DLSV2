namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using System;
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public class ActionPlanResource : CurrentLearningItem
    {
        public ActionPlanResource() { }

        public ActionPlanResource(LearningLogItem learningLogItem, ResourceReferenceWithResourceDetails resource)
        {
            Name = resource.Title;
            Id = learningLogItem.LearningLogItemId;
            StartedDate = learningLogItem.LoggedDate;
            CompleteByDate = learningLogItem.DueDate;
            Completed = learningLogItem.CompletedDate;
            RemovedDate = learningLogItem.ArchivedDate;
            LastAccessed = learningLogItem.LastAccessedDate;
            ResourceDescription = resource.Description;
            ResourceType = resource.ResourceType;
            CatalogueName = resource.Catalogue.Name;
            ResourceLink = resource.Link;
            ResourceReferenceId = resource.RefId;
            AbsentInLearningHub = resource.AbsentInLearningHub;
        }

        public DateTime? Completed { get; set; }
        public DateTime? RemovedDate { get; set; }
        public string ResourceDescription { get; set; } = string.Empty;
        public string ResourceLink { get; set; } = string.Empty;
        public string CatalogueName { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public int ResourceReferenceId { get; set; }
        public bool AbsentInLearningHub { get; set; }
    }
}
