namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Centre.Administrator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [Route("TrackingSystem/Centre/Administrators")]
    public class AdministratorController : Controller
    {
        private const string AdminFilterCookieName = "AdminFilter";
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(30);
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
            // Query parameter should take priority over cookie value
            if (!Request.Query.ContainsKey(nameof(filterBy)))
            {
                filterBy = Request.Cookies[AdminFilterCookieName];
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

            if (filterBy != null)
            {
                Response.Cookies.Append(
                    AdminFilterCookieName,
                    filterBy,
                    new CookieOptions
                    {
                        Expires = CookieExpiry
                    }
                );
            }
            else
            {
                Response.Cookies.Delete(AdminFilterCookieName);
            }

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

        private IEnumerable<string> GetCourseCategories(int centreId)
        {
            var categories = commonService.GetCategoryListForCentre(centreId).Select(c => c.CategoryName);
            categories = categories.Prepend("All");
            return categories;
        }
    }
}
