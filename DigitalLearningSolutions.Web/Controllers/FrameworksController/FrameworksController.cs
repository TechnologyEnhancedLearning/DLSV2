﻿namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using GDS.MultiPageFormData;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.AspNetCore.Hosting;

    [Authorize(Policy = CustomPolicies.UserFrameworksAdminOnly)]
    [SetDlsSubApplication(nameof(DlsSubApplication.Frameworks))]
    [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
    public partial class FrameworksController : Controller
    {
        private readonly IFrameworkService frameworkService;
        private readonly ICommonService commonService;
        private readonly IFrameworkNotificationService frameworkNotificationService;
        private readonly ILogger<FrameworksController> logger;
        private readonly IImportCompetenciesFromFileService importCompetenciesFromFileService;
        private readonly ICompetencyLearningResourcesService competencyLearningResourcesService;
        private readonly ILearningHubApiClient learningHubApiClient;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IClockUtility clockUtility;
        private readonly IWebHostEnvironment webHostEnvironment;
        public FrameworksController(
            IFrameworkService frameworkService,
            ICommonService commonService,
            IFrameworkNotificationService frameworkNotificationService,
            ILogger<FrameworksController> logger,
            IImportCompetenciesFromFileService importCompetenciesFromFileService,
            ICompetencyLearningResourcesService competencyLearningResourcesService,
            ILearningHubApiClient learningHubApiClient,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IMultiPageFormService multiPageFormService,
            IClockUtility clockUtility,
            IWebHostEnvironment webHostEnvironment
        )
        {
            this.frameworkService = frameworkService;
            this.commonService = commonService;
            this.frameworkNotificationService = frameworkNotificationService;
            this.logger = logger;
            this.importCompetenciesFromFileService = importCompetenciesFromFileService;
            this.competencyLearningResourcesService = competencyLearningResourcesService;
            this.learningHubApiClient = learningHubApiClient;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.multiPageFormService = multiPageFormService;
            this.clockUtility = clockUtility;
            this.webHostEnvironment = webHostEnvironment;
        }

        private int? GetCentreId()
        {
            return User.GetCustomClaimAsInt(CustomClaimTypes.UserCentreId);
        }

        private int GetAdminId()
        {
            return User.GetCustomClaimAsRequiredInt(CustomClaimTypes.UserAdminId);
        }

        private bool GetIsFrameworkDeveloper()
        {
            var isFrameworkDeveloper = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper);
            return isFrameworkDeveloper != null && (bool)isFrameworkDeveloper;
        }

        private bool GetIsFrameworkContributor()
        {
            var isFrameworkContributor = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor);
            return isFrameworkContributor != null && (bool)isFrameworkContributor;
        }

        private string? GetUserFirstName()
        {
            return User.GetCustomClaim(CustomClaimTypes.UserForename);
        }

        private bool GetIsWorkforceManager()
        {
            var isWorkforceManager = User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceManager);
            return isWorkforceManager != null && (bool)isWorkforceManager;
        }
        private bool GetIsWorkforceContributor()
        {
            var isWorkforceContributor = User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceContributor);
            return isWorkforceContributor != null && (bool)isWorkforceContributor;
        }
    }
}
