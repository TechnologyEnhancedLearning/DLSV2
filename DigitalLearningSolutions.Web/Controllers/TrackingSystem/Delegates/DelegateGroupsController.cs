﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Groups")]
    public class DelegateGroupsController : Controller
    {
        private const string DelegateGroupsFilterCookieName = "DelegateGroupsFilter";
        private readonly ICentreCustomPromptsService centreCustomPromptsService;
        private readonly IGroupsService groupsService;

        public DelegateGroupsController(
            ICentreCustomPromptsService centreCustomPromptsService,
            IGroupsService groupsService
        )
        {
            this.centreCustomPromptsService = centreCustomPromptsService;
            this.groupsService = groupsService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                DelegateGroupsFilterCookieName
            );

            var centreId = User.GetCentreId();
            var groups = groupsService.GetGroupsForCentre(centreId).ToList();

            var model = new DelegateGroupsViewModel(
                groups,
                GetRegistrationPromptsWithSetOptions(centreId),
                searchString,
                sortBy,
                sortDirection,
                filterBy,
                page
            );

            Response.UpdateOrDeleteFilterCookie(DelegateGroupsFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllDelegateGroups")]
        public IActionResult AllDelegateGroups()
        {
            var centreId = User.GetCentreId();
            var groups = groupsService.GetGroupsForCentre(centreId).ToList();

            var model = new AllDelegateGroupsViewModel(groups, GetRegistrationPromptsWithSetOptions(centreId));

            return View(model);
        }

        [HttpGet("Add")]
        public IActionResult AddDelegateGroup()
        {
            return View(new AddDelegateGroupViewModel());
        }

        [HttpPost("Add")]
        public IActionResult AddDelegateGroup(AddDelegateGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            groupsService.AddDelegateGroup(
                User.GetCentreId(),
                model.GroupName!,
                model.GroupDescription,
                User.GetAdminId()!.Value
            );
            return RedirectToAction("Index");
        }

        [HttpGet("{groupId:int}/EditDescription")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult EditDescription(int groupId, int? returnPage)
        {
            var centreId = User.GetCentreId();
            var group = groupsService.GetGroupAtCentreById(groupId, centreId);

            var model = new EditDelegateGroupDescriptionViewModel(group, returnPage);
            return View(model);
        }

        [HttpPost("{groupId:int}/EditDescription")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult EditDescription(EditDelegateGroupDescriptionViewModel model, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();
            groupsService.UpdateGroupDescription(
                groupId,
                centreId,
                model.Description
            );

            return RedirectToAction("Index");
        }

        [HttpGet("{groupId:int}/EditGroupName")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult EditGroupName(int groupId, int? returnPage)
        {
            var centreId = User.GetCentreId();
            var group = groupsService.GetGroupAtCentreById(groupId, centreId);

            if (group?.LinkedToField != 0)
            {
                return NotFound();
            }

            var model = new EditGroupNameViewModel(group.GroupLabel!, returnPage);
            return View(model);
        }

        [HttpPost("{groupId:int}/EditGroupName")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult EditGroupName(EditGroupNameViewModel model, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();
            var group = groupsService.GetGroupAtCentreById(groupId, centreId);

            if (group?.LinkedToField != 0)
            {
                return NotFound();
            }

            groupsService.UpdateGroupName(
                groupId,
                centreId,
                model.GroupName
            );

            return RedirectToAction("Index");
        }

        [Route("{groupId:int}/Delete")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult DeleteGroup(int groupId, int? returnPage)
        {
            var delegates = groupsService.GetGroupDelegates(groupId);
            var courses = groupsService.GetUsableGroupCoursesForCentre(groupId, User.GetCentreId());

            if (delegates.Any() || courses.Any())
            {
                return RedirectToAction("ConfirmDeleteGroup", new { groupId, returnPage });
            }

            groupsService.DeleteDelegateGroup(groupId, false);
            return RedirectToAction("Index");
        }

        [HttpGet("{groupId:int}/Delete/Confirm")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult ConfirmDeleteGroup(int groupId, int? returnPage)
        {
            var groupLabel = groupsService.GetGroupName(groupId, User.GetCentreId())!;
            var delegateCount = groupsService.GetGroupDelegates(groupId).Count();
            var courseCount = groupsService.GetUsableGroupCoursesForCentre(groupId, User.GetCentreId()).Count();

            var model = new ConfirmDeleteGroupViewModel
            {
                GroupLabel = groupLabel,
                DelegateCount = delegateCount,
                CourseCount = courseCount,
                ReturnPage = returnPage,
            };

            return View(model);
        }

        [HttpPost("{groupId:int}/Delete/Confirm")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult ConfirmDeleteGroup(int groupId, ConfirmDeleteGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            groupsService.DeleteDelegateGroup(groupId, model.DeleteEnrolments);

            return RedirectToAction("Index");
        }

        [HttpGet("Generate")]
        public IActionResult GenerateGroups()
        {
            var registrationFieldOptions = GetRegistrationFieldOptionsSelectList();

            var model = new GenerateGroupsViewModel(registrationFieldOptions);
            return View(model);
        }

        [HttpPost("Generate")]
        public IActionResult GenerateGroups(GenerateGroupsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RegistrationFieldOptions = GetRegistrationFieldOptionsSelectList(model.RegistrationFieldOptionId);
                return View(model);
            }

            var adminId = User.GetAdminIdKnownNotNull()!;
            var centreId = User.GetCentreId();
            var registrationField = (RegistrationField)model.RegistrationFieldOptionId;

            var groupDetails = new GroupGenerationDetails(
                adminId,
                centreId,
                registrationField!,
                model.PrefixGroupName,
                model.PopulateExisting,
                model.AddNewRegistrants,
                model.SyncFieldChanges,
                model.SkipDuplicateNames
            );

            groupsService.GenerateGroupsFromRegistrationField(groupDetails);

            return RedirectToAction("Index");
        }

        private IEnumerable<CustomPrompt> GetRegistrationPromptsWithSetOptions(int centreId)
        {
            return centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Where(cp => cp.Options.Any());
        }

        private IEnumerable<SelectListItem> GetRegistrationFieldOptionsSelectList(int? selectedId = null)
        {
            var centreId = User.GetCentreId();

            var centreCustomPrompts = centreCustomPromptsService
                .GetCustomPromptsThatHaveOptionsForCentreByCentreId(centreId);
            var registrationFieldOptions =
                CentreCustomPromptHelper.MapCentreCustomPromptsToSelectList(centreCustomPrompts);

            var jobGroupOption = (7, "Job group");
            registrationFieldOptions.Add(jobGroupOption);

            return SelectListHelper.MapOptionsToSelectListItems(registrationFieldOptions, selectedId);
        }
    }
}
