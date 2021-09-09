namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    public class LearningContentViewModel
    {
        public readonly string Brand;
        public readonly bool LongTitle;
        public readonly string Title;

        public LearningContentViewModel(string brand, string title, bool longTitle = false)
        {
            Brand = brand;
            Title = title;
            LongTitle = longTitle;
        }
    }
}
