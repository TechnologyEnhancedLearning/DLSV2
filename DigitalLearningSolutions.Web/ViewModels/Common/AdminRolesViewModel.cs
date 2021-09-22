namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AdminRolesViewModel
    {
        public readonly List<RadiosListItemViewModel> Radios = new List<RadiosListItemViewModel>();

        public List<CheckboxListItemViewModel> Checkboxes = new List<CheckboxListItemViewModel>
        {
            AdminRoleInputs.CentreAdminCheckbox, AdminRoleInputs.SupervisorCheckbox
        };

        public AdminRolesViewModel(){}

        public AdminRolesViewModel(User user, int centreId)
        {
            CentreId = centreId;
            FullName = user.FullName;
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

        public AdminRoles GetAdminRoles()
        {
            return new AdminRoles(
                IsCentreAdmin,
                IsSupervisor,
                IsContentCreator,
                IsTrainer,
                ContentManagementRole.IsContentManager,
                ContentManagementRole.ImportOnly
            );
        }
    }
}
