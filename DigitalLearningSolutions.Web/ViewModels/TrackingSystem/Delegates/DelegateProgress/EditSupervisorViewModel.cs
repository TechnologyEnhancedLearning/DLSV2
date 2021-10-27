namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditSupervisorViewModel : EditSupervisorFormData
    {
        public EditSupervisorViewModel(
            int progressId,
            DelegateProgressAccessRoute accessedVia,
            IEnumerable<AdminUser> supervisors,
            DelegateCourseInfo info
        ) : base(info, progressId)
        {
            AccessedVia = accessedVia;
            var supervisorIdNames = supervisors.Select(s => (s.Id, s.FullName));
            Supervisors = SelectListHelper.MapOptionsToSelectListItems(
                supervisorIdNames,
                info.SupervisorAdminId
            );
            CustomisationId = info.CustomisationId;
            CourseName = info.CourseName;
            DelegateName = info.DelegateFirstName == null
                ? info.DelegateLastName
                : $"{info.DelegateFirstName} {info.DelegateLastName}";
        }

        public int ProgressId { get; set; }

        public string DelegateName { get; set; }

        public int CustomisationId { get; set; }

        public string CourseName { get; set; }

        public DelegateProgressAccessRoute AccessedVia { get; set; }

        public IEnumerable<SelectListItem> Supervisors { get; set; }
    }
}
