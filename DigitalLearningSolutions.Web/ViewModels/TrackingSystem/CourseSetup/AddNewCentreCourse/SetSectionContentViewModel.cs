namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;

    public class SetSectionContentViewModel
    {
        public SetSectionContentViewModel() { }

        public SetSectionContentViewModel(IEnumerable<SelectSectionViewModel> sections)
        {
            Sections = GetSectionModels(sections);
        }

        public IEnumerable<SectionContentViewModel> Sections { get; set; }

        public IEnumerable<CourseTutorialViewModel> GetTutorialsFromSections(
        )
        {
            var tutorials = new List<CourseTutorialViewModel>();
            foreach (var section in Sections)
            {
                tutorials.AddRange(section.Tutorials);
            }

            return tutorials;
        }

        private static IEnumerable<SectionContentViewModel> GetSectionModels(
            IEnumerable<SelectSectionViewModel> sections
        )
        {
            return sections.Select(section => new SectionContentViewModel(section, false)).ToList();
        }
    }
}
