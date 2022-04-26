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
            DelegateAccessRoute accessedVia,
            IEnumerable<AdminUser> supervisors,
            DelegateCourseInfo info
        ) : base(info)
        {
            ProgressId = progressId;
            AccessedVia = accessedVia;
            Supervisors = PopulateSupervisors(info.SupervisorAdminId, supervisors);
        }

        public EditSupervisorViewModel(
            EditSupervisorFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia,
            IEnumerable<AdminUser> supervisors
        ) : base(formData)
        {
            ProgressId = progressId;
            AccessedVia = accessedVia;
            Supervisors = PopulateSupervisors(formData.SupervisorId, supervisors);
        }

        public int ProgressId { get; set; }

        public DelegateAccessRoute AccessedVia { get; set; }

        public IEnumerable<SelectListItem> Supervisors { get; set; }

        private static IEnumerable<SelectListItem> PopulateSupervisors(
            int? supervisorId,
            IEnumerable<AdminUser> supervisors
        )
        {
            var supervisorIdNames = supervisors.Select(s => (s.Id, s.FullName));
            return SelectListHelper.MapOptionsToSelectListItems(
                supervisorIdNames,
                supervisorId
            );
        }
    }
}
