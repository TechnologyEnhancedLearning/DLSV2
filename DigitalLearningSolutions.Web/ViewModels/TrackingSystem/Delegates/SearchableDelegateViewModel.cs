namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    public class SearchableDelegateViewModel
    {
        public SearchableDelegateViewModel(DelegateInfoViewModel delegateInfoViewModel)
        {
            DelegateInfo = delegateInfoViewModel;
        }

        public DelegateInfoViewModel DelegateInfo { get; set; }

        public string RegStatusTagName =>
            DelegateInfo.IsSelfReg
                ? "Self registered" + (DelegateInfo.IsExternalReg ? " (External)" : "")
                : "Registered by centre";

        public string ActiveTagName => DelegateInfo.IsActive ? "Active" : "Inactive";
        public string PasswordTagName => DelegateInfo.IsPasswordSet ? "Password set" : "Password not set";
    }
}
