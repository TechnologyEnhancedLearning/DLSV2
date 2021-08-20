namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [Route("TrackingSystem/Centre/Administrators")]
    public class AdministratorController : Controller
    {
        private const string AdminFilterCookieName = "AdminFilter";
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(30);
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly IUserDataService userDataService;

        public AdministratorController(
            IUserDataService userDataService,
            ICourseCategoriesDataService courseCategoriesDataService
        )
        {
            this.userDataService = userDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            if (filterBy == null && filterValue == null)
            {
                filterBy = Request.Cookies[AdminFilterCookieName];
            }
            else if (filterBy != null && filterBy.ToUpper() == FilteringHelper.ClearString)
            {
                filterBy = null;
            }

            filterBy = FilteringHelper.AddNewFilterToFilterBy(filterBy, filterValue);

            var centreId = User.GetCentreId();
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var categories = GetCourseCategories(centreId);

            var model = new CentreAdministratorsViewModel(
                centreId,
                adminUsersAtCentre,
                categories,
                searchString,
                filterBy,
                page
            );

            Response.UpdateOrDeleteFilterCookie(AdminFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllAdmins")]
        public IActionResult AllAdmins()
        {
            var centreId = User.GetCentreId();
            var adminUsersAtCentre = userDataService.GetAdminUsersByCentreId(centreId);
            var categories = GetCourseCategories(centreId);
            var model = new AllAdminsViewModel(adminUsersAtCentre, categories);
            return View("AllAdmins", model);
        }

        [Route("{adminId:int}/EditAdminRoles")]
        [HttpGet]
        public IActionResult EditAdminRoles(int adminId)
        {
            var adminUser = userDataService.GetAdminUserById(adminId);
            if (adminUser == null)
            {
                return NotFound();
            }

            var centreId = User.GetCentreId();
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });

            var model = new EditRolesViewModel(adminUser, centreId, categories);
            return View(model);
        }

        [Route("{adminId:int}/EditAdminRoles")]
        [HttpPost]
        public IActionResult EditAdminRoles(EditRolesViewModel model, int adminId)
        {
            userDataService.UpdateAdminUserPermissions(
                adminId,
                model.IsCentreAdmin,
                model.IsSupervisor,
                model.IsTrainer,
                model.IsContentCreator,
                model.ContentManagementRole.IsContentManager,
                model.ContentManagementRole.ImportOnly,
                model.LearningCategory
            );

            return RedirectToAction("Index");
        }

        [Route("{adminId:int}/UnlockAccount")]
        [HttpPost]
        public IActionResult UnlockAccount(int adminId)
        {
            var centreId = User.GetCentreId();
            var adminUser = userDataService.GetAdminUserById(adminId);

            if (adminUser == null || adminUser.CentreId != centreId)
            {
                return NotFound();
            }

            userDataService.UpdateAdminUserFailedLoginCount(adminId, 0);

            return RedirectToAction("Index");
        }

        private IEnumerable<string> GetCourseCategories(int centreId)
        {
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            categories = categories.Prepend("All");
            return categories;
        }
    }
}
