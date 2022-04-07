namespace DigitalLearningSolutions.Web.ViewModels.Home
{
    using DigitalLearningSolutions.Data.Models.Common;

    public class LearningContentSummary
    {
        public readonly int ID;
        public readonly string Brand;
        public readonly string? Description;
        public readonly byte[]? Image;

        public LearningContentSummary(BrandDetail brand)
        {
            this.ID = brand.BrandID;
            this.Brand = brand.BrandName;
            this.Description = brand.BrandDescription;
            this.Image = brand.BrandImage;
        }
    }
}
