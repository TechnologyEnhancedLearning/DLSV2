using DigitalLearningSolutions.Data.Models.Supervisor;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    public class EnrolSupervisorViewModel
    {
        public EnrolSupervisorViewModel() { }
        public EnrolSupervisorViewModel(
            int delegateId,
            string delegateName,
            bool isSelfAssessment,
            IEnumerable<SupervisorForEnrolDelegate> supervisorList
        )
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            IsSelfAssessment = isSelfAssessment;
            SupervisorList = supervisorList;
        }
        public EnrolSupervisorViewModel(
            int delegateId,
            string delegateName,
            bool isSelfAssessment,
            IEnumerable<SupervisorForEnrolDelegate> supervisorList,
            IEnumerable<SelfAssessmentSupervisorRole>? supervisorRoleList
        )
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            IsSelfAssessment = isSelfAssessment;
            SupervisorList = supervisorList;
            SupervisorRoleList = supervisorRoleList;
        }
        public int DelegateId { get; set; }
        public string? DelegateName { get; set; }
        public IEnumerable<SupervisorForEnrolDelegate> SupervisorList { get; set; }
        public IEnumerable<SelfAssessmentSupervisorRole>? SupervisorRoleList { get; set; }
        public int? SelectedSupervisor { get; set; }
        public bool IsSelfAssessment { get; set; }
        public int? SelectedSupervisorRoleId { get; set; }
    }
}
