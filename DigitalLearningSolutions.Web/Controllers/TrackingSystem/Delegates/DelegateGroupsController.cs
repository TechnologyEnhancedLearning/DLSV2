using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Helpers;
using DigitalLearningSolutions.Data.Models.CustomPrompts;
using DigitalLearningSolutions.Data.Models.DelegateGroups;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Helpers.FilterOptions;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.ServiceFilter;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
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
        private readonly IPaginateService paginateService;

        public DelegateGroupsController(
            ICentreRegistrationPromptsService centreRegistrationPromptsService,
            IGroupsService groupsService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IPaginateService paginateService
        )
        {
            this.centreRegistrationPromptsService = centreRegistrationPromptsService;
            this.groupsService = groupsService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.paginateService = paginateService;
        }

        [NoCaching]
        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = "",
            string? sortDirection = "",
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1,
            int? itemsPerPage = 10
        )
        {
            searchString = searchString == null ? string.Empty : searchString.Trim();
            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = DefaultSortByOptions.Name.PropertyName;
            }
            if (string.IsNullOrEmpty(sortDirection))
            {
                sortDirection = GenericSortingHelper.Ascending;
            }

            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                DelegateGroupsFilterCookieName,
                null
            );

            var centreId = User.GetCentreIdKnownNotNull();

            var addedByAdmins = groupsService.GetAdminsForCentreGroups(centreId);

            foreach (var admin in addedByAdmins)
            {
                admin.FullName = DisplayStringHelper.GetPotentiallyInactiveAdminName(
                    admin.Forename,
                    admin.Surname,
                    admin.Active
                );
            }

            var registrationPrompts = GetRegistrationPromptsWithSetOptions(centreId);
            var availableFilters = DelegateGroupsViewModelFilterOptions
                .GetDelegateGroupFilterModels(addedByAdmins, registrationPrompts).ToList();

            if (TempData["DelegateGroupCentreId"]?.ToString() != centreId.ToString()
                    && existingFilterString != null)
            {
                existingFilterString = FilterHelper.RemoveNonExistingGroupFilters(availableFilters, existingFilterString);
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            string filterAddedBy = "";
            string filterLinkedField = "";

            if (!string.IsNullOrEmpty(existingFilterString))
            {
                var selectedFilters = existingFilterString.Split(FilteringHelper.FilterSeparator).ToList();

                if (!string.IsNullOrEmpty(newFilterToAdd))
                {
                    var filterHeader = newFilterToAdd.Split(FilteringHelper.Separator)[0];
                    var dupfilters = selectedFilters.Where(x => x.Contains(filterHeader));
                    if (dupfilters.Count() > 1)
                    {
                        foreach (var filter in selectedFilters)
                        {
                            if (filter.Contains(filterHeader))
                            {
                                selectedFilters.Remove(filter);
                                existingFilterString = string.Join(FilteringHelper.FilterSeparator, selectedFilters);
                                break;
                            }
                        }
                    }
                }

                if (selectedFilters.Count > 0)
                {
                    foreach (var filter in selectedFilters)
                    {
                        var filterArr = filter.Split(FilteringHelper.Separator);
                        var filterValue = filterArr[2];
                        if (filterValue == FilteringHelper.EmptyValue) filterValue = "No option selected";

                        if (filter.Contains("AddedBy"))
                            filterAddedBy = filterValue;

                        if (filter.Contains("LinkedToField"))
                            filterLinkedField = filterValue;
                    }
                }
            }

            (var groups, var resultCount) = groupsService.GetGroupsForCentre(
                search: searchString ?? string.Empty,
                offSet,
                rows: itemsPerPage ?? 0,
                sortBy,
                sortDirection,
                centreId: centreId,
                filterAddedBy,
                filterLinkedField);

            if (groups.Count() == 0 && resultCount > 0)
            {
                page = 1;
                offSet = 0;

                (groups, resultCount) = groupsService.GetGroupsForCentre(
                    search: searchString ?? string.Empty,
                    offSet,
                    rows: itemsPerPage ?? 0,
                    sortBy,
                    sortDirection,
                    centreId: centreId,
                    filterAddedBy,
                    filterLinkedField);
            }

            var result = paginateService.Paginate(
                groups,
                resultCount,
                new PaginationOptions(page, itemsPerPage),
                new FilterOptions(existingFilterString, availableFilters, DelegateActiveStatusFilterOptions.IsActive.FilterValue),
                searchString,
                sortBy,
                sortDirection
            );

            result.Page = page;
            TempData["Page"] = result.Page;

            var model = new DelegateGroupsViewModel(
                result,
                availableFilters
            );

            model.TotalPages = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = resultCount;
            Response.UpdateFilterCookie(DelegateGroupsFilterCookieName, result.FilterString);
            TempData["DelegateGroupCentreId"] = centreId;

            return View(model);
        }

        [Route("AllDelegateGroups")]
        public IActionResult AllDelegateGroups()
        {
            var centreId = User.GetCentreIdKnownNotNull();

            var groups = groupsService.GetGroupsForCentre(centreId: centreId).ToList();

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
            var centreId = User.GetCentreIdKnownNotNull();
            if (!groupsService.IsDelegateGroupExist(model.GroupName.Trim(), centreId))
            {
                groupsService.AddDelegateGroup(
                User.GetCentreIdKnownNotNull(),
                model.GroupName!.Trim(),
                model.GroupDescription,
                User.GetAdminId()!.Value
            );
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(nameof(model.GroupName), "Delegate group with the same name already exists");
                return View("AddDelegateGroup", model);
            }
        }

        [HttpGet("{groupId:int}/EditDescription")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult EditDescription(int groupId, ReturnPageQuery returnPageQuery)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var group = groupsService.GetGroupAtCentreById(groupId, centreId);

            var model = new EditDelegateGroupDescriptionViewModel(group!, returnPageQuery);
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

            var centreId = User.GetCentreIdKnownNotNull();
            groupsService.UpdateGroupDescription(
                groupId,
                centreId,
                model.Description
            );

            return RedirectToAction("Index");
        }

        [HttpGet("{groupId:int}/EditGroupName")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult EditGroupName(int groupId, ReturnPageQuery returnPageQuery)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var group = groupsService.GetGroupAtCentreById(groupId, centreId);

            if (group?.LinkedToField != 0)
            {
                return NotFound();
            }

            var model = new EditGroupNameViewModel(group.GroupLabel!, returnPageQuery);
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

            var centreId = User.GetCentreIdKnownNotNull();
            var group = groupsService.GetGroupAtCentreById(groupId, centreId);

            if (group?.LinkedToField != 0)
            {
                return NotFound();
            }

            if ((!groupsService.IsDelegateGroupExist(model.GroupName.Trim(), centreId)) || (group.GroupLabel.Trim() == model.GroupName.Trim()))
            {
                groupsService.UpdateGroupName(
                groupId,
                centreId,
                model.GroupName.Trim()
            );
            }
            else
            {
                ModelState.AddModelError(nameof(model.GroupName), "Delegate group with the same name already exists");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [Route("{groupId:int}/Delete")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult DeleteGroup(int groupId, ReturnPageQuery returnPageQuery)
        {
            var delegates = groupsService.GetGroupDelegates(groupId);
            var courses = groupsService.GetUsableGroupCoursesForCentre(groupId, User.GetCentreIdKnownNotNull());

            if (delegates.Any() || courses.Any())
            {
                return RedirectToAction("ConfirmDeleteGroup", new { groupId, returnPageQuery });
            }

            groupsService.DeleteDelegateGroup(groupId, false);
            return RedirectToAction("Index");
        }

        [HttpGet("{groupId:int}/Delete/Confirm")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult ConfirmDeleteGroup(int groupId, ReturnPageQuery returnPageQuery)
        {
            var groupLabel = groupsService.GetGroupName(groupId, User.GetCentreIdKnownNotNull())!;
            var delegateCount = groupsService.GetGroupDelegates(groupId).Count();
            var courseCount = groupsService.GetUsableGroupCoursesForCentre(groupId, User.GetCentreIdKnownNotNull()).Count();

            var model = new ConfirmDeleteGroupViewModel
            {
                GroupLabel = groupLabel,
                DelegateCount = delegateCount,
                CourseCount = courseCount,
                ReturnPageQuery = returnPageQuery,
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
            var centreId = User.GetCentreIdKnownNotNull();
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
            var centreId = User.GetCentreIdKnownNotNull();

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
