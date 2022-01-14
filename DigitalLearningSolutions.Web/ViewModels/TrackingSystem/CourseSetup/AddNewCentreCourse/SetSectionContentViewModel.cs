namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
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

        public int Index { get; set; }
    }
}
