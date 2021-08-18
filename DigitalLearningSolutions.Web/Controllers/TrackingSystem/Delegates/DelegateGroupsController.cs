﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/Groups")]
    public class DelegateGroupsController : Controller
    {
        private const string DelegateGroupsFilterCookieName = "DelegateGroupsFilter";
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(30);
        private readonly ICustomPromptsService customPromptsService;
        private readonly IGroupsDataService groupsDataService;

        public DelegateGroupsController(
            IGroupsDataService groupsDataService,
            ICustomPromptsService customPromptsService
        )
        {
            this.groupsDataService = groupsDataService;
            this.customPromptsService = customPromptsService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            // Query parameter should take priority over cookie value
            // We use this method to check for the query parameter rather 
            // than filterBy != null as filterBy is set to null to clear 
            // the filter string when javascript is off.
            if (!Request.Query.ContainsKey(nameof(filterBy)))
            {
                filterBy = Request.Cookies[DelegateGroupsFilterCookieName];
            }

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.AddNewFilterToFilterBy(filterBy, filterValue);

            var centreId = User.GetCentreId();
            var groups = groupsDataService.GetGroupsForCentre(centreId);

            var model = new DelegateGroupsViewModel(
                groups,
                GetRegistrationPrompts(centreId),
                sortBy,
                sortDirection,
                filterBy,
                page
            );

            if (filterBy != null)
            {
                Response.Cookies.Append(
                    DelegateGroupsFilterCookieName,
                    filterBy,
                    new CookieOptions
                    {
                        Expires = CookieExpiry
                    }
                );
            }
            else
            {
                Response.Cookies.Delete(DelegateGroupsFilterCookieName);
            }

            return View(model);
        }

        [Route("AllDelegateGroups")]
        public IActionResult AllDelegateGroups()
        {
            var centreId = User.GetCentreId();
            var groups = groupsDataService.GetGroupsForCentre(centreId);

            var model = new AllDelegateGroupsViewModel(groups, GetRegistrationPrompts(centreId));

            return View(model);
        }

        [Route("{groupId:int}/Delegates/{page:int=1}")]
        public IActionResult GroupDelegates(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsDataService.GetGroupName(groupId, centreId);

            if (groupName == null)
            {
                return NotFound();
            }

            var groupDelegates = groupsDataService.GetGroupDelegates(groupId);

            var model = new GroupDelegatesViewModel(groupId, groupName, groupDelegates, page);

            return View(model);
        }

        [Route("{groupId:int}/Courses/{page:int=1}")]
        public IActionResult GroupCourses(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsDataService.GetGroupName(groupId, centreId);

            if (groupName == null)
            {
                return NotFound();
            }
            var groupCourses = groupsDataService.GetGroupCourses(groupId, centreId);

            var model = new GroupCoursesViewModel(groupId, groupName, groupCourses, page);

            return View(model);
        }

        private IEnumerable<(int, string)> GetRegistrationPrompts(int centreId)
        {
            return customPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Where(cp => cp.Options.Any())
                .Select(
                    cp => (cp.CustomPromptNumber > 3 ? cp.CustomPromptNumber + 1 : cp.CustomPromptNumber,
                        cp.CustomPromptText)
                );
        }
    }
}
