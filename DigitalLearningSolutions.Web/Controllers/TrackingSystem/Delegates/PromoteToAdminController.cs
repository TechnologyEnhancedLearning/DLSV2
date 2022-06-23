namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.PromoteToAdmin;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/PromoteToAdmin")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    public class PromoteToAdminController : Controller
    {
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ILogger<PromoteToAdminController> logger;
        private readonly IRegistrationService registrationService;
        private readonly IUserDataService userDataService;

        public PromoteToAdminController(
            IUserDataService userDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            IRegistrationService registrationService,
            ILogger<PromoteToAdminController> logger
        )
        {
            this.userDataService = userDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.registrationService = registrationService;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);

            if (delegateUser!.IsAdmin || !delegateUser.IsPasswordSet)
            {
                return NotFound();
            }

            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });
            var numberOfAdmins = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

            var model = new PromoteToAdminViewModel(delegateUser, centreId, categories, numberOfAdmins);
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(AdminRolesFormData formData, int delegateId)
        {
            try
            {
                registrationService.PromoteDelegateToAdmin(
                    formData.GetAdminRoles(),
                    AdminCategoryHelper.AdminCategoryToCategoryId(formData.LearningCategory),
                    delegateId
                );
            }
            catch (AdminCreationFailedException e)
            {
                logger.LogError(e, $"Error creating admin account for promoted delegate: {e.Message}");

                return new StatusCodeResult(500);
            }

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }
    }
}
