namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditDelegateViewModel : EditDelegateFormData
    {
        public EditDelegateViewModel(
            DelegateEntity delegateEntity,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditDelegateRegistrationPromptViewModel> editDelegateRegistrationPromptViewModels
        ) : base(delegateEntity, jobGroups)
        {
            DelegateId = delegateEntity.DelegateAccount.Id;
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(
                jobGroups,
                delegateEntity.UserAccount.JobGroupName
            );
            CustomFields = editDelegateRegistrationPromptViewModels;
        }

        public EditDelegateViewModel(
            EditDelegateFormData formData,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditDelegateRegistrationPromptViewModel> editDelegateRegistrationPromptViewModels,
            int delegateId
        ) : base(formData)
        {
            DelegateId = delegateId;
            var jobGroupName = jobGroups.Where(jg => jg.id == formData.JobGroupId).Select(jg => jg.name)
                .SingleOrDefault();
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, jobGroupName);
            CustomFields = editDelegateRegistrationPromptViewModels;
        }

        public int DelegateId { get; }

        public IEnumerable<SelectListItem> JobGroups { get; }

        public List<EditDelegateRegistrationPromptViewModel> CustomFields { get; }
    }
}
