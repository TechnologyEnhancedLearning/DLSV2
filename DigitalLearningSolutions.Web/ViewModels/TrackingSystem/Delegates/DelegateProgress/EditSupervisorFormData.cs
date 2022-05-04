namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class EditSupervisorFormData
    {
        public EditSupervisorFormData() { }

        protected EditSupervisorFormData(DelegateCourseInfo info, ReturnPageQuery? returnPageQuery)
        {
            DelegateId = info.DelegateId;
            SupervisorId = info.SupervisorAdminId;
            CustomisationId = info.CustomisationId;
            CourseName = info.CourseName;
            DelegateName = info.DelegateFirstName == null
                ? info.DelegateLastName
                : $"{info.DelegateFirstName} {info.DelegateLastName}";
            ReturnPageQuery = returnPageQuery;
        }

        protected EditSupervisorFormData(EditSupervisorFormData formData)
        {
            SupervisorId = formData.SupervisorId;
            DelegateId = formData.DelegateId;
            CourseName = formData.CourseName;
            CustomisationId = formData.CustomisationId;
            DelegateName = formData.DelegateName;
            ReturnPageQuery = formData.ReturnPageQuery;
        }

        public int? SupervisorId { get; set; }
        public int DelegateId { get; set; }
        public string? DelegateName { get; set; }
        public int CustomisationId { get; set; }
        public string? CourseName { get; set; }
        public ReturnPageQuery? ReturnPageQuery { get; set; }
    }
}
