﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
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
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.User;

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
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public AdministratorController(
            IUserDataService userDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            IUserService userService
        )
        {
            this.userDataService = userDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.userService = userService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1,
            int? itemsPerPage = null
        )
        {
            filterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                AdminFilterCookieName
            );

            var centreId = User.GetCentreId();
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var categories = GetCourseCategories(centreId);
            var loggedInUserId = User.GetAdminId();
            var loggedInAdminUser = userDataService.GetAdminUserById(loggedInUserId!.GetValueOrDefault());


            var model = new CentreAdministratorsViewModel(
                centreId,
                adminUsersAtCentre,
                categories,
                searchString,
                filterBy,
                page,
                loggedInAdminUser!,
                itemsPerPage
            );

            Response.UpdateOrDeleteFilterCookie(AdminFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllAdmins")]
        public IActionResult AllAdmins()
        {
            var centreId = User.GetCentreId();
            var loggedInUserId = User.GetAdminId();
            var loggedInAdminUser = userDataService.GetAdminUserById(loggedInUserId!.GetValueOrDefault());


            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var categories = GetCourseCategories(centreId);
            var model = new AllAdminsViewModel(
                adminUsersAtCentre,
                categories,
                loggedInAdminUser!
            );
            return View("AllAdmins", model);
        }

        [Route("{adminId:int}/EditAdminRoles")]
        [HttpGet]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult EditAdminRoles(int adminId, int? returnPage)
        {
            var centreId = User.GetCentreId();
            var adminUser = userDataService.GetAdminUserById(adminId);

            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });
            var numberOfAdmins = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

            var model = new EditRolesViewModel(adminUser!, centreId, categories, numberOfAdmins, returnPage);
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
                model.LearningCategory
            );

            return RedirectToAction("Index", new { page = model.ReturnPage });
        }

        [Route("{adminId:int}/UnlockAccount")]
        [HttpPost]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult UnlockAccount(int adminId)
        {
            userDataService.UpdateAdminUserFailedLoginCount(adminId, 0);

            return RedirectToAction("Index");
        }

        [Route("{adminId:int}/DeactivateAdmin")]
        [HttpGet]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult DeactivateAdmin(int adminId, int? returnPage)
        {
            var adminUser = userDataService.GetAdminUserById(adminId);

            if (!CanCurrentUserDeactivateAdmin(adminUser!))
            {
                return NotFound();
            }
            
            var model = new DeactivateAdminViewModel(adminUser!, returnPage);
            return View(model);
        }

        [Route("{adminId:int}/DeactivateAdmin")]
        [HttpPost]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessAdminUser))]
        public IActionResult DeactivateAdmin(int adminId, DeactivateAdminViewModel model)
        {
            var adminUser = userDataService.GetAdminUserById(adminId);

            if (!CanCurrentUserDeactivateAdmin(adminUser!))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            userService.DeactivateOrDeleteAdmin(adminId);

            return View("DeactivateAdminConfirmation");
        }

        private IEnumerable<string> GetCourseCategories(int centreId)
        {
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            categories = categories.Prepend("All");
            return categories;
        }

        private bool CanCurrentUserDeactivateAdmin(AdminUser adminToDeactivate)
        {
            var loggedInUserId = User.GetAdminId();
            var loggedInAdminUser = userDataService.GetAdminUserById(loggedInUserId!.GetValueOrDefault());

            return UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminToDeactivate!, loggedInAdminUser!);
        }
    }
}
