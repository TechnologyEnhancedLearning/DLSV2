namespace DigitalLearningSolutions.Web.Helpers
{
    public static class VerifyYourEmailTextHelper
    {
        public static string UnverifiedCentreEmailConsequences =
            "You will not be able to access that account until you verify the address.";

        public static string UnverifiedPrimaryEmailConsequences =>
            "You can edit your account details, but you cannot access any centre accounts until it is verified.";

        public static string VerifyEmailLinkCommonInfo(bool multipleEmailsAreUnverified)
        {
            var numSpecificPhrases = new
            {
                address = multipleEmailsAreUnverified ? "addresses" : "address",
                thisAddress = multipleEmailsAreUnverified ? "each of the addresses above" : "this address",
                link = multipleEmailsAreUnverified ? "links" : "link",
            };

            return $"An email with a verification link has been sent to {numSpecificPhrases.thisAddress}." +
                   $" Please click the {numSpecificPhrases.link} to verify your email {numSpecificPhrases.address}.";
        }

        public static string DirectionsToResendLinkByVisitingMyAccountPage(bool multipleEmailsAreUnverified)
        {
            var numSpecificPhrases = new
            {
                folder = multipleEmailsAreUnverified ? "folders" : "folder",
                it = multipleEmailsAreUnverified ? "them" : "it",
                email = multipleEmailsAreUnverified ? "emails" : "email",
            };

            return
                $" If you have not received the {numSpecificPhrases.email} after a few minutes, check your Junk {numSpecificPhrases.folder}," +
                $" or you can resend {numSpecificPhrases.it} by visiting the My account page.";
        }

        public static string DirectionsToResendLinkByClickingButtonBelow(bool multipleEmailsAreUnverified)
        {
            var numSpecificPhrases = new
            {
                folder = multipleEmailsAreUnverified ? "folders" : "folder",
                it = multipleEmailsAreUnverified ? "them" : "it",
                email = multipleEmailsAreUnverified ? "emails" : "email",
            };

            return $"Check your Junk {numSpecificPhrases.folder} if you can’t find {numSpecificPhrases.it}," +
                   $" or you can resend {numSpecificPhrases.it} by clicking the button below.";
        }
    }
}
