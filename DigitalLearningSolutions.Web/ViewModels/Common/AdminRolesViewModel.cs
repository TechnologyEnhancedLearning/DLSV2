namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AdminRolesViewModel : AdminRolesFormData
    {
        public readonly List<RadiosListItemViewModel> Radios = new List<RadiosListItemViewModel>();

        public List<CheckboxListItemViewModel> Checkboxes = new List<CheckboxListItemViewModel>
        {
            AdminRoleInputs.CentreAdminCheckbox, AdminRoleInputs.SupervisorCheckbox
        };

        public AdminRolesViewModel(){}

        public AdminRolesViewModel(User user, int centreId) : base(user)
        {
            CentreId = centreId;
        }

        public int CentreId { get; set; }
        public IEnumerable<SelectListItem> LearningCategories { get; set; }

        public bool NotAllRolesDisplayed => Radios.Count < 3 || Checkboxes.Count < 4;
        public bool NoContentManagerOptionsAvailable => Radios.Count == 1;
    }
}
