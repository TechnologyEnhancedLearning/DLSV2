namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditRolesViewModel
    {
        public List<CheckboxListItemViewModel> Checkboxes = new List<CheckboxListItemViewModel>
        {
            AdminRoleInputs.CentreAdminCheckbox, AdminRoleInputs.SupervisorCheckbox
        };

        public readonly List<RadiosListItemViewModel> Radios = new List<RadiosListItemViewModel>();

        public EditRolesViewModel() { }

        public EditRolesViewModel(AdminUser user, int centreId, IEnumerable<Category> categories, NumberOfAdministrators numberOfAdmins)
        {
            CentreId = centreId;
            FullName = $"{user.FirstName} {user.LastName}";

            IsCentreAdmin = user.IsCentreAdmin;
            IsSupervisor = user.IsSupervisor;
            IsTrainer = user.IsTrainer;
            IsContentCreator = user.IsContentCreator;

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

            LearningCategory = user.CategoryId;
            LearningCategories = SelectListHelper.MapOptionsToSelectListItems(
                categories.Select(c => (c.CourseCategoryID, c.CategoryName)),
                user.CategoryId
            );

            SetUpCheckboxesAndRadioButtons(user, numberOfAdmins);
        }

        public int CentreId { get; set; }
        public string? FullName { get; set; }
        public bool IsCentreAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsTrainer { get; set; }
        public bool IsContentCreator { get; set; }
        public ContentManagementRole ContentManagementRole { get; set; }
        public int LearningCategory { get; set; }
        public IEnumerable<SelectListItem> LearningCategories { get; set; }

        public bool NotAllRolesDisplayed => Radios.Count < 3 || Checkboxes.Count < 4;
        public bool NoContentManagerOptionsAvailable => Radios.Count == 1;

        private void SetUpCheckboxesAndRadioButtons(AdminUser user, NumberOfAdministrators numberOfAdmins)
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
