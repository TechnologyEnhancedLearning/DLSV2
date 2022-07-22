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
            UserAccount userAccount,
            DelegateAccount? delegateAccount,
            int? centreId,
            List<(int id, string name)> jobGroups,
            string? centreSpecificEmail,
            List<EditDelegateRegistrationPromptViewModel> editDelegateRegistrationPromptViewModels,
            List<(int centreId, string centreName, string? centreSpecificEmail)> allCentreSpecificEmails,
            DlsSubApplication dlsSubApplication,
            string? returnUrl,
            bool isCheckDetailRedirect
        ) : base(
            userAccount,
            delegateAccount,
            jobGroups,
            centreSpecificEmail,
            allCentreSpecificEmails,
            returnUrl,
            isCheckDetailRedirect
        )
        {
            IsLoggedInToCentre = centreId != null;
            DlsSubApplication = dlsSubApplication;
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(
                jobGroups,
                userAccount.JobGroupName
            );
            DelegateRegistrationPrompts = editDelegateRegistrationPromptViewModels;
            AllCentreSpecificEmails = allCentreSpecificEmails;
        }

        public MyAccountEditDetailsViewModel(
            MyAccountEditDetailsFormData formData,
            int? centreId,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditDelegateRegistrationPromptViewModel> editDelegateRegistrationPromptViewModels,
            List<(int, string, string?)> allCentreSpecificEmails,
            DlsSubApplication dlsSubApplication
        ) : base(formData)
        {
            var jobGroupName = jobGroups.Where(jg => jg.id == formData.JobGroupId).Select(jg => jg.name)
                .SingleOrDefault();

            IsLoggedInToCentre = centreId != null;
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, jobGroupName);
            DelegateRegistrationPrompts = editDelegateRegistrationPromptViewModels;
            AllCentreSpecificEmails = allCentreSpecificEmails;
            DlsSubApplication = dlsSubApplication;
        }

        public bool IsLoggedInToCentre { get; set; }
        public DlsSubApplication DlsSubApplication { get; set; }

        public IEnumerable<SelectListItem> JobGroups { get; }

        public List<EditDelegateRegistrationPromptViewModel> DelegateRegistrationPrompts { get; }

        public new Dictionary<string, (string, string?)> AllCentreSpecificEmailsDictionary =>
            AllCentreSpecificEmails != null
                ? AllCentreSpecificEmails.ToDictionary(
                    row => row.centreId.ToString(),
                    row => (row.centreName, row.centreSpecificEmail)
                )
                : new Dictionary<string, (string, string?)>();
    }
}
