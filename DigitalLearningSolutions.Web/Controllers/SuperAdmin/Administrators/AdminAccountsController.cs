namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Administrators
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Administrators;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]

    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Admins))]
    public class AdminAccountsController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly IAdminDownloadFileService adminDownloadFileService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ICentresDataService centresDataService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly IUserService userService;
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
        private readonly INotificationPreferencesDataService notificationPreferencesDataService;
        private readonly INotificationDataService notificationDataService;
        public AdminAccountsController(IUserDataService userDataService,
            ICentresDataService centresDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IAdminDownloadFileService adminDownloadFileService,
            ICourseCategoriesDataService courseCategoriesDataService,
            IUserService userService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            INotificationPreferencesDataService notificationPreferencesDataService,
            INotificationDataService notificationDataService
            )
        {
            this.userDataService = userDataService;
            this.centresDataService = centresDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.adminDownloadFileService = adminDownloadFileService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.userService = userService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.notificationPreferencesDataService = notificationPreferencesDataService;
            this.notificationDataService = notificationDataService;
        }

        [Route("SuperAdmin/AdminAccounts/{page=0:int}")]
        public IActionResult Index(
          int page = 1,
          string? Search = "",
          int AdminId = 0,
          string? UserStatus = "",
          string? Role = "",
          int? CentreId = 0,
          int? itemsPerPage = 10,
          string? SearchString = "",
          string? ExistingFilterString = ""
        )
        {
            var loggedInSuperAdmin = userDataService.GetAdminById(User.GetAdminId()!.Value);
            if (loggedInSuperAdmin.AdminAccount.Active == false)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(SearchString) || string.IsNullOrEmpty(ExistingFilterString))
            {
                page = 1;
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            UserStatus = (string.IsNullOrEmpty(UserStatus) ? "Any" : UserStatus);
            Role = (string.IsNullOrEmpty(Role) ? "Any" : Role);

            if (!string.IsNullOrEmpty(SearchString))
            {
                List<string> searchFilters = SearchString.Split("-").ToList();
                if (searchFilters.Count == 2)
                {
                    string searchFilter = searchFilters[0];
                    if (searchFilter.Contains("SearchQuery|"))
                    {
                        Search = searchFilter.Split("|")[1];
                    }

                    string userIdFilter = searchFilters[1];
                    if (userIdFilter.Contains("AdminID|"))
                    {
                        AdminId = Convert.ToInt16(userIdFilter.Split("|")[1]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(ExistingFilterString))
            {
                List<string> selectedFilters = ExistingFilterString.Split("-").ToList();
                if (selectedFilters.Count == 3)
                {
                    string adminStatusFilter = selectedFilters[0];
                    if (adminStatusFilter.Contains("UserStatus|"))
                    {
                        UserStatus = adminStatusFilter.Split("|")[1];
                    }

                    string roleFilter = selectedFilters[1];
                    if (roleFilter.Contains("Role|"))
                    {
                        Role = roleFilter.Split("|")[1];
                    }

                    string centreFilter = selectedFilters[2];
                    if (centreFilter.Contains("CentreID|"))
                    {
                        CentreId = Convert.ToInt16(centreFilter.Split("|")[1]);
                    }
                }
            }

            (var Admins, var ResultCount) = this.userDataService.GetAllAdmins(Search ?? string.Empty, offSet, itemsPerPage ?? 0, AdminId, UserStatus, Role, CentreId, AuthHelper.FailedLoginThreshold);

            var centres = centresDataService.GetAllCentres().ToList();
            centres.Insert(0, (0, "Any"));

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                Admins,
                searchSortPaginationOptions
            );

            result.Page = page;
            if (
                !string.IsNullOrEmpty(Search) ||
                AdminId != 0 ||
                !string.IsNullOrEmpty(UserStatus) ||
                !string.IsNullOrEmpty(Role) ||
                CentreId != 0
            )
            {
                result.SearchString = "SearchQuery|" + Search + "-AdminID|" + AdminId;
                result.FilterString = "UserStatus|" + UserStatus + "-Role|" + Role + "-CentreID|" + CentreId;
                TempData["SearchString"] = result.SearchString;
                TempData["FilterString"] = result.FilterString;
            }
            TempData["Page"] = result.Page;

            var model = new AdminAccountsViewModel(
                result,
                loggedInSuperAdmin!.AdminAccount
            );

            ViewBag.Roles = SelectListHelper.MapOptionsToSelectListItems(
                GetRoles(), Role
            );

            ViewBag.UserStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetUserStatus(), UserStatus
            );

            ViewBag.Centres = SelectListHelper.MapOptionsToSelectListItems(
                centres, CentreId
            );

            model.TotalPages = (int)(ResultCount / itemsPerPage) + ((ResultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = ResultCount;
            model.AdminID = AdminId == 0 ? null : AdminId;
            model.UserStatus = UserStatus;
            model.Role = Role;
            model.CentreID = CentreId == 0 ? null : CentreId;
            model.Search = Search;


            model.JavascriptSearchSortFilterPaginateEnabled = false;
            ViewBag.AdminId = TempData["AdminId"];
            ModelState.ClearAllErrors();
            return View(model);
        }

        [Route("SuperAdmin/Admins/{adminId=0:int}/ManageRoles")]
        public IActionResult ManageRoles(int adminId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var adminUser = userDataService.GetAdminUserById(adminId);

            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });
            var numberOfAdmins = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

            var model = new ManageRoleViewModel(adminUser!, centreId, categories, numberOfAdmins);
            var result = centresDataService.GetCentreDetailsById(centreId);
            model.CentreName = result.CentreName;

            if (TempData["SearchString"] != null)
            {
                model.SearchString = Convert.ToString(TempData["SearchString"]);
            }
            if (TempData["FilterString"] != null)
            {
                model.ExistingFilterString = Convert.ToString(TempData["FilterString"]);
            }
            if (TempData["Page"] != null)
            {
                model.Page = Convert.ToInt16(TempData["Page"]);
            }
            TempData["AdminId"] = adminId;
            TempData.Keep();
            return View(model);
        }


        [HttpPost]
        [Route("SuperAdmin/Admins/{adminId=0:int}/ManageRoles")]
        public IActionResult ManageRoles(AdminRolesFormData formData, int adminId)
        {
            if (!(formData.IsCentreAdmin ||
                formData.IsSupervisor ||
                formData.IsNominatedSupervisor ||
                formData.IsContentCreator ||
                formData.IsTrainer ||
                formData.IsCenterManager ||
                formData.ContentManagementRole.IsContentManager && formData.ContentManagementRole.ImportOnly ||
                formData.ContentManagementRole.IsContentManager && !formData.ContentManagementRole.ImportOnly ||
                formData.IsLocalWorkforceManager))
            {
                var centreId = User.GetCentreIdKnownNotNull();
                var adminUser = userDataService.GetAdminUserById(adminId);

                var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
                categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });
                var numberOfAdmins = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

                var model = new ManageRoleViewModel(adminUser!, centreId, categories, numberOfAdmins);
                var result = centresDataService.GetCentreDetailsById(centreId);
                model.CentreName = result.CentreName;
                model.ContentManagementRole = formData.ContentManagementRole;
                model.IsCentreAdmin = formData.IsCentreAdmin;
                model.IsSupervisor = formData.IsSupervisor;
                model.IsNominatedSupervisor = formData.IsNominatedSupervisor;
                model.IsContentCreator = formData.IsContentCreator;
                model.IsTrainer = formData.IsTrainer;
                model.IsCenterManager = formData.IsCenterManager;
                model.IsLocalWorkforceManager = formData.IsLocalWorkforceManager;
                model.IsSuperAdmin = formData.IsSuperAdmin;
                model.IsReportViewer = formData.IsReportViewer;
                model.IsFrameworkDeveloper = formData.IsFrameworkDeveloper;
                model.IsWorkforceManager = formData.IsWorkforceManager;

                if (TempData["SearchString"] != null)
                {
                    model.SearchString = Convert.ToString(TempData["SearchString"]);
                }
                if (TempData["FilterString"] != null)
                {
                    model.ExistingFilterString = Convert.ToString(TempData["FilterString"]);
                }
                if (TempData["Page"] != null)
                {
                    model.Page = Convert.ToInt16(TempData["Page"]);
                }
                TempData["AdminId"] = adminId;
                TempData.Keep();

                ModelState.Clear();
                ModelState.AddModelError("IsCenterManager", $"Delegate must have at least one role to be an Admin.");
                ViewBag.RequiredCheckboxMessage = "Delegate must have at least one role to be an Admin.";
                return View(model);
            }

            TempData["AdminId"] = adminId;
            int? learningCategory = formData.LearningCategory == 0 ? null : formData.LearningCategory;
            userDataService.UpdateAdminUserAndSpecialPermissions(
                adminId,
                formData.IsCentreAdmin,
                formData.IsSupervisor,
                formData.IsNominatedSupervisor,
                formData.IsTrainer,
                formData.IsContentCreator,
                formData.ContentManagementRole.IsContentManager,
                formData.ContentManagementRole.ImportOnly,
                learningCategory,
                formData.IsCenterManager,
                formData.IsSuperAdmin,
                formData.IsReportViewer,
                formData.IsLocalWorkforceManager,
                formData.IsFrameworkDeveloper,
                formData.IsWorkforceManager
            );

            int isCentreManager = formData.IsCenterManager ? 1 : 0;
            int isCMSManager = (formData.ContentManagementRole.IsContentManager && !formData.ContentManagementRole.ImportOnly) ? 1 : 0;
            int isContentCreator = formData.IsContentCreator ? 1 : 0;

            IEnumerable<int> notificationIds = notificationDataService.GetRoleBasedNotifications(isCentreManager, isCMSManager, isContentCreator);
            int userId = userDataService.GetUserIdFromAdminId(adminId);

            notificationPreferencesDataService.SetNotificationPreferencesForAdmin(userId, notificationIds);

            return RedirectToAction("Index", "AdminAccounts", new { AdminId = adminId });
        }

        [Route("Export")]
        public IActionResult Export(
            string? searchString = null,
            string? existingFilterString = null
        )
        {
            var content = adminDownloadFileService.GetAllAdminsFile(
                searchString,
                existingFilterString
            );

            const string fileName = "Digital Learning Solutions Administrators.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }

        public List<string> GetUserStatus()
        {
            return new List<string>(new string[] { "Any", "Active", "Inactive" });
        }

        public List<string> GetRoles()
        {
            var roles = new List<string>(new string[] { "Any" });

            foreach (var role in AdminAccountsViewModelFilterOptions.RoleOptions.Select(r => r.DisplayText))
            {
                roles.Add(role);
            }

            return roles;
        }

        [Route("SuperAdmin/AdminAccounts/{adminId=0:int}/DeleteAdmin")]
        public IActionResult DeleteAdmin(int adminId = 0)
        {
            userDataService.DeleteAdminAccount(adminId);
            return RedirectToAction("Index", "AdminAccounts");
        }

        [Route("SuperAdmin/AdminAccounts/{adminId=0:int}/{actionType='':string}/UpdateAdminStatus")]
        public IActionResult UpdateAdminStatus(int adminId, string actionType)
        {
            userDataService.UpdateAdminStatus(adminId, (actionType == "Reactivate"));
            TempData["AdminId"] = adminId;
            return RedirectToAction("Index", "AdminAccounts", new { AdminId = adminId });
        }

        [Route("SuperAdmin/AdminAccounts/{adminId=0:int}/ChangeCentre")]
        public IActionResult EditCentre(int adminId)
        {
            var adminUser = userDataService.GetAdminUserById(adminId);
            var centres = centresDataService.GetAllCentres(true).ToList();
            ViewBag.Centres = SelectListHelper.MapOptionsToSelectListItems(
               centres, adminUser.CentreId
           );
            var model = new EditCentreViewModel(adminUser, adminUser.CentreId);
            if (TempData["SearchString"] != null)
            {
                model.SearchString = Convert.ToString(TempData["SearchString"]);
            }
            if (TempData["FilterString"] != null)
            {
                model.ExistingFilterString = Convert.ToString(TempData["FilterString"]);
            }
            if (TempData["Page"] != null)
            {
                model.Page = Convert.ToInt16(TempData["Page"]);
            }
            TempData["AdminId"] = adminId;
            if (TempData["CentreId"] != null)
            {
                ModelState.AddModelError("CentreId", "User is already admin for the centre.");
                TempData.Remove("CentreId");
            }
            return View(model);
        }
        [HttpPost]
        [Route("SuperAdmin/AdminAccounts/{adminId=0:int}/ChangeCentre")]
        public IActionResult EditCentre(int adminId, int centreId)
        {
            TempData["AdminId"] = adminId;
            int? userId = userDataService.GetUserIdByAdminId(adminId);
            if (userDataService.IsUserAlreadyAdminAtCentre(userId, centreId))
            {
                TempData["CentreId"] = centreId;
                return RedirectToAction("EditCentre", "AdminAccounts", new { AdminId = adminId });
            }
            else
            {
                userDataService.UpdateAdminCentre(adminId, centreId);
                return RedirectToAction("Index", "AdminAccounts", new { AdminId = adminId });
            }
        }

        public IActionResult RedirectToUser(int UserId) {
            TempData["UserId"] = UserId;
            return RedirectToAction("Index", "Users",new { UserId = UserId });
        }

        [Route("SuperAdmin/AdminAccounts/{adminId:int}/DeactivateAdmin")]
        [HttpGet]
        public IActionResult DeactivateOrDeleteAdmin(int adminId, ReturnPageQuery returnPageQuery)
        {
            var admin = userDataService.GetAdminById(adminId);

            if (!CurrentUserCanDeactivateAdmin(admin!.AdminAccount))
            {
                return StatusCode((int)HttpStatusCode.Gone);
            }

            var model = new DeactivateAdminViewModel(admin, returnPageQuery);
            return View(model);
        }

        [Route("SuperAdmin/AdminAccounts/{adminId:int}/DeactivateAdmin")]
        [HttpPost]
        public IActionResult DeactivateOrDeleteAdmin(int adminId, DeactivateAdminViewModel model)
        {
            var admin = userDataService.GetAdminById(adminId);

            if (!CurrentUserCanDeactivateAdmin(admin!.AdminAccount))
            {
                return StatusCode((int)HttpStatusCode.Gone);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            userService.DeactivateOrDeleteAdminForSuperAdmin(adminId);

            return View("DeactivateOrDeleteAdminConfirmation");
        }

        private bool CurrentUserCanDeactivateAdmin(AdminAccount adminToDeactivate)
        {
            var loggedInAdmin = userDataService.GetAdminById(User.GetAdminId()!.GetValueOrDefault());

            return UserPermissionsHelper.LoggedInAdminCanDeactivateUser(adminToDeactivate, loggedInAdmin!.AdminAccount);
        }
    }
}
