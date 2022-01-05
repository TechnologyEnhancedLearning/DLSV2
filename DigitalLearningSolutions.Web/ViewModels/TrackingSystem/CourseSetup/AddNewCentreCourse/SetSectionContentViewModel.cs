namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;

    public class SetSectionContentViewModel
    {
        public SetSectionContentViewModel() { }

        public SetSectionContentViewModel(SelectSectionViewModel section, int index)
        {
            SectionName = section.Name;
            Index = index;
        }

        public string SectionName { get; set; }

        public int Index { get; set; }

        public IEnumerable<CourseTutorialViewModel> Tutorials { get; set; }
    }
}
