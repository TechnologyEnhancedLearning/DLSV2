namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [Route("TrackingSystem/Centre/Administrators")]
    public class AdministratorController : Controller
    {
        private readonly ICommonService commonService;
        private readonly IUserDataService userDataService;

        public AdministratorController(
            IUserDataService userDataService,
            ICommonService commonService
        )
        {
            this.userDataService = userDataService;
            this.commonService = commonService;
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
                filterBy = NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(filterBy, filterValue);
            }

            var centreId = User.GetCentreId();
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var categories = commonService.GetCategoryListForCentre(centreId).Select(c => c.CategoryName);

            var model = new CentreAdministratorsViewModel(
                centreId,
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
