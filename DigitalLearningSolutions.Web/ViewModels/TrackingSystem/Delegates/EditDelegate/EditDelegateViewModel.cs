﻿namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EditDelegate
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
            DelegateUser delegateUser,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditCustomFieldViewModel> editCustomFieldViewModels
        ) : base(delegateUser, jobGroups)
        {
            DelegateId = delegateUser.Id;
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, delegateUser.JobGroupName);
            CustomFields = editCustomFieldViewModels;
        }

        public EditDelegateViewModel(
            EditDelegateFormData formData,
            IReadOnlyCollection<(int id, string name)> jobGroups,
            List<EditCustomFieldViewModel> editCustomFieldViewModels,
            int delegateId
        ) : base(formData)
        {
            DelegateId = delegateId;
            var jobGroupName = jobGroups.Where(jg => jg.id == formData.JobGroupId).Select(jg => jg.name)
                .SingleOrDefault();
            JobGroups = SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, jobGroupName);
            CustomFields = editCustomFieldViewModels;
        }

        public int DelegateId { get; }

        public IEnumerable<SelectListItem> JobGroups { get; }

        public List<EditCustomFieldViewModel> CustomFields { get; }
    }
}
