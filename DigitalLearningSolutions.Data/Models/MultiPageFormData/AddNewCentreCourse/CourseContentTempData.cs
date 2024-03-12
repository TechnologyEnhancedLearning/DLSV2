﻿namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;

    public class CourseContentTempData
    {
        public CourseContentTempData() { }

        public CourseContentTempData(
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

        public IEnumerable<Section> AvailableSections { get; set; } = Enumerable.Empty<Section>();

        public IEnumerable<int>? SelectedSectionIds { get; set; }

        public IEnumerable<Section> GetSelectedSections()
        {
            return AvailableSections.Where(section => SelectedSectionIds!.Contains(section.SectionId)).ToList();
        }
    }
}
