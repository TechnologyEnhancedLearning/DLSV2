namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class SetCourseContentViewModel
    {
        public SetCourseContentViewModel() { }

        public SetCourseContentViewModel(IEnumerable<SelectSectionViewModel> availableSections)
        {
            AvailableSections = availableSections;
            IncludeAllSections = true;
            SelectedSectionIds = null;
        }

        public SetCourseContentViewModel(
            IEnumerable<SelectSectionViewModel> availableSections,
            bool includeAllSections,
            IEnumerable<int>? selectedSectionIds
        )
        {
            AvailableSections = availableSections;
            IncludeAllSections = includeAllSections;
            SelectedSectionIds = selectedSectionIds;
        }

        public bool IncludeAllSections { get; set; }

        public IEnumerable<SelectSectionViewModel> AvailableSections { get; set; }

        [Required(ErrorMessage = "You must select at least one section")]
        public IEnumerable<int>? SelectedSectionIds { get; set; }

        public IEnumerable<SelectSectionViewModel> GetSelectedSections()
        {
            return AvailableSections.Where(section => SelectedSectionIds!.Contains(section.Id)).ToList();
        }
    }
}
