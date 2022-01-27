namespace DigitalLearningSolutions.Web.Models.Enums
{
    using DigitalLearningSolutions.Data.Enums;

    public class FallbackDataUsedWarningText : Enumeration
    {
        public static readonly FallbackDataUsedWarningText ResourceName =
            new FallbackDataUsedWarningText(
                0,
                "The resource name may be out of date because we are currently unable to retrieve information from the Learning Hub."
            );

        public static readonly FallbackDataUsedWarningText ResourceDetails =
            new FallbackDataUsedWarningText(
                1,
                "Some resource details may be out of date because we are currently unable to retrieve information from the Learning Hub."
            );

        private FallbackDataUsedWarningText(int id, string text) : base(id, text) { }
    }
}
