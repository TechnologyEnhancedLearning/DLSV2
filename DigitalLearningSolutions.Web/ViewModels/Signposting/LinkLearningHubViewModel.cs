namespace DigitalLearningSolutions.Web.ViewModels.Signposting
{
    public class LinkLearningHubViewModel
    {
        public LinkLearningHubViewModel(bool isAccountAlreadyLinked, int? learningHubResourceId)
        {
            ShowIsAlreadyLinkedWarning = isAccountAlreadyLinked;
            ResourceLinkId = learningHubResourceId;
        }

        public bool ShowIsAlreadyLinkedWarning { get; set; }

        public int? ResourceLinkId { get; set; }

        public bool ShowResourceLink => ResourceLinkId.HasValue;
    }
}
