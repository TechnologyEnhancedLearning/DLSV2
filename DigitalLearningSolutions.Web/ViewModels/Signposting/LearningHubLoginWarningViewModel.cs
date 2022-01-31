namespace DigitalLearningSolutions.Web.ViewModels.Signposting
{
    using DigitalLearningSolutions.Data.Models.External.LearningHubApiClient;

    public class LearningHubLoginWarningViewModel
    {
        public LearningHubLoginWarningViewModel() { }

        public LearningHubLoginWarningViewModel(
            ResourceReferenceWithResourceDetails resource,
            bool learningHubAccountLinked,
            bool apiIsAccessible
        )
        {
            ResourceRefId = resource.RefId;
            ResourceTitle = resource.Title;
            LearningHubLoginWarningDismissed = false;
            LearningHubAccountLinked = learningHubAccountLinked;
            ApiIsAccessible = apiIsAccessible;
        }

        public int ResourceRefId { get; set; }
        public string ResourceTitle { get; set; }
        public bool LearningHubLoginWarningDismissed { get; set; }
        public bool LearningHubAccountLinked { get; set; }
        public bool ApiIsAccessible { get; set; }
    }
}
