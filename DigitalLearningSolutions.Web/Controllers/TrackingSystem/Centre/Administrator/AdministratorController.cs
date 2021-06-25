namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
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
            int page = 1,
            string? filterBy = null,
            string? filterValue = null
        )
        {
            if (filterValue != null)
            {
                filterBy += NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(filterBy, filterValue);
            }
            
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(User.GetCentreId());
            var categories = new List<string> {
                "Undefined",
                "Office 2007",
                "Office 2010",
                "Digital Workplace",
                "test",
                "Clinical Skills"
            };
            var model = new CentreAdministratorsViewModel(
                User.GetCentreId(),
                adminUsersAtCentre,
                categories,
                searchString,
                DefaultSortByOptions.Name.PropertyName,
                BaseSearchablePageViewModel.Ascending,
                filterBy,
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
