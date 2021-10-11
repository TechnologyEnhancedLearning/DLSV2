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
            ApplicationType application
        ) : base(adminUser, delegateUser, jobGroups)
        {
            Application = application;
        }

        public EditDetailsViewModel(EditDetailsFormData formData, ApplicationType application) : base(formData)
        {
            Application = application;
        }

        public ApplicationType Application { get; set; }
    }
}
