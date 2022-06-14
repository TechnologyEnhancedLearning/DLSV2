namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;

    public class CourseContentData
    {
        public CourseContentData(){}

        public CourseContentData(
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
    }
}
