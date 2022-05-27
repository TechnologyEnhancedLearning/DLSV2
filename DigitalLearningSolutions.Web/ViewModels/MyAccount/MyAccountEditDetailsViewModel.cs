namespace DigitalLearningSolutions.Web.ViewModels.MyAccount
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class MyAccountEditDetailsViewModel : MyAccountEditDetailsFormData
    {
        public MyAccountEditDetailsViewModel(
            AdminUser? adminUser,
            DelegateUser? delegateUser,
            List<(int id, string name)> jobGroups,
            string? centreEmail,
            List<EditDelegateRegistrationPromptViewModel> editDelegateRegistrationPromptViewModels,
            DlsSubApplication dlsSubApplication,
            string? returnUrl,
            bool isCheckDetailRedirect
        ) : base(adminUser, delegateUser, jobGroups, centreEmail, returnUrl, isCheckDetailRedirect)
        {
            DlsSubApplication = dlsSubApplication;
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(
                jobGroups,
                delegateUser?.JobGroupName
            );
            DelegateRegistrationPrompts = editDelegateRegistrationPromptViewModels;
        }

        public MyAccountEditDetailsViewModel(
            MyAccountEditDetailsFormData formData,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditDelegateRegistrationPromptViewModel> editDelegateRegistrationPromptViewModels,
            DlsSubApplication dlsSubApplication
        ) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
            var jobGroupName = jobGroups.Where(jg => jg.id == formData.JobGroupId).Select(jg => jg.name)
                .SingleOrDefault();
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, jobGroupName);
            DelegateRegistrationPrompts = editDelegateRegistrationPromptViewModels;
        }

        public DlsSubApplication DlsSubApplication { get; set; }

        public IEnumerable<SelectListItem> JobGroups { get; }

        public List<EditDelegateRegistrationPromptViewModel> DelegateRegistrationPrompts { get; }
    }
}
