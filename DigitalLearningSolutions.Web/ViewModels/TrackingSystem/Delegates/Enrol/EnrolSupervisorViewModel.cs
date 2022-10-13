using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Models.Supervisor;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.Enrol
{
    public class EnrolSupervisorViewModel
        //: Enumeration
    {
        //public EnrolSupervisorViewModel(int delegateId, string delegateName)
        //    : base(delegateId, delegateName) { }
        public EnrolSupervisorViewModel() { }
        public EnrolSupervisorViewModel(
            int delegateId,
            string delegateName,
            bool isSelfAssessment,
            IEnumerable<SupervisorForEnrolDelegate> supervisorList,
            int selectedSupervisor
        )
            //: base(delegateId, delegateName)
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            IsSelfAssessment = isSelfAssessment;
            SupervisorList = PopulateItems(supervisorList, selectedSupervisor);
        }
        public EnrolSupervisorViewModel(
            int delegateId,
            string delegateName,
            bool isSelfAssessment,
            IEnumerable<SupervisorForEnrolDelegate> supervisorList,
            int selectedSupervisor,
            IEnumerable<SelfAssessmentSupervisorRole>? supervisorRoleList
        )
            //: base(delegateId, delegateName)
        {
            DelegateId = delegateId;
            DelegateName = delegateName;
            IsSelfAssessment = isSelfAssessment;
            SupervisorList = PopulateItems(supervisorList, selectedSupervisor);
            SupervisorRoleList = supervisorRoleList;
            //SupervisorRoleList1 = PopulateRadioItems(supervisorRoleList);
        }
        public int DelegateId { get; set; }
        public string? DelegateName { get; set; }

        public IEnumerable<SelectListItem> SupervisorList { get; set; }
        public IEnumerable<SelfAssessmentSupervisorRole>? SupervisorRoleList { get; set; }
        public IEnumerable<RadiosListItemViewModel>? SupervisorRoleList1 { get; set; }

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

        //private static IEnumerable<RadiosListItemViewModel> PopulateRadioItems(
        //    IEnumerable<SelfAssessmentSupervisorRole> roleList
        //)
        //{
        //    List<RadiosListItemViewModel> radioItems = new List<RadiosListItemViewModel>();
        //    roleList.ToList().ForEach(role =>
        //    {
        //        RadiosListItemViewModel objRadiosListItemViewModel = new RadiosListItemViewModel();
        //        objRadiosListItemViewModel.HintText = role.RoleName;
        //        objRadiosListItemViewModel.Label = role.RoleName;
        //        objRadiosListItemViewModel.Enumeration = new EnrolSupervisorViewModel(role.ID, role.RoleName);
        //        radioItems.Add(objRadiosListItemViewModel);
        //    });

        //    return radioItems;
        //}
    }
}
