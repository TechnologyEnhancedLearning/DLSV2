namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators
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
    using NHSUKViewComponents.Web.ViewModels;

    public class ManageRoleViewModel : AdminRolesViewModel
    {
        public ManageRoleViewModel() { }

        public ManageRoleViewModel(
            AdminUser user,
            int centreId,
            IEnumerable<Category> categories,
            CentreContractAdminUsage numberOfAdmins
        ) : base(user, centreId)
        {
            IsCentreAdmin = user.IsCentreAdmin;
            IsSupervisor = user.IsSupervisor;
            IsCenterManager = user.IsCentreManager;
            IsNominatedSupervisor = user.IsNominatedSupervisor;
            IsTrainer = user.IsTrainer;
            IsContentCreator = user.IsContentCreator;
            IsSuperAdmin = user.IsSuperAdmin;
            IsReportViewer = user.IsReportsViewer;
            IsLocalWorkforceManager = user.IsLocalWorkforceManager;
            IsFrameworkDeveloper = user.IsFrameworkDeveloper;
            IsWorkforceManager = user.IsWorkforceManager;

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

            LearningCategory = AdminCategoryHelper.CategoryIdToAdminCategory(user.CategoryId);
            LearningCategories = SelectListHelper.MapOptionsToSelectListItems(
                categories.Select(c => (c.CourseCategoryID, c.CategoryName)),
                user.CategoryId
            );

            SetUpCheckboxesAndRadioButtons(user, numberOfAdmins);
        }
        public string CentreName { get; set; }
        public List<CheckboxListItemViewModel> SpecialPermissions { get; set; }
        public string? SearchString { get; set; }
        public string? ExistingFilterString { get; set; }
        public int Page { get; set; }
        private void SetUpCheckboxesAndRadioButtons(AdminUser user, CentreContractAdminUsage numberOfAdmins)
        {
            SpecialPermissions= new List<CheckboxListItemViewModel>();
            if (!numberOfAdmins.TrainersAtOrOverLimit || user.IsTrainer)
            {
                Checkboxes.Add(AdminRoleInputs.TrainerCheckbox);
            }

            if (!numberOfAdmins.CcLicencesAtOrOverLimit || user.IsContentCreator)
            {
                Checkboxes.Add(AdminRoleInputs.ContentCreatorCheckbox);
            }
            Checkboxes.Add(AdminRoleInputs.LocalWorkforceManagerCheckbox);

            SpecialPermissions.Add(AdminRoleInputs.SuperAdministratorCheckbox);
            SpecialPermissions.Add(AdminRoleInputs.ReportViewerCheckbox);
            SpecialPermissions.Add(AdminRoleInputs.FrameworkDeveloperCheckbox);
            SpecialPermissions.Add(AdminRoleInputs.WorkforceManagerCheckbox);

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
