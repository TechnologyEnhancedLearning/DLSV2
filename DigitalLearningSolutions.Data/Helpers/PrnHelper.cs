namespace DigitalLearningSolutions.Data.Helpers
{
    public static class PrnHelper
    {
        public static string GetPrnDisplayString(bool hasBeenPromptedForPrn, string? professionalRegistrationNumber)
        {
            return hasBeenPromptedForPrn
                ? professionalRegistrationNumber ?? "Not professionally registered"
                : "Not yet provided";
        }

        public static bool? GetHasPrnForDelegate(bool hasBeenPromptedForPrn, string? professionalRegistrationNumber)
        {
            return hasBeenPromptedForPrn ? (bool?)(professionalRegistrationNumber != null) : null;
        }
    }
}
