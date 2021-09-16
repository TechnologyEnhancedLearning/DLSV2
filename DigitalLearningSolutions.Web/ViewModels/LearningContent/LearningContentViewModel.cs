namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    public class LearningContentViewModel
    {
        public readonly string Brand;
        public readonly bool TitleIsLong;
        public readonly string Title;

        public LearningContentViewModel(string brand, string title, bool titleIsLong = false)
        {
            Brand = brand;
            Title = title;
            TitleIsLong = titleIsLong;
        }
    }
}
