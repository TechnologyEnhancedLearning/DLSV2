namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;

    public class SetCourseContentViewModel
    {
        public SetCourseContentViewModel() { }

        public SetCourseContentViewModel(IEnumerable<SelectSectionViewModel> availableSections)
        {
            AvailableSections = availableSections;
            IncludeAllSections = true;
            SectionsToInclude = new List<SelectSectionViewModel>();
        }

        public bool IncludeAllSections { get; set; }
        public IEnumerable<SelectSectionViewModel> AvailableSections { get; set; }
        public IEnumerable<SelectSectionViewModel> SectionsToInclude { get; set; }

        public void SetSectionsToInclude()
        {
            SectionsToInclude = IncludeAllSections
                ? AvailableSections
                : AvailableSections.Where(section => section.IsSectionSelected);
        }
    }
}
