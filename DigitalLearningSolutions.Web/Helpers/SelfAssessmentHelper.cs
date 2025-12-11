namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

    public static class SelfAssessmentHelper
    {
        public static bool CheckRetirementDate(DateTime? date)
        {
            if (date == null)
                return false;

            DateTime retirementOffsetDate = DateTime.Today.AddDays(14);
            DateTime today = DateTime.Today;
            return (date >= today && date <= retirementOffsetDate);
        }
    }
}
