namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class AdminRolesViewModel : AdminRolesFormData
    {
        private const int MaxNumberOfCmsRoleRadios = 3;
        private const int MinNumberOfCmsRoleRadios = 1;
        private const int MaxNumberOfRoleCheckboxes = 6;

        public readonly List<CheckboxListItemViewModel> Checkboxes = new List<CheckboxListItemViewModel>
        {
            AdminRoleInputs.CentreManagerCheckbox,
            AdminRoleInputs.CentreAdminCheckbox,
            AdminRoleInputs.SupervisorCheckbox,
            AdminRoleInputs.NominatedSupervisorCheckbox
        };

        public readonly List<RadiosListItemViewModel> Radios = new List<RadiosListItemViewModel>();

        protected AdminRolesViewModel() { }

        protected AdminRolesViewModel(User user, int centreId) : base(user)
        {
            CentreId = centreId;
        }

        public int CentreId { get; set; }
        public IEnumerable<SelectListItem> LearningCategories { get; set; }

        public bool NotAllRolesDisplayed =>
            Radios.Count < MaxNumberOfCmsRoleRadios || Checkboxes.Count < MaxNumberOfRoleCheckboxes;

        public bool NoContentManagerOptionsAvailable => Radios.Count == MinNumberOfCmsRoleRadios;
    }
}
