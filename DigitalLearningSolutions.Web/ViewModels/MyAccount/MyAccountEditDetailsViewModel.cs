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
            List<EditCustomFieldViewModel> editCustomFieldViewModels,
            DlsSubApplication dlsSubApplication
        ) : base(adminUser, delegateUser, jobGroups)
        {
            DlsSubApplication = dlsSubApplication;
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(
                jobGroups,
                delegateUser?.JobGroupName
            );
            CustomFields = editCustomFieldViewModels;
        }

        public MyAccountEditDetailsViewModel(
            MyAccountEditDetailsFormData formData,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditCustomFieldViewModel> editCustomFieldViewModels,
            DlsSubApplication dlsSubApplication
        ) : base(formData)
        {
            DlsSubApplication = dlsSubApplication;
            var jobGroupName = jobGroups.Where(jg => jg.id == formData.JobGroupId).Select(jg => jg.name)
                .SingleOrDefault();
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, jobGroupName);
            CustomFields = editCustomFieldViewModels;
        }

        public DlsSubApplication DlsSubApplication { get; set; }

        public IEnumerable<SelectListItem> JobGroups { get; }

        public List<EditCustomFieldViewModel> CustomFields { get; }
    }
}
