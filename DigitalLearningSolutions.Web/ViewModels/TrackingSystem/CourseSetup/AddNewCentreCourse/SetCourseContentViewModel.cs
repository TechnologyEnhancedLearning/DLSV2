namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class SetCourseContentViewModel
    {
        public SetCourseContentViewModel() { }

        public SetCourseContentViewModel(IEnumerable<Section> availableSections)
        {
            AvailableSections = availableSections;
            IncludeAllSections = true;
            SelectedSectionIds = null;
        }

        public SetCourseContentViewModel(
            IEnumerable<Section> availableSections,
            bool includeAllSections,
            IEnumerable<int>? selectedSectionIds
        )
        {
            AvailableSections = availableSections;
            IncludeAllSections = includeAllSections;
            SelectedSectionIds = selectedSectionIds;
        }

        public bool IncludeAllSections { get; set; }

        public IEnumerable<Section> AvailableSections { get; set; }

        [Required(ErrorMessage = "You must select at least one section")]
        public IEnumerable<int>? SelectedSectionIds { get; set; }

        public IEnumerable<Section> GetSelectedSections()
        {
            return AvailableSections.Where(section => SelectedSectionIds!.Contains(section.SectionId)).ToList();
        }

        public void SelectAllSections()
        {
            SelectedSectionIds = AvailableSections.Select(s => s.SectionId);
        }
    }
}
