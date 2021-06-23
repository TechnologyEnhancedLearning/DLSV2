namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    public class LearningContentViewModel
    {
        public readonly string Brand;
        public readonly string Title;

        public LearningContentViewModel(string brand, string title)
        {
            Brand = brand;
            Title = title;
        }
    }
}
