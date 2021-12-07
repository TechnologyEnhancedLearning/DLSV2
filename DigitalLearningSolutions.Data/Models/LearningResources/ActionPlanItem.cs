﻿namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    using System;
    using DigitalLearningSolutions.Data.Models.LearningHubApiClient;

    public class ActionPlanItem : CurrentLearningItem
    {
        public ActionPlanItem() { }

        public ActionPlanItem(LearningLogItem learningLogItem, ResourceReferenceWithResourceDetails resource)
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
        }

        public DateTime? Completed { get; set; }
        public DateTime? RemovedDate { get; set; }
        public string ResourceDescription { get; set; }
        public string ResourceLink { get; set; }
        public string CatalogueName { get; set; }
        public string ResourceType { get; set; }
    }
}
