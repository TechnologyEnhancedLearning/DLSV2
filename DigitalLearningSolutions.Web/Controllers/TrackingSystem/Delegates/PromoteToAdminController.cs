namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.PromoteToAdmin;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.FeatureManagement.Mvc;
    using IEmailService = DigitalLearningSolutions.Web.Services.IEmailService;

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
        private readonly IUserService userService;
        private readonly IEmailGenerationService emailGenerationService;
        private readonly IEmailService emailService;

        public PromoteToAdminController(
            IUserDataService userDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            IRegistrationService registrationService,
            ILogger<PromoteToAdminController> logger,
            IUserService userService,
            IEmailGenerationService emailGenerationService,
            IEmailService emailService
        )
        {
            this.userDataService = userDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.centreContractAdminUsageService = centreContractAdminUsageService;
            this.registrationService = registrationService;
            this.logger = logger;
            this.userService = userService;
            this.emailGenerationService = emailGenerationService;
            this.emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var userId = userDataService.GetUserIdFromDelegateId(delegateId);
            var userEntity = userService.GetUserById(userId);

            if (userEntity!.CentreAccountSetsByCentreId[centreId].CanLogIntoAdminAccount
                || string.IsNullOrWhiteSpace(userEntity.UserAccount.PasswordHash))
            {
                return NotFound();
            }

            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });
            var numberOfAdmins = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

            var model = new PromoteToAdminViewModel(
                userEntity.UserAccount.FirstName,
                userEntity.UserAccount.LastName,
                delegateId,
                userId,
                centreId,
                categories,
                numberOfAdmins
            );
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(AdminRolesFormData formData, int delegateId)
        {
            var adminRoles = formData.GetAdminRoles();

            if (!(adminRoles.IsCentreAdmin ||
                adminRoles.IsSupervisor ||
                adminRoles.IsNominatedSupervisor ||
                adminRoles.IsContentCreator ||
                adminRoles.IsTrainer ||
                adminRoles.IsCentreManager))
            {
                var centreId = User.GetCentreIdKnownNotNull();
                var userId = userDataService.GetUserIdFromDelegateId(delegateId);
                var userEntity = userService.GetUserById(userId);

                var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
                categories = categories.Prepend(new Category { CategoryName = "All", CourseCategoryID = 0 });
                var numberOfAdmins = centreContractAdminUsageService.GetCentreAdministratorNumbers(centreId);

                var model = new PromoteToAdminViewModel(
                                userEntity.UserAccount.FirstName,
                                userEntity.UserAccount.LastName,
                                delegateId,
                                userId,
                                centreId,
                                categories,
                                numberOfAdmins
                            );
                model.ContentManagementRole = formData.ContentManagementRole;
                ModelState.Clear();
                ModelState.AddModelError("IsCenterManager", $"Delegate must have one role to be promoted to Admin.");
                return View(model);
            }
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (currentAdminUser, _) = userService.GetUsersById(userAdminId, userDelegateId);

            var centreName = currentAdminUser.CentreName;

            try
            {
                registrationService.PromoteDelegateToAdmin(
                    adminRoles,
                    AdminCategoryHelper.AdminCategoryToCategoryId(formData.LearningCategory),
                    formData.UserId,
                    formData.CentreId
                );

                var delegateUserEmailDetails = userDataService.GetDelegateById(delegateId);

                var delegateEmail = delegateUserEmailDetails.UserAccount.PrimaryEmail;

                if (Guid.TryParse(delegateEmail, out _))
                {
                    delegateEmail = delegateUserEmailDetails.EmailForCentreNotifications;
                }

                if (delegateUserEmailDetails != null)
                {
                    var adminRolesEmail = emailGenerationService.GenerateDelegateAdminRolesNotificationEmail(
                        firstName: delegateUserEmailDetails.UserAccount.FirstName,
                        supervisorFirstName: currentAdminUser.FirstName!,
                        supervisorLastName: currentAdminUser.LastName,
                        supervisorEmail: currentAdminUser.EmailAddress!,
                        isCentreAdmin: adminRoles.IsCentreAdmin,
                        isCentreManager: adminRoles.IsCentreManager,
                        isSupervisor: adminRoles.IsSupervisor,
                        isNominatedSupervisor: adminRoles.IsNominatedSupervisor,
                        isTrainer: adminRoles.IsTrainer,
                        isContentCreator: adminRoles.IsContentCreator,
                        isCmsAdmin: adminRoles.IsCmsAdministrator,
                        isCmsManager: adminRoles.IsCmsManager,
                        primaryEmail: delegateEmail,
                        centreName: centreName
                    );

                    emailService.SendEmail(adminRolesEmail);
                }
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
