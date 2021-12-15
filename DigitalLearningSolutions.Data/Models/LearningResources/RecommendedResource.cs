namespace DigitalLearningSolutions.Data.Models.LearningResources
{
    public class RecommendedResource
    {
        public int LearningResourceReferenceId { get; set; }

        public int LearningHubReferenceId { get; set; }

        public string ResourceName { get; set; }

        public string ResourceDescription { get; set; }

        public string ResourceType { get; set; }

        public string CatalogueName { get; set; }

        public string ResourceLink { get; set; }

        public bool IsInActionPlan { get; set; }

        public bool IsCompleted { get; set; }

        public int? LearningLogId { get; set; }

        // TODO HEEDLS-705 Actually populate and use this
        public decimal RecommendationScore { get; set; }
    }
}
