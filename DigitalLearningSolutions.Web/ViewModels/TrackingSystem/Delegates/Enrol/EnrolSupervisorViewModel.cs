using DigitalLearningSolutions.Data.Models.Supervisor;
using DigitalLearningSolutions.Web.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    public class EnrolSupervisorViewModel
    {
        public EnrolSupervisorViewModel() { }
        public EnrolSupervisorViewModel(
            int delegateId,
            string delegateName,
            bool isSelfAssessment,
            IEnumerable<SupervisorForEnrolDelegate> supervisorList,
            int  selectedSupervisor
        )
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            IsSelfAssessment = isSelfAssessment;
            SupervisorList = SupervisorList = PopulateItems(supervisorList, selectedSupervisor);
        }
        public EnrolSupervisorViewModel(
            int delegateId,
            string delegateName,
            bool isSelfAssessment,
            IEnumerable<SupervisorForEnrolDelegate> supervisorList,
            int selectedSupervisor,
            IEnumerable<SelfAssessmentSupervisorRole>? supervisorRoleList
        )
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            IsSelfAssessment = isSelfAssessment;
            SupervisorList = PopulateItems(supervisorList, selectedSupervisor);
            SupervisorRoleList = supervisorRoleList;
        }
        public int DelegateId { get; set; }
        public string? DelegateName { get; set; }

        public IEnumerable<SelectListItem> SupervisorList { get; set; }
        public IEnumerable<SelfAssessmentSupervisorRole>? SupervisorRoleList { get; set; }

        public int? SelectedSupervisor { get; set; }
        public bool IsSelfAssessment { get; set; }
        public int? SelectedSupervisorRoleId { get; set; }

        private static IEnumerable<SelectListItem> PopulateItems(
            IEnumerable<SupervisorForEnrolDelegate> supervisorList, int selected
        )
        {
            var LearningItemIdNames = supervisorList.Select(s => (s.AdminId, s.Name));
            return SelectListHelper.MapOptionsToSelectListItems(
                LearningItemIdNames, selected
           );
        }
    }
}
