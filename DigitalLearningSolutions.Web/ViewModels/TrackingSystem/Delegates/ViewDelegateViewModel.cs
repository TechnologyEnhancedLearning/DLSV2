namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;

    public class ViewDelegateViewModel
    {
        public ViewDelegateViewModel(
            DelegateInfoViewModel delegateInfoViewModel,
            IEnumerable<DelegateCourseInfoViewModel> courseInfoViewModelList
        )
        {
            DelegateInfo = delegateInfoViewModel;
            CourseInfoList = courseInfoViewModelList.ToList();
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }

        public List<DelegateCourseInfoViewModel> CourseInfoList { get; set; }

        public string RegStatusTagName =>
            DelegateInfo.IsSelfReg
                ? "Self registered" + (DelegateInfo.IsExternalReg ? " (External)" : "")
                : "Registered by centre";

        public string ActiveTagName => DelegateInfo.IsActive ? "Active" : "Inactive";
        public string PasswordTagName => DelegateInfo.IsPasswordSet ? "Password set" : "Password not set";
    }
}
