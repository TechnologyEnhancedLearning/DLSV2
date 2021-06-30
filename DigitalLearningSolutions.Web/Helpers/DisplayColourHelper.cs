namespace DigitalLearningSolutions.Web.Helpers
{
    public static class DisplayColourHelper
    {
        // The colour strings here correspond to css classes defined in contractDetails.scss
        public static string GetDisplayColourForPercentage(long number, long limit)
        {
            if (limit == 0 && number == 0)
            {
                return "grey";
            }

            if (limit < 0)
            {
                return "blue";
            }

            var usage = (double)number / limit;

            if (0 <= usage && usage < 0.6)
            {
                return "green";
            }

            if (0.6 <= usage && usage < 1)
            {
                return "yellow";
            }

            if (usage >= 1)
            {
                return "red";
            }

            return "blue";
        }
    }
}
