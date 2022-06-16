namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;

    public class CourseContentData
    {
        public CourseContentData() { }

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

        public IEnumerable<int>? SelectedSectionIds { get; set; }

        public IEnumerable<Section> GetSelectedSections()
        {
            return AvailableSections.Where(section => SelectedSectionIds!.Contains(section.SectionId)).ToList();
        }
    }
}
