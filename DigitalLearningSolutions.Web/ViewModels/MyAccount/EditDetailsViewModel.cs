namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;

    public class EditDetailsViewModel : EditDetailsFormData
    {
        public EditDetailsViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            List<(int id, string name)> jobGroups,
            DlsSubApplication dlsSubApplication
        ) : base(adminUser, delegateUser, jobGroups)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public EditDetailsViewModel(EditDetailsFormData formData, DlsSubApplication dlsSubApplication) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
        }

        public DlsSubApplication DlsSubApplication { get; set; }
    }
}
