namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Centre))]
    [Route("TrackingSystem/Centre/Administrators")]
    public class AdministratorController : Controller
    {
        private const string AdminFilterCookieName = "AdminFilter";
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public AdministratorController(
            IUserDataService userDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            IUserService userService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.userDataService = userDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.userService = userService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1,
            int? itemsPerPage = null
        )
        {
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                AdminFilterCookieName
            );

            var centreId = User.GetCentreIdKnownNotNull();
            var adminsAtCentre = userDataService.GetAdminsByCentreId(centreId);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            var loggedInAdmin = userDataService.GetAdminById(User.GetAdminId()!.Value);

            var availableFilters =
                AdministratorsViewModelFilterOptions.GetAllAdministratorsFilterModels(categories);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                new FilterOptions(existingFilterString, availableFilters),
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                adminsAtCentre,
                searchSortPaginationOptions
            );

            var model = new CentreAdministratorsViewModel(
                centreId,
                result,
                availableFilters,
                loggedInAdmin!.AdminAccount
            );

            Response.UpdateFilterCookie(AdminFilterCookieName, result.FilterString);

            return View(model);
        }

        [Route("AllAdmins")]
        public IActionResult AllAdmins()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var loggedInAdmin = userDataService.GetAdminById(User.GetAdminId()!.Value);

            var adminsAtCentre = userDataService.GetAdminsByCentreId(centreId);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            var model = new AllAdminsViewModel(
                adminsAtCentre,
                categories,
                loggedInAdmin!.AdminAccount
            );
            return View("AllAdmins", model);
        }

        [Route("{adminId:int}/EditAdminRoles")]
        [HttpGet]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult EditAdminRoles(int adminId, ReturnPageQuery returnPageQuery)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var adminUser = userDataService.GetAdminUserById(adminId);

            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });
            var numberOfAdmins = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

            var model = new EditRolesViewModel(adminUser!, centreId, categories, numberOfAdmins, returnPageQuery);
            return View(model);
        }

        [Route("{adminId:int}/EditAdminRoles")]
        [HttpPost]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult EditAdminRoles(AdminRolesFormData model, int adminId)
        {
            userService.UpdateAdminUserPermissions(
                adminId,
                model.GetAdminRoles(),
                AdminCategoryHelper.AdminCategoryToCategoryId(model.LearningCategory)
            );

            return RedirectToAction(
                "Index",
                "Administrator",
                model.ReturnPageQuery.ToRouteDataDictionary(),
                model.ReturnPageQuery.ItemIdToReturnTo
            );
        }

        [Route("{adminId:int}/UnlockAccount")]
        [HttpPost]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult UnlockAccount(int adminId)
        {
            userService.ResetFailedLoginCountByUserId(userDataService.GetUserIdByAdminId(adminId)!.Value);

            return RedirectToAction("Index");
        }

        [Route("{adminId:int}/DeactivateAdmin")]
        [HttpGet]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult DeactivateOrDeleteAdmin(int adminId, ReturnPageQuery returnPageQuery)
        {
            var admin = userDataService.GetAdminById(adminId);

            if (!CurrentUserCanDeactivateAdmin(admin!.AdminAccount))
            {
                return NotFound();
            }

            var model = new DeactivateAdminViewModel(admin, returnPageQuery);
            return View(model);
        }

        [Route("{adminId:int}/DeactivateAdmin")]
        [HttpPost]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult DeactivateOrDeleteAdmin(int adminId, DeactivateAdminViewModel model)
        {
            var admin = userDataService.GetAdminById(adminId);

            if (!CurrentUserCanDeactivateAdmin(admin!.AdminAccount))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            userService.DeactivateOrDeleteAdmin(adminId);

            return View("DeactivateOrDeleteAdminConfirmation");
        }

        private bool CurrentUserCanDeactivateAdmin(AdminAccount adminToDeactivate)
        {
            var loggedInAdmin = userDataService.GetAdminById(User.GetAdminId()!.GetValueOrDefault());

            return UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminToDeactivate, loggedInAdmin!.AdminAccount);
        }

    }
}
