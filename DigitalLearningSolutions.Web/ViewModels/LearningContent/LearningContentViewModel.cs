namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using DigitalLearningSolutions.Data.Models.Common;

    public class LearningContentViewModel
    {
        public readonly string Name;
        public readonly string? Description;

        public LearningContentViewModel(BrandDetail brand)
        {
            Name = brand.BrandName;
            Description = brand.BrandDescription;
        }
    }
}
