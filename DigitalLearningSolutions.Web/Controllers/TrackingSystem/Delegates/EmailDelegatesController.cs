namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.EmailDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using ConfigHelper = DigitalLearningSolutions.Web.Helpers.ConfigHelper;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Email")]
    public class EmailDelegatesController : Controller
    {
        private const string EmailDelegateFilterCookieName = "EmailDelegateFilter";
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IUserService userService;

        public EmailDelegatesController(
            CentreCustomPromptHelper centreCustomPromptHelper,
            IJobGroupsDataService jobGroupsDataService,
            IPasswordResetService passwordResetService,
            IUserService userService
        )
        {
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.jobGroupsDataService = jobGroupsDataService;
            this.passwordResetService = passwordResetService;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index(
            string? filterBy = null,
            string? filterValue = null,
            bool selectAll = false
        )
        {
            var newFilterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                EmailDelegateFilterCookieName
            );
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(User.GetCentreId());
            var delegateUsers = GetDelegateUserCards();

            var model = new EmailDelegatesViewModel(
                delegateUsers,
                jobGroups,
                customPrompts,
                newFilterBy,
                selectAll
            );

            Response.UpdateOrDeleteFilterCookie(EmailDelegateFilterCookieName, newFilterBy);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(EmailDelegatesViewModel model, string? filterBy = null, string? filterValue = null)
        {
            var delegateUsers = GetDelegateUserCards();

            if (!ModelState.IsValid)
            {
                var newFilterBy = FilteringHelper.GetFilterBy(
                    filterBy,
                    filterValue,
                    Request,
                    EmailDelegateFilterCookieName
                );
                var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
                var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(User.GetCentreId());
                var viewModel = new EmailDelegatesViewModel(delegateUsers, jobGroups, customPrompts, newFilterBy)
                {
                    SelectedDelegateIds = model.SelectedDelegateIds,
                    Day = model.Day,
                    Month = model.Month,
                    Year = model.Year,
                };
                return View(viewModel);
            }

            var selectedUsers = delegateUsers.Where(user => model.SelectedDelegateIds!.Contains(user.Id)).ToList();
            var emailDate = new DateTime(model.Year!.Value, model.Month!.Value, model.Day!.Value);
            var baseUrl = ConfigHelper.GetAppRootPath(ConfigHelper.GetAppConfig());

            passwordResetService.SendWelcomeEmailsToDelegates(selectedUsers, emailDate, baseUrl);

            return View("Confirmation", selectedUsers.Count);
        }

        [Route("AllEmailDelegateItems")]
        public IActionResult AllEmailDelegateItems(IEnumerable<int> selectedIds)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(User.GetCentreId());
            var delegateUsers = GetDelegateUserCards();

            var model = new AllEmailDelegateItemsViewModel(delegateUsers, jobGroups, customPrompts, selectedIds);

            return View(model);
        }

        private IEnumerable<DelegateUserCard> GetDelegateUserCards()
        {
            var centreId = User.GetCentreId();
            return userService.GetDelegateUserCardsForWelcomeEmail(centreId)
                .OrderByDescending(card => card.DateRegistered);
        }
    }
}
