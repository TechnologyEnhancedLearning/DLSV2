namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse
{
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;

    public class SetCourseOptionsViewModel : EditCourseOptionsFormData
    {
        public SetCourseOptionsViewModel() { }

        public SetCourseOptionsViewModel(bool diagAssess)
        {
            if (diagAssess)
            {
                Checkboxes.Add(
                    new CheckboxListItemViewModel(
                        nameof(DiagnosticObjectiveSelection),
                        "Allow diagnostic objective selection",
                        "Allow the learner to choose which objectives to be assessed against when starting a diagnostic assessment."
                    )
                );
            }
        }
    }
}
