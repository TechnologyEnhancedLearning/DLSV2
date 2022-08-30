﻿namespace DigitalLearningSolutions.Web.Controllers.SupervisorController
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserSupervisor)]
    public partial class SupervisorController : Controller
    {
        private readonly ISupervisorService supervisorService;
        private readonly ICommonService commonService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly IFrameworkService frameworkService;
        private readonly IConfigDataService configDataService;
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IUserDataService userDataService;
        private readonly ILogger<SupervisorController> logger;
        private readonly IConfiguration config;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IRegistrationService registrationService;
        private readonly ICentresDataService centresDataService;
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
           ICentresDataService centresDataService
        )
        {
            this.supervisorService = supervisorService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.selfAssessmentService = selfAssessmentService;
            this.frameworkService = frameworkService;
            this.configDataService = configDataService;
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.userDataService = userDataService;
            this.logger = logger;
            this.config = config;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.multiPageFormService = multiPageFormService;
            this.registrationService = registrationService;
            this.centresDataService = centresDataService;
        }

        private int GetCentreId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserCentreId);
        }

        private int GetAdminID()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }

        private string GetUserEmail()
        {
            var userEmail = User.GetUserEmail();
            if (userEmail == null)
            {
                return "";
            }
            else
            {
                return userEmail;
            }
        }

        private string? GetBannerText()
        {
            var centreId = User.GetCentreId();
            var bannerText = centresDataService.GetBannerText(centreId);
            return bannerText;
        }
    }
}
