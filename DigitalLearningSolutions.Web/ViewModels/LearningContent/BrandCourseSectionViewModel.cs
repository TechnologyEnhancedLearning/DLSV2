namespace DigitalLearningSolutions.Web.ViewModels.LearningContent
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class BrandCourseSectionViewModel
    {
        public BrandCourseSectionViewModel(Section section)
        {
            SectionName = section.SectionName;
            Tutorials = section.Tutorials.Select(t => new BrandCourseTutorialViewModel(t));
        }

        public IEnumerable<BrandCourseTutorialViewModel> Tutorials { get; set; }
        public string SectionName { get; set; }
    }
}
