namespace DigitalLearningSolutions.Data.Models.MultiPageFormData.AddNewCentreCourse
{
    public class CourseOptionsTempData
    {
        public CourseOptionsTempData(
            bool active,
            bool allowSelfEnrolment,
            bool diagnosticObjectiveSelection,
            bool hideInLearningPortal
        )
        {
            Active = active;
            AllowSelfEnrolment = allowSelfEnrolment;
            DiagnosticObjectiveSelection = diagnosticObjectiveSelection;
            HideInLearningPortal = hideInLearningPortal;
        }

        public bool Active { get; set; }
        public bool AllowSelfEnrolment { get; set; }
        public bool DiagnosticObjectiveSelection { get; set; }
        public bool HideInLearningPortal { get; set; }
    }
}
