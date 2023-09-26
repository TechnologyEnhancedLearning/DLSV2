﻿namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using GDS.MultiPageFormData;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserSupervisor)]
    public partial class SupervisorController : Controller
    {
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly IFrameworkService frameworkService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ISupervisorService supervisorService;
        private readonly IUserDataService userDataService;
        private readonly IRegistrationService registrationService;
        private readonly ICentresDataService centresDataService;
        private readonly IUserService userService;
        private readonly IEmailGenerationService emailGenerationService;
        private readonly IEmailService emailService;
        private readonly ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService;
        private readonly IClockUtility clockUtility;

        public SupervisorController(
           ISupervisorService supervisorService,
           ICommonService commonService,
           IFrameworkNotificationService frameworkNotificationService,
           ISelfAssessmentService selfAssessmentService,
           IFrameworkService frameworkService,
           IConfigDataService configDataService,
           ICentreRegistrationPromptsService centreRegistrationPromptsService,
           IUserDataService userDataService,
           ILogger<SupervisorController> logger,
           IConfiguration config,
           ISearchSortFilterPaginateService searchSortFilterPaginateService,
           IMultiPageFormService multiPageFormService,
           IRegistrationService registrationService,
           ICentresDataService centresDataService,
           IUserService userService,
           IEmailGenerationService emailGenerationService,
           IEmailService emailService,
           ICandidateAssessmentDownloadFileService candidateAssessmentDownloadFileService,
           IClockUtility clockUtility
           )
        {
            this.supervisorService = supervisorService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.frameworkService = frameworkService;
            this.selfAssessmentService = selfAssessmentService;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userDataService = userDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.multiPageFormService = multiPageFormService;
            this.registrationService = registrationService;
            this.centresDataService = centresDataService;
            this.userService = userService;
            this.emailGenerationService = emailGenerationService;
            this.emailService = emailService;
            this.candidateAssessmentDownloadFileService = candidateAssessmentDownloadFileService;
            this.clockUtility = clockUtility;
        }

        private int GetCentreId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId);
        }

        private int GetAdminId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }

        private string GetUserEmail()
        {
            var adminId = GetAdminId();
            var adminEntity = userDataService.GetAdminById(adminId);
            return adminEntity!.EmailForCentreNotifications;
        }

        private string? GetBannerText()
        {
            var centreId = (int)User.GetCentreId();
            var bannerText = centresDataService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
