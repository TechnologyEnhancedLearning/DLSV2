namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Services;
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
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
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
            var categories = commonService.GetCategoryListForCentre(centreId);
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

        private IEnumerable<string> GetCourseCategories(int centreId)
        {
            var categories = commonService.GetCategoryListForCentre(centreId).Select(c => c.CategoryName);
            categories = categories.Prepend("All");
            return categories;
        }
    }
}
