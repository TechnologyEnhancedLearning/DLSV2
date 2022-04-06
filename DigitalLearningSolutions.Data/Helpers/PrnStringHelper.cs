namespace DigitalLearningSolutions.Data.Helpers
{
    public static class PrnStringHelper
    {
        public static string GetPrnDisplayString(bool hasBeenPromptedForPrn, string? professionalRegistrationNumber)
        {
            return hasBeenPromptedForPrn
                ? professionalRegistrationNumber ?? "Not professionally registered"
                : "Not yet provided";
        }
    }
}
