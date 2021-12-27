namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;

    public class SetCourseContentViewModel
    {
        public SetCourseContentViewModel() { }

        public SetCourseContentViewModel(IEnumerable<SelectSectionViewModel> availableSections)
        {
            AvailableSections = availableSections;
            IncludeAllSections = true;
        }

        public SetCourseContentViewModel(bool includeAllSections, IEnumerable<SelectSectionViewModel> sectionsToInclude)
        {
            IncludeAllSections = includeAllSections;
            SectionsToInclude = sectionsToInclude;
        }

        public bool IncludeAllSections { get; set; }
        public IEnumerable<SelectSectionViewModel> AvailableSections { get; set; }
        public IEnumerable<SelectSectionViewModel> SectionsToInclude { get; set; }

        public void SetAvailableSections(IEnumerable<SelectSectionViewModel> sections)
        {
            AvailableSections = sections;
        }

        public void SetSections()
        {
            SectionsToInclude = AvailableSections.Where(section => section.IsSectionSelected);
        }
    }
}
