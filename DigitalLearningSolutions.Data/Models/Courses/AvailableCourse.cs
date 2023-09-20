namespace DigitalLearningSolutions.Data.Models.Courses
{
    public class AvailableCourse : BaseLearningItem
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
        public int DelegateStatus { get; set; }
        public int HideInLearnerPortal { get; set; }

        private string? category;
        private string? topic;

        private static string? GetValidOrNull(string? toValidate)
        {
            return toValidate != null && toValidate.ToLower() == "undefined" ? null : toValidate;
        }
    }
}
