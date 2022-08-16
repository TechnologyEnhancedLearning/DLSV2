namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;

    public static class PossibleEmailUpdateHelper
    {
        public static List<PossibleEmailUpdate> GetUnverifiedPrimaryAndCentreEmails(
            string primaryEmail,
            string? centreSpecificEmail
        )
        {
            var primaryEmailModel = new PossibleEmailUpdate
            {
                OldEmail = null,
                NewEmail = primaryEmail,
                NewEmailIsVerified = false,
                IsDelegateEmailSetByAdmin = false,
            };

            var possibleEmailUpdates = new List<PossibleEmailUpdate> { primaryEmailModel };

            if (centreSpecificEmail != null)
            {
                var centreSpecificEmailModel = new PossibleEmailUpdate
                {
                    OldEmail = null,
                    NewEmail = centreSpecificEmail,
                    NewEmailIsVerified = false,
                    IsDelegateEmailSetByAdmin = false,
                };

                possibleEmailUpdates.Add(centreSpecificEmailModel);
            }

            return possibleEmailUpdates;
        }

        public static List<PossibleEmailUpdate> GetSingleUnverifiedEmail(
            string email
        )
        {
            var emailModel = new PossibleEmailUpdate
            {
                OldEmail = null,
                NewEmail = email,
                NewEmailIsVerified = false,
                IsDelegateEmailSetByAdmin = false,
            };

            return new List<PossibleEmailUpdate> { emailModel };
        }
    }
}
