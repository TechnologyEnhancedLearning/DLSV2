namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditSupervisorFormData
    {
        public EditSupervisorFormData() { }

        public EditSupervisorFormData(DelegateCourseInfo info, int progressId)
        {
            DelegateId = info.DelegateId;
            ProgressId = progressId;
            SupervisorId = info.SupervisorAdminId;
        }

        public int? SupervisorId { get; set; }
        public int DelegateId { get; set; }
        public int ProgressId { get; set; }
    }
}
