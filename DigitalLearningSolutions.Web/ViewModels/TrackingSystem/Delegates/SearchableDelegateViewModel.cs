namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates
{
    /* TODO: Search and sort functionality is part of HEEDLS-491.
       Filename includes 'Searchable' to avoid having to change name later */

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
