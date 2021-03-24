namespace DigitalLearningSolutions.Data.Models.Frameworks
{
    public class BrandedFramework : BaseFramework
    {
        public string Brand { get; set; }
        public string? Category
        {
            get => category;

            set => category = GetValidOrNull(value);
        }
        public string? Topic
        {
            get => topic;

            set => topic = GetValidOrNull(value);
        }
        private string? category;
        private string? topic;

        private static string? GetValidOrNull(string? toValidate)
        {
            return toValidate != null && toValidate.ToLower() == "undefined" ? null : toValidate;
        }
    }
}
