﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
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
        private readonly ICourseService courseService;
        private readonly IGroupsService groupsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserService userService;

        public DelegateGroupsController(
            ICentreCustomPromptsService centreCustomPromptsService,
            IGroupsService groupsService,
            IUserService userService,
            ICourseService courseService,
            IJobGroupsDataService jobGroupsDataService
        )
        {
            this.centreCustomPromptsService = centreCustomPromptsService;
            this.groupsService = groupsService;
            this.userService = userService;
            this.courseService = courseService;
            this.jobGroupsDataService = jobGroupsDataService;
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

        [HttpGet]
        [Route("Generate")]
        public IActionResult GenerateGroups()
        {
            var registrationFieldOptions = GetRegistrationFieldOptionsSelectList();

            var model = new GenerateGroupsViewModel(registrationFieldOptions);
            return View(model);
        }

        [HttpPost]
        [Route("Generate")]
        public IActionResult GenerateGroups(GenerateGroupsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.RegistrationFieldOptions = GetRegistrationFieldOptionsSelectList(model.RegistrationFieldOptionId);
                return View(model);
            }

            var adminId = User.GetAdminId();
            var centreId = User.GetCentreId();
            var isJobGroup = model.RegistrationFieldOptionId == 7;
            var linkedToField = GetLinkedToFieldValue((int)model.RegistrationFieldOptionId!);

            var (newGroupNames, groupNamePrefix) = GetNewGroupNamesAndPrefix(model, isJobGroup);

            using var transaction = new TransactionScope();
            foreach (var (newGroupId, newGroupName) in newGroupNames)
            {
                var groupName = model.PrefixGroupName ? $"{groupNamePrefix} - {newGroupName}" : newGroupName;

                var groupId = CreateNewGroup(model, adminId, centreId, linkedToField, groupName);

                if (model.AddExistingDelegates && groupId != null)
                {
                    groupsService.AddDelegatesWithMatchingAnswersToGroup(
                        (int)groupId,
                        linkedToField,
                        centreId,
                        isJobGroup ? null : newGroupName,
                        isJobGroup ? newGroupId : (int?)null
                    );
                }
            }

            transaction.Complete();

            return RedirectToAction("Index");
        }

        private IEnumerable<CustomPrompt> GetRegistrationPromptsWithSetOptions(int centreId)
        {
            return centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Where(cp => cp.Options.Any());
        }

        private IEnumerable<SelectListItem> GetRegistrationFieldOptionsSelectList(int? selectedId = null)
        {
            var registrationFieldOptions = GetCustomPromptOptions();

            var jobGroupOption = (7, "Job group");
            registrationFieldOptions.Add(jobGroupOption);

            return SelectListHelper.MapOptionsToSelectListItems(registrationFieldOptions, selectedId);
        }

        private List<(int id, string name)> GetCustomPromptOptions()
        {
            var centreId = User.GetCentreId();

            var customPromptOptions = centreCustomPromptsService
                .GetCustomPromptsThatHaveOptionsForCentreByCentreId(centreId)
                .Select(cp => (cp.CustomPromptNumber, cp.CustomPromptText)).ToList<(int id, string name)>();

            var customPromptNames = customPromptOptions.Select(r => r.name).ToList();

            if (customPromptNames.Distinct().Count() == customPromptOptions.Count)
            {
                return customPromptOptions;
            }

            return customPromptOptions.Select(
                cpo => customPromptNames.Count(cpn => cpn == cpo.name) > 1
                    ? (cpo.id, $"{cpo.name} (Prompt {cpo.id})")
                    : cpo
            ).ToList<(int id, string name)>();
        }

        private static int GetLinkedToFieldValue(int registrationFieldOptionId)
        {
            return registrationFieldOptionId switch
            {
                4 => 5,
                5 => 6,
                6 => 7,
                7 => 4,
                _ => registrationFieldOptionId,
            };
        }

        private (List<(int id, string name)>, string groupNamePrefix) GetNewGroupNamesAndPrefix(
            GenerateGroupsViewModel model,
            bool isJobGroup
        )
        {
            if (isJobGroup)
            {
                var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
                const string groupNamePrefix = "Job group";
                return (jobGroups, groupNamePrefix);
            }
            else
            {
                var centreId = User.GetCentreId();
                var registrationPrompt = centreCustomPromptsService
                    .GetCustomPromptsThatHaveOptionsForCentreByCentreId(centreId).Single(
                        cp => cp.CustomPromptNumber == model.RegistrationFieldOptionId
                    );
                var customPromptOptions = registrationPrompt.Options.Select((option, index) => (index, option))
                    .ToList<(int id, string name)>();
                var groupNamePrefix = registrationPrompt.CustomPromptText;
                return (customPromptOptions, groupNamePrefix);
            }
        }

        private int? CreateNewGroup(
            GenerateGroupsViewModel model,
            int? adminId,
            int centreId,
            int linkedToField,
            string groupName
        )
        {
            if (model.SkipDuplicateNames && groupsService.GetGroupAtCentreByName(groupName, centreId) != null)
            {
                return null;
            }

            return groupsService.AddDelegateGroup(
                centreId,
                groupName,
                null,
                (int)adminId!,
                linkedToField,
                model.SyncFieldChanges,
                model.AddNewRegistrants,
                model.AddExistingDelegates
            );
        }
    }
}
