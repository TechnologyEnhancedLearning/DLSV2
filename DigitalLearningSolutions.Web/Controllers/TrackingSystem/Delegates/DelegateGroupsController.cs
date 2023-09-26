﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Centres;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using DocumentFormat.OpenXml.Spreadsheet;
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

        [Route("{page=0:int}")]
        public IActionResult Index(
          int page = 1,
          string? search = "",
          int? itemsPerPage = 10,
          string? searchString = "",
          string? existingFilterString = ""
        )
        {
            if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(existingFilterString))
            {
                page = 1;
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;

            //if (!string.IsNullOrEmpty(SearchString))
            //{
            //    List<string> searchFilters = SearchString.Split("-").ToList();
            //    if (searchFilters.Count == 2)
            //    {
            //        string searchFilter = searchFilters[0];
            //        if (searchFilter.Contains("SearchQuery|"))
            //        {
            //            Search = searchFilter.Split("|")[1];
            //        }

            //        string userIdFilter = searchFilters[1];
            //        if (userIdFilter.Contains("UserId|"))
            //        {
            //            UserId = Convert.ToInt32(userIdFilter.Split("|")[1]);
            //        }
            //    }
            //}

            //if (!string.IsNullOrEmpty(ExistingFilterString))
            //{
            //    List<string> selectedFilters = ExistingFilterString.Split("-").ToList();
            //    if (selectedFilters.Count == 3)
            //    {
            //        string userStatusFilter = selectedFilters[0];
            //        if (userStatusFilter.Contains("UserStatus|"))
            //        {
            //            UserStatus = userStatusFilter.Split("|")[1];
            //        }

            //        string emailStatusFilter = selectedFilters[1];
            //        if (emailStatusFilter.Contains("EmailStatus|"))
            //        {
            //            EmailStatus = emailStatusFilter.Split("|")[1];
            //        }

            //        string jobGroupFilter = selectedFilters[2];
            //        if (jobGroupFilter.Contains("JobGroup|"))
            //        {
            //            JobGroupId = Convert.ToInt16(jobGroupFilter.Split("|")[1]);
            //        }
            //    }
            //}

            var centreId = User.GetCentreIdKnownNotNull();

            (var groups, var resultCount) = groupsService.GetGroupsForCentrePaginated(
                search: searchString ?? string.Empty,
                offSet,
                rows: itemsPerPage ?? 0,
                centreId: centreId);

            var registrationPrompts = GetRegistrationPromptsWithSetOptions(centreId);
            var availableFilters = DelegateGroupsViewModelFilterOptions
                .GetDelegateGroupFilterModels(groups.ToList(), registrationPrompts).ToList();

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                groups,
                searchSortPaginationOptions
            );

            result.Page = page;
            if (
                !string.IsNullOrEmpty(searchString) ||
                centreId != 0
            )
            {
                // result.SearchString = "SearchQuery|" + search + "-UserId|" + UserId;
                // result.FilterString = "UserStatus|" + UserStatus + "-EmailStatus|" + EmailStatus + "-JobGroup|" + JobGroupId;

                TempData["SearchString"] = result.SearchString;
                TempData["FilterString"] = result.FilterString;
            }
            TempData["Page"] = result.Page;

            var model = new DelegateGroupsViewModel(
                result,
                null
            );

            //var model = new DelegateGroupsViewModel(
            //    result,
            //    availableFilters
            //);

            model.TotalPages = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = resultCount;
            //model.centreId = CentreId == 0 ? null : CentreId;
            //model.Search = search;

            model.JavascriptSearchSortFilterPaginateEnabled = false;
            //ModelState.ClearAllErrors();

            ViewBag.CentreId = TempData["CentreId"];

            Response.UpdateFilterCookie(DelegateGroupsFilterCookieName, result.FilterString);

            return View(model);
        }

        [Route("AllDelegateGroups")]
        public IActionResult AllDelegateGroups()
        {
            var centreId = User.GetCentreIdKnownNotNull();

            var groups = groupsService.GetGroupsForCentre(centreId: centreId).ToList();

            //var groups = groupsService.GetGroupsForCentrePaginated(
            //    search: "",
            //    offset: 0,
            //    rows: 10,
            //    centreId: centreId).ToList();

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
                User.GetCentreIdKnownNotNull(),
                model.GroupName!,
                model.GroupDescription,
                User.GetAdminId()!.Value
            );
            return RedirectToAction("Index");
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

            groupsService.UpdateGroupName(
                groupId,
                centreId,
                model.GroupName
            );

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
