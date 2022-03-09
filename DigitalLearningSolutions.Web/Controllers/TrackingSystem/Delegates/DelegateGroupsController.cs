namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
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
        private readonly ICentreRegistrationPromptsService centreRegistrationPromptsService;
        private readonly IGroupsService groupsService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public DelegateGroupsController(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IGroupsService groupsService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.groupsService = groupsService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                DelegateGroupsFilterCookieName
            );

            var centreId = User.GetCentreId();
            var groups = groupsService.GetGroupsForCentre(centreId).ToList();
            var admins = groups.Select(
                g => (g.AddedByAdminId,
                    DisplayStringHelper.GetPotentiallyInactiveAdminName(
                        g.AddedByFirstName,
                        g.AddedByLastName,
                        g.AddedByAdminActive
                    ))
            ).Distinct();
            var registrationPrompts = GetRegistrationPromptsWithSetOptions(centreId);
            var availableFilters = new[]
            {
                new FilterModel(
                    nameof(Group.AddedByAdminId),
                    "Added by",
                    DelegateGroupsViewModelFilterOptions.GetAddedByOptions(admins)
                ),
                new FilterModel(
                    nameof(Group.LinkedToField),
                    "Linked field",
                    DelegateGroupsViewModelFilterOptions.GetLinkedFieldOptions(registrationPrompts)
                ),
            };

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                new FilterOptions(
                    existingFilterString,
                    availableFilters,
                    null
                ),
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                groups,
                searchSortPaginationOptions
            );

            var model = new DelegateGroupsViewModel(
                result,
                availableFilters
            );

            Response.UpdateFilterCookie(DelegateGroupsFilterCookieName, result.FilterString);

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

            var fieldIsValid = centreRegistrationPromptsService
                .GetCentreRegistrationPromptsThatHaveOptionsByCentreId(centreId).Select(cp => cp.RegistrationField.Id)
                .Contains(registrationField!.Id) || registrationField.Equals(RegistrationField.JobGroup);

            if (!fieldIsValid)
            {
                return StatusCode(500);
            }

            var groupDetails = new GroupGenerationDetails(
                adminId,
                centreId,
                registrationField,
                model.PrefixGroupName,
                model.PopulateExisting,
                model.AddNewRegistrants,
                model.SyncFieldChanges,
                model.SkipDuplicateNames
            );

            groupsService.GenerateGroupsFromRegistrationField(groupDetails);

            return RedirectToAction("Index");
        }

        private IEnumerable<CentreRegistrationPrompt> GetRegistrationPromptsWithSetOptions(int centreId)
        {
            return centreRegistrationPromptsService.GetCentreRegistrationPromptsByCentreId(centreId).CustomPrompts
                .Where(cp => cp.Options.Any());
        }

        private IEnumerable<SelectListItem> GetRegistrationFieldOptionsSelectList(int? selectedId = null)
        {
            var centreId = User.GetCentreId();

            var centreCustomPrompts = centreRegistrationPromptsService
                .GetCentreRegistrationPromptsThatHaveOptionsByCentreId(centreId);
            var registrationFieldOptions =
                PromptsService.MapCentreRegistrationPromptsToDataForSelectList(centreCustomPrompts);

            var jobGroupOption = (RegistrationField.JobGroup.Id, "Job group");
            registrationFieldOptions.Add(jobGroupOption);

            return SelectListHelper.MapOptionsToSelectListItems(registrationFieldOptions, selectedId);
        }
    }
}
