namespace DigitalLearningSolutions.Data.Helpers
{
    public static class CentreEmailHelper
    {
        public static string GetEmailForCentreNotifications(
            string primaryEmail,
            string? centreEmail
        )
        {
            return centreEmail ?? primaryEmail;
        }
    }
}
