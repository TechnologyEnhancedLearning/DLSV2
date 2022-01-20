namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models.Courses;

    public class EditSupervisorFormData
    {
        public EditSupervisorFormData() { }

        protected EditSupervisorFormData(DelegateCourseInfo info, int? returnPage)
        {
            DelegateId = info.DelegateId;
            SupervisorId = info.SupervisorAdminId;
            CustomisationId = info.CustomisationId;
            CourseName = info.CourseName;
            DelegateName = info.DelegateFirstName == null
                ? info.DelegateLastName
                : $"{info.DelegateFirstName} {info.DelegateLastName}";
            ReturnPage = returnPage;
        }

        protected EditSupervisorFormData(EditSupervisorFormData formData)
        {
            SupervisorId = formData.SupervisorId;
            DelegateId = formData.DelegateId;
            CourseName = formData.CourseName;
            CustomisationId = formData.CustomisationId;
            DelegateName = formData.DelegateName;
            ReturnPage = formData.ReturnPage;
        }

        public int? SupervisorId { get; set; }
        public int DelegateId { get; set; }
        public string? DelegateName { get; set; }
        public int CustomisationId { get; set; }
        public string? CourseName { get; set; }
        public int? ReturnPage { get; set; }
    }
}
