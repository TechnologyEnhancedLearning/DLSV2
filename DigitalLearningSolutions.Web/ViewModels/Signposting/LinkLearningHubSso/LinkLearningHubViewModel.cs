namespace DigitalLearningSolutions.Web.ViewModels.Signposting.LinkLearningHubSso
{
    public class LinkLearningHubViewModel
    {
        public LinkLearningHubViewModel(bool isAccountAlreadyLinked, int? learningHubResourceId)
        {
            ShowIsAlreadyLinkedWarning = isAccountAlreadyLinked;
            ResourceLinkId = learningHubResourceId;
            ShowResourceLink = learningHubResourceId.HasValue;
        }

        public bool ShowIsAlreadyLinkedWarning { get; set; }

        public int? ResourceLinkId { get; set; }

        public bool ShowResourceLink { get; set; }
    }
}
