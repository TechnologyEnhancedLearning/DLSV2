namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class MyAccountEditDetailsViewModel : MyAccountEditDetailsFormData
    {
        public MyAccountEditDetailsViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            List<(int id, string name)> jobGroups,
            DlsSubApplication dlsSubApplication
        ) : base(adminUser, delegateUser, jobGroups)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public MyAccountEditDetailsViewModel(MyAccountEditDetailsFormData formData, DlsSubApplication dlsSubApplication) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
