namespace DigitalLearningSolutions.Web.Helpers
{
    public static class LearningHubWarningTextHelper
    {
        public static string ResourceNameMayBeOutOfDate =>
            "The resource name may be out of date because we are currently unable to retrieve information from the Learning Hub.";

        public static string ResourceDetailsMayBeOutOfDate =>
            "Some resource details may be out of date because we are currently unable to retrieve information from the Learning Hub.";

        public static string ResourceHasBeenRemoved =>
            "This resource has been removed from the Learning Hub so its details may be out of date.";

        public static string LinkAccount =>
            "The first time you view a resource, you will be taken to the Learning Hub to link your account, " +
            "or create and link your account if you don't already have one. You will only have to do this once. " +
            "Clicking \"Launch resource\" will start the linking process in a new tab.";

        public static string AutomaticLogin =>
            "Please note that you will only be logged into Learning Hub automatically if you navigate to resources via " +
            "Digital Learning Solutions.";
    }
}
