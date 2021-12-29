namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;

    public class SetSectionContentViewModel
    {
        public SetSectionContentViewModel() { }

        public SetSectionContentViewModel(IEnumerable<SelectSectionViewModel> sections)
        {
            Sections = GetSectionModels(sections);
        }

        public IEnumerable<SectionContentViewModel> Sections { get; set; }

        private static IEnumerable<SectionContentViewModel> GetSectionModels(
            IEnumerable<SelectSectionViewModel> sections
        )
        {
            return sections.Select(section => new SectionContentViewModel(section, false)).ToList();
        }
    }
}
