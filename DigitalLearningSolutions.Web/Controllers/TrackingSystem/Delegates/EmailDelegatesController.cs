namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Email")]
    public class EmailDelegatesController : Controller
    {
        private const string EmailDelegateFilterCookieName = "EmailDelegateFilter";
        private readonly PromptsService promptsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IUserService userService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IConfiguration config;
        private readonly IClockUtility clockUtility;

        public EmailDelegatesController(
            PromptsService promptsService,
            IJobGroupsDataService jobGroupsDataService,
            IPasswordResetService passwordResetService,
            IUserService userService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IConfiguration config,
            IClockUtility clockUtility
        )
        {
            this.promptsService = promptsService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.passwordResetService = passwordResetService;
            this.userService = userService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.config = config;
            this.clockUtility = clockUtility;
        }

        [HttpGet]
        public IActionResult Index(
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            bool selectAll = false
        )
        {
            var newFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                EmailDelegateFilterCookieName
            );
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(User.GetCentreIdKnownNotNull());
            var delegateUsers = GetDelegateUserCards();

            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            var availableFilters = EmailDelegatesViewModelFilterOptions.GetEmailDelegatesFilterModels(
                jobGroups,
                promptsWithOptions
            );

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                new FilterOptions(newFilterString, availableFilters),
                null
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                delegateUsers,
                searchSortPaginationOptions
            );

            var model = new EmailDelegatesViewModel(
                result,
                availableFilters,
                clockUtility.UtcToday,
                selectAll
            );

            Response.UpdateFilterCookie(EmailDelegateFilterCookieName, result.FilterString);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(
            EmailDelegatesFormData formData,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false
        )
        {
            var delegateUsers = GetDelegateUserCards();

            if (!ModelState.IsValid)
            {
                var newFilterString = FilteringHelper.GetFilterString(
                    existingFilterString,
                    newFilterToAdd,
                    clearFilters,
                    Request,
                    EmailDelegateFilterCookieName
                );
                var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
                var customPrompts = promptsService.GetCentreRegistrationPrompts(User.GetCentreIdKnownNotNull());

                var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
                var availableFilters = EmailDelegatesViewModelFilterOptions.GetEmailDelegatesFilterModels(
                    jobGroups,
                    promptsWithOptions
                );

                var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                    null,
                    null,
                    new FilterOptions(newFilterString, availableFilters),
                    null
                );

                var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                    delegateUsers,
                    searchSortPaginationOptions
                );

                var viewModel = new EmailDelegatesViewModel(result, availableFilters, formData);

                if (viewModel.Delegates != null && viewModel.Delegates.Where(x => x.IsDelegateSelected).Count() == 0)
                {
                    ModelState.Clear();
                    ModelState.AddModelError(viewModel.Delegates.FirstOrDefault().Id+"-checkbox", $"You must select at least one delegate");
                    ViewBag.RequiredCheckboxMessage = "You must select at least one delegate";
                }

                return View(viewModel);
            }

            var emailDate = new DateTime(formData.Year!.Value, formData.Month!.Value, formData.Day!.Value);
            var baseUrl = config.GetAppRootPath();

            passwordResetService.SendWelcomeEmailsToDelegates(formData.SelectedDelegateIds!, emailDate, baseUrl);

            return View("Confirmation", formData.SelectedDelegateIds!.Count());
        }

        [Route("AllEmailDelegateItems")]
        public IActionResult AllEmailDelegateItems(IEnumerable<int> selectedIds)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(User.GetCentreIdKnownNotNull());
            var delegateUsers = GetDelegateUserCards();

            var model = new AllEmailDelegateItemsViewModel(delegateUsers, jobGroups, customPrompts, selectedIds);

            return View(model);
        }

        private IEnumerable<DelegateUserCard> GetDelegateUserCards()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            return userService.GetDelegateUserCardsForWelcomeEmail(centreId)
                .OrderByDescending(card => card.DateRegistered);
        }
    }
}
