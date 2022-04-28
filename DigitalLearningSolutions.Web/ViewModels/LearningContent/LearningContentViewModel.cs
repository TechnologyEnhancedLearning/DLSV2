namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    public class LearningContentViewModel
    {
        public readonly string Name;
        public readonly string? Description;
        public readonly IEnumerable<TutorialSummary> Tutorials;

        public LearningContentViewModel(BrandDetail brand, IEnumerable<TutorialSummary> tutorials)
        {
            Name = brand.BrandName;
            Description = brand.BrandDescription;
            Tutorials = tutorials;
        }
    }
}
