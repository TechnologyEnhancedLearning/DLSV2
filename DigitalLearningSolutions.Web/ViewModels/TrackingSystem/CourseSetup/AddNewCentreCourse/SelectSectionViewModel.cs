namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using DigitalLearningSolutions.Data.Models;

    public class SelectSectionViewModel
    {
        public SelectSectionViewModel() { }

        public SelectSectionViewModel(
            Section section,
            bool isSectionSelected
        )
        {
            Id = section.SectionId;
            Name = section.SectionName;
            IsSectionSelected = isSectionSelected;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSectionSelected { get; set; }
    }
}
