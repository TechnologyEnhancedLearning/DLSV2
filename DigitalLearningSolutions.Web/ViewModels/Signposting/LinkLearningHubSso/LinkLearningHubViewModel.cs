namespace DigitalLearningSolutions.Web.ViewModels.Signposting.LinkLearningHubSso
{
    public class LinkLearningHubViewModel
    {
        public LinkLearningHubViewModel(int? learningHubResourceId)
        {
            ResourceLinkId = learningHubResourceId;
            ShowResourceLink = learningHubResourceId.HasValue;
        }

        public int? ResourceLinkId { get; set; }

        public bool ShowResourceLink { get; set; }
    }
}
