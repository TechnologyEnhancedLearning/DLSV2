﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private const string DelegateFilterCookieName = "DelegateFilter";
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public AllDelegatesController(
            IUserDataService userDataService,
            CentreCustomPromptHelper centreCustomPromptHelper,
            IJobGroupsDataService jobGroupsDataService
        )
        {
            this.userDataService = userDataService;
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.jobGroupsDataService = jobGroupsDataService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            int page = 1,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int? itemsPerPage = null
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                DelegateFilterCookieName,
                DelegateActiveStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(centreId);
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);

            var model = new AllDelegatesViewModel(
                delegateUsers,
                jobGroups,
                customPrompts,
                page,
                searchString,
                sortBy,
                sortDirection,
                filterBy,
                itemsPerPage
            );

            Response.UpdateOrDeleteFilterCookie(DelegateFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllDelegateItems")]
        public IActionResult AllDelegateItems()
        {
            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(centreId);
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);

            var model = new AllDelegateItemsViewModel(delegateUsers, jobGroups, customPrompts);

            return View(model);
        }
    }
}
