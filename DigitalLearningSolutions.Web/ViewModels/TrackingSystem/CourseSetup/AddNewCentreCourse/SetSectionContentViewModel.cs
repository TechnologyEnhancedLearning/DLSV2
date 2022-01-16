namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;

    public class SetSectionContentViewModel : EditCourseSectionFormData
    {
        public SetSectionContentViewModel() { }

        public SetSectionContentViewModel(SelectSectionViewModel section, int index, bool showDiagnostic)
        {
            SectionName = section.Name;
            Index = index;
            ShowDiagnostic = showDiagnostic;
        }

        public SetSectionContentViewModel(
            EditCourseSectionFormData formData,
            int index
        ) : base(formData)
        {
            Index = index;
        }

        public SetSectionContentViewModel(
            SelectSectionViewModel section,
            int index,
            bool showDiagnostic,
            IEnumerable<Tutorial> tutorials
        )
        {
            SectionName = section.Name;
            Index = index;
            ShowDiagnostic = showDiagnostic;
            Tutorials = tutorials.Select(t => new CourseTutorialViewModel(t));
        }

        public int Index { get; set; }
    }
}
