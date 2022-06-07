namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using System.Collections.Generic;

    public class LinkViewModelWithFragment
    {
        public readonly string AspAction;

        public readonly Dictionary<string, string> AspAllRouteData;

        public readonly string AspController;

        public readonly string? Fragment;

        public readonly string LinkText;

        public LinkViewModelWithFragment(
            string aspController,
            string aspAction,
            string linkText,
            Dictionary<string, string> aspAllRouteData,
            string? fragment
        )
        {
            AspAction = aspAction;
            AspController = aspController;
            LinkText = linkText;
            AspAllRouteData = aspAllRouteData;
            Fragment = fragment;
        }
    }
}
