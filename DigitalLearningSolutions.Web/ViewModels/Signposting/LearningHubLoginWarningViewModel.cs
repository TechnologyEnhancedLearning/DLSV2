namespace DigitalLearningSolutions.Web.ViewModels.Signposting
{
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public class LearningHubLoginWarningViewModel
    {
        public LearningHubLoginWarningViewModel() { }

        public LearningHubLoginWarningViewModel(
            ResourceReferenceWithResourceDetails resource,
            bool learningHubAccountLinked,
            bool resourceSourcedFromFallbackData
        )
        {
            ResourceRefId = resource.RefId;
            ResourceTitle = resource.Title;
            LearningHubLoginWarningDismissed = false;
            LearningHubAccountLinked = learningHubAccountLinked;
            ResourceSourcedFromFallbackData = resourceSourcedFromFallbackData;
        }

        public int ResourceRefId { get; set; }
        public string ResourceTitle { get; set; }
        public bool LearningHubLoginWarningDismissed { get; set; }
        public bool LearningHubAccountLinked { get; set; }
        public bool ResourceSourcedFromFallbackData { get; set; }
    }
}
