namespace DigitalLearningSolutions.Data.Helpers
{
    using System;

    public static class CentreEmailHelper
    {
        public static string GetEmailForCentreNotifications(
            string primaryEmail,
            string? centreEmail,
            DateTime? centreEmailVerified
        )
        {
            if (centreEmailVerified != null)
            {
                return centreEmail ?? primaryEmail;
            }

            return primaryEmail;
        }
    }
}
