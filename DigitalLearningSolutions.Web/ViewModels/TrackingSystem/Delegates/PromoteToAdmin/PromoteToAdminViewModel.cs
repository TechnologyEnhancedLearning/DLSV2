namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.PromoteToAdmin
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;

    public class PromoteToAdminViewModel : AdminRolesViewModel
    {
        public PromoteToAdminViewModel() { }

        public PromoteToAdminViewModel(
            DelegateUser user,
            int centreId,
            IEnumerable<Category> categories,
            CentreContractAdminUsage numberOfAdmins
        ) : base(user, centreId)
        {
            DelegateId = user.Id;

            IsCentreAdmin = false;
            IsSupervisor = false;
            IsNominatedSupervisor = false;
            IsTrainer = false;
            IsContentCreator = false;
            ContentManagementRole = ContentManagementRole.NoContentManagementRole;

            LearningCategory = 0;
            LearningCategories = SelectListHelper.MapOptionsToSelectListItems(
                categories.Select(c => (c.CourseCategoryID, c.CategoryName)),
                0
            );

            SetUpCheckboxesAndRadioButtons(numberOfAdmins);
        }

        public int DelegateId { get; set; }

        private void SetUpCheckboxesAndRadioButtons(CentreContractAdminUsage numberOfAdmins)
        {
            if (!numberOfAdmins.TrainersAtOrOverLimit)
            {
                Checkboxes.Add(AdminRoleInputs.TrainerCheckbox);
            }

            if (!numberOfAdmins.CcLicencesAtOrOverLimit)
            {
                Checkboxes.Add(AdminRoleInputs.ContentCreatorCheckbox);
            }

            if (!numberOfAdmins.CmsAdministratorsAtOrOverLimit)
            {
                Radios.Add(AdminRoleInputs.CmsAdministratorRadioButton);
            }

            if (!numberOfAdmins.CmsManagersAtOrOverLimit)
            {
                Radios.Add(AdminRoleInputs.CmsManagerRadioButton);
            }

            Radios.Add(AdminRoleInputs.NoCmsPermissionsRadioButton);
        }
    }
}
