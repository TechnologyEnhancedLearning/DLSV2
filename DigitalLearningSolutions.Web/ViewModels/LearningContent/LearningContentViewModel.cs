namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using DigitalLearningSolutions.Data.Models.Courses;

    public class LearningContentViewModel
    {
        public readonly string Name;
        public readonly string? Description;
        public readonly IEnumerable<TutorialSummary> Tutorials;
        public readonly IEnumerable<ApplicationWithSections> Applications;

        public LearningContentViewModel(BrandDetail brand, IEnumerable<TutorialSummary> tutorials, IEnumerable<ApplicationWithSections> applications)
        {
            Name = brand.BrandName;
            Description = brand.BrandDescription;
            Tutorials = tutorials;
            Applications = applications;
        }
    }
}
