namespace DigitalLearningSolutions.Web.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public static class PossibleEmailUpdateTestHelper
    {
        public static bool PossibleEmailUpdatesMatch(PossibleEmailUpdate update1, PossibleEmailUpdate update2)
        {
            return string.Equals(update1.OldEmail, update2.OldEmail) &&
                   string.Equals(update1.NewEmail, update2.NewEmail) &&
                   update1.NewEmailIsVerified == update2.NewEmailIsVerified;
        }

        public static bool PossibleEmailUpdateListsMatch(
            List<PossibleEmailUpdate> list1,
            List<PossibleEmailUpdate> list2
        )
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            return !list1.Where((t, i) => !PossibleEmailUpdatesMatch(t, list2[i])).Any();
        }
    }
}
