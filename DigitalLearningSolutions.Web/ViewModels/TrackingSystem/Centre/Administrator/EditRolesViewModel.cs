namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;

    public class EditRolesViewModel : AdminRolesViewModel
    {
        public EditRolesViewModel() { }

        public EditRolesViewModel(
            AdminUser user,
            int centreId,
            IEnumerable<Category> categories,
            CentreContractAdminUsage numberOfAdmins,
            ReturnPageQuery returnPageQuery
        ) : base(user, centreId)
        {
            IsCentreAdmin = user.IsCentreAdmin;
            IsSupervisor = user.IsSupervisor;
            IsNominatedSupervisor = user.IsNominatedSupervisor;
            IsTrainer = user.IsTrainer;
            IsContentCreator = user.IsContentCreator;
            ReturnPageQuery = returnPageQuery;

            if (user.IsCmsAdministrator)
            {
                ContentManagementRole = ContentManagementRole.CmsAdministrator;
            }
            else if (user.IsCmsManager)
            {
                ContentManagementRole = ContentManagementRole.CmsManager;
            }
            else
            {
                ContentManagementRole = ContentManagementRole.NoContentManagementRole;
            }

            LearningCategory = user.CategoryId ?? 0;
            LearningCategories = SelectListHelper.MapOptionsToSelectListItems(
                categories.Select(c => (c.CourseCategoryID, c.CategoryName)),
                user.CategoryId
            );

            SetUpCheckboxesAndRadioButtons(user, numberOfAdmins);
        }

        private void SetUpCheckboxesAndRadioButtons(AdminUser user, CentreContractAdminUsage numberOfAdmins)
        {
            if (!numberOfAdmins.TrainersAtOrOverLimit || user.IsTrainer)
            {
                Checkboxes.Add(AdminRoleInputs.TrainerCheckbox);
            }

            if (!numberOfAdmins.CcLicencesAtOrOverLimit || user.IsContentCreator)
            {
                Checkboxes.Add(AdminRoleInputs.ContentCreatorCheckbox);
            }

            if (!numberOfAdmins.CmsAdministratorsAtOrOverLimit || user.IsCmsAdministrator)
            {
                Radios.Add(AdminRoleInputs.CmsAdministratorRadioButton);
            }

            if (!numberOfAdmins.CmsManagersAtOrOverLimit || user.IsCmsManager)
            {
                Radios.Add(AdminRoleInputs.CmsManagerRadioButton);
            }

            Radios.Add(AdminRoleInputs.NoCmsPermissionsRadioButton);
        }
    }
}
