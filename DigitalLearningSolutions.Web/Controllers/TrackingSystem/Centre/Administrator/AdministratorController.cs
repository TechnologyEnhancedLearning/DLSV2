namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [Route("TrackingSystem/Centre/Administrators")]
    public class AdministratorController : Controller
    {
        private readonly IUserDataService userDataService;

        public AdministratorController(IUserDataService userDataService)
        {
            this.userDataService = userDataService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            int page = 1
        )
        {
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(User.GetCentreId());
            var model = new CentreAdministratorsViewModel(
                User.GetCentreId(),
                adminUsersAtCentre,
                searchString,
                DefaultSortByOptions.Name.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                page
            );

            return View(model);
        }

        [Route("AllAdmins")]
        public IActionResult AllAdmins()
        {
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(User.GetCentreId());
            var model = new AllAdminsViewModel(adminUsersAtCentre);
            return View("AllAdmins", model);
        }
    }
}
