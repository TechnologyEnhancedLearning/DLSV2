﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
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
        private readonly ICourseCategoriesService courseCategoriesService;
        private readonly ILogger<PromoteToAdminController> logger;
        private readonly IRegistrationService registrationService;
        private readonly IUserService userService;
        private readonly IEmailGenerationService emailGenerationService;
        private readonly IEmailService emailService;

        public PromoteToAdminController(
            ICourseCategoriesService courseCategoriesService,
            ICentreContractAdminUsageService centreContractAdminUsageService,
            IRegistrationService registrationService,
            ILogger<PromoteToAdminController> logger,
            IUserService userService,
            IEmailGenerationService emailGenerationService,
            IEmailService emailService
        )
        {
            this.courseCategoriesService = courseCategoriesService;
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
            var userId = userService.GetUserIdFromDelegateId(delegateId);
            var userEntity = userService.GetUserById(userId);

            if (TempData["IsDelegatePromoted"] != null)
            {
                TempData.Remove("IsDelegatePromoted");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            }
            if (userEntity!.CentreAccountSetsByCentreId[centreId].CanLogIntoAdminAccount)
            {
                return NotFound();
            }

            var categories = courseCategoriesService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
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
                adminRoles.IsCentreManager ||
                adminRoles.IsContentManager))
            {
                var centreId = User.GetCentreIdKnownNotNull();
                var userId = userService.GetUserIdFromDelegateId(delegateId);
                var userEntity = userService.GetUserById(userId);

                var categories = courseCategoriesService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
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
                ViewBag.RequiredCheckboxMessage = "Delegate must have one role to be promoted to Admin.";
                return View(model);
            }
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var currentAdminUser = userService.GetAdminUserByAdminId(userAdminId);

            var centreName = currentAdminUser.CentreName;

            try
            {
                registrationService.PromoteDelegateToAdmin(
                    adminRoles,
                    AdminCategoryHelper.AdminCategoryToCategoryId(formData.LearningCategory),
                    formData.UserId,
                    formData.CentreId,
                    false
                );

                var delegateUserEmailDetails = userService.GetDelegateById(delegateId);

                int? learningCategory = formData.LearningCategory == 0 ? null : formData.LearningCategory;
                var learningCategoryName = courseCategoriesService.GetCourseCategoryName(learningCategory);

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
                        primaryEmail: delegateUserEmailDetails.EmailForCentreNotifications,
                        centreName: centreName,
                        categoryName: learningCategoryName
                    );

                    emailService.SendEmail(adminRolesEmail);
                }
            }
            catch (AdminCreationFailedException e)
            {
                logger.LogError(e, $"Error creating admin account for promoted delegate: {e.Message}");

                return new StatusCodeResult(500);
            }
            TempData["IsDelegatePromoted"] = true;
            return RedirectToAction("Index", "ViewDelegate", new { delegateId = delegateId, callType = ViewDelegateNavigationType.PromoteToAdmin });
        }
    }
}
