namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Administrator;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.PromoteToAdmin;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreManager)]
    [Route("TrackingSystem/Delegates/{delegateId:int}/PromoteToAdmin")]
    public class PromoteToAdminController : Controller
    {
        private readonly ICentreContractAdminUsageService centreContractAdminUsageService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly IUserDataService userDataService;
        private readonly IRegistrationService registrationService;

        public PromoteToAdminController(
            IUserDataService userDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            IRegistrationService registrationService
        )
        {
            this.userDataService = userDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.registrationService = registrationService;
        }

        [HttpGet]
        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserById(delegateId);

            if (delegateUser == null || delegateUser.CentreId != centreId)
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
                registrationService.PromoteDelegateToAdmin(formData.GetAdminRoles(), delegateId);
            }
            catch (AdminCreationFailedException e)
            {
                var error = e.Error;

                if (error.Equals(AdminCreationError.UnexpectedError))
                {
                    return new StatusCodeResult(500);
                }

                if (error.Equals(AdminCreationError.EmailAlreadyInUse))
                {
                    return View("EmailInUse", delegateId);
                }

                return new StatusCodeResult(500);
            }

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }
    }
}
