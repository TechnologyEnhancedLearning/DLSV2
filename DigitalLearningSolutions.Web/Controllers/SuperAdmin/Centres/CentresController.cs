﻿using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Helpers;
using DigitalLearningSolutions.Data.Models.Centres;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Extensions;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.CentreCourses;
using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Centres))]
    public class CentresController : Controller
    {
        private readonly ICentresService centresService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IRegionDataService regionDataService;
        private readonly ICentresDataService centresDataService;
        private readonly IContractTypesDataService contractTypesDataService;
        private readonly ICourseDataService courseDataService;
        public CentresController(ICentresService centresService, ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IRegionDataService regionDataService, ICentresDataService centresDataService, IContractTypesDataService contractTypesDataService, ICourseDataService courseDataService)
        {
            this.centresService = centresService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.regionDataService = regionDataService;
            this.centresDataService = centresDataService;
            this.contractTypesDataService = contractTypesDataService;
            this.courseDataService = courseDataService;
        }

        [Route("SuperAdmin/Centres/{page=0:int}")]
        public IActionResult Index(
          int page = 1,
          string? search = "",
          int? itemsPerPage = 10,
          int region = 0,
          int centreType = 0,
          int contractType = 0,
          string centreStatus = "",
          string? searchString = "",
          string? existingFilterString = ""
        )
        {
            if (string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(existingFilterString))
            {
                page = 1;
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            centreStatus = (string.IsNullOrEmpty(centreStatus) ? "Any" : centreStatus);

            if (!string.IsNullOrEmpty(searchString))
            {
                List<string> searchFilters = searchString.Split("-").ToList();
                if (searchFilters.Count == 1)
                {
                    string searchFilter = searchFilters[0];
                    if (searchFilter.Contains("SearchQuery|"))
                    {
                        search = searchFilter.Split("|")[1];
                    }
                }
            }
            if (!string.IsNullOrEmpty(existingFilterString))
            {
                List<string> selectedFilters = existingFilterString.Split("-").ToList();
                if (selectedFilters.Count == 4)
                {
                    string regionFilter = selectedFilters[0];
                    if (regionFilter.Contains("Region|"))
                    {
                        region = Convert.ToInt16(regionFilter.Split("|")[1]);
                    }

                    string centreTypeFilter = selectedFilters[1];
                    if (centreTypeFilter.Contains("CentreType|"))
                    {
                        centreType = Convert.ToInt16(centreTypeFilter.Split("|")[1]);
                    }

                    string contractTypeFilter = selectedFilters[2];
                    if (contractTypeFilter.Contains("ContractType|"))
                    {
                        contractType = Convert.ToInt16(contractTypeFilter.Split("|")[1]);
                    }

                    string centreStatusFilter = selectedFilters[3];
                    if (centreStatusFilter.Contains("CentreStatus|"))
                    {
                        centreStatus = centreStatusFilter.Split("|")[1];
                    }
                }
            }
            (var centres, var resultCount) = this.centresService.GetAllCentreSummariesForSuperAdmin(search ?? string.Empty, offSet, itemsPerPage ?? 10, region, centreType, contractType, centreStatus);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                centres,
                searchSortPaginationOptions
            );
            result.Page = page;
            if (
                !string.IsNullOrEmpty(search) ||
                region != 0 ||
                centreType != 0 ||
                contractType != 0 ||
                !string.IsNullOrEmpty(centreStatus)
            )
            {
                result.SearchString = "SearchQuery|" + search + "";
                result.FilterString = "Region|" + region + "-CentreType|" + centreType + "-ContractType|" + contractType + "-CentreStatus|" + centreStatus;
            }

            var model = new CentresViewModel(result);
            model.TotalPages = (int)(resultCount / itemsPerPage) + ((resultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = resultCount;
            model.Search = search;
            model.Region = region;
            model.ContractType = contractType;
            model.CentreStatus = centreStatus;
            model.CentreType = centreType;
            model.JavascriptSearchSortFilterPaginateEnabled = false;

            var regions = regionDataService.GetRegionsAlphabetical().ToList();
            regions.Insert(0, (0, "Any"));
            ViewBag.Regions = SelectListHelper.MapOptionsToSelectListItems(
                regions, region
            );

            var centreTypes = this.centresDataService.GetCentreTypes().ToList();
            centreTypes.Insert(0, (0, "Any"));
            ViewBag.CentreTypes = SelectListHelper.MapOptionsToSelectListItems(
                centreTypes, centreType
            );

            var contractTypes = this.contractTypesDataService.GetContractTypes().ToList();
            contractTypes.Insert(0, (0, "Any"));
            ViewBag.ContractTypes = SelectListHelper.MapOptionsToSelectListItems(
                contractTypes, contractType
            );

            ViewBag.CentreStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetCentreStatus(), centreStatus
            );
            ModelState.ClearAllErrors();
            return View(model);
        }
        public List<string> GetCentreStatus()
        {
            return new List<string>(new[] { "Any", "Active", "Inactive" });
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/Manage")]
        public IActionResult ManageCentre(int centreId = 0)
        {
            Centre centre = centresDataService.GetFullCentreDetailsById(centreId);
            return View(centre);
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/Courses")]
        public IActionResult Courses(int centreId = 0)
        {
            var courses = this.courseDataService.GetApplicationsAvailableToCentre(centreId);
            List<CentreCoursesViewModel> centreCoursesViewModel;
            centreCoursesViewModel = courses.GroupBy(x => x.ApplicationId).Select(
                application => new CentreCoursesViewModel
                {
                    ApplicationID = application.FirstOrDefault()!.ApplicationId,
                    ApplicationName = application.FirstOrDefault()!.ApplicationName,
                    CentreCourseCustomisations = application.Select(courseCustomisation => new CentreCourseCustomisation
                    {
                        CustomisationID = courseCustomisation.CustomisationId,
                        CustomisationName = courseCustomisation.CustomisationName,
                        DelegateCount = courseCustomisation.DelegateCount
                    }).ToList()
                }).ToList();

            ViewBag.CentreName = centresDataService.GetCentreName(centreId) + "  (" + centreId + ")";
            return View(centreCoursesViewModel);
        }

        [HttpGet]
        [NoCaching]
        [Route("SuperAdmin/Centres/{centreId=0:int}/EditCentreDetails")]
        public IActionResult EditCentreDetails(int centreId = 0)
        {
            var centreDetails = centresDataService.GetCentreDetailsById(centreId)!;

            var regions = regionDataService.GetRegionsAlphabetical().ToList();
            ViewBag.Regions = SelectListHelper.MapOptionsToSelectListItems(
                regions, centreDetails.RegionId
            );

            var centreTypes = this.centresDataService.GetCentreTypes().ToList();
            ViewBag.CentreTypes = SelectListHelper.MapOptionsToSelectListItems(
                centreTypes, centreDetails.CentreTypeId
            );

            var model = new EditCentreDetailsSuperAdminViewModel(centreDetails);

            return View(model);
        }

        [HttpPost]
        [Route("SuperAdmin/Centres/{centreId=0:int}/EditCentreDetails")]
        public IActionResult EditCentreDetails(EditCentreDetailsSuperAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var regions = regionDataService.GetRegionsAlphabetical().ToList();
                ViewBag.Regions = SelectListHelper.MapOptionsToSelectListItems(
                    regions, model.RegionId
                );

                var centreTypes = this.centresDataService.GetCentreTypes().ToList();
                ViewBag.CentreTypes = SelectListHelper.MapOptionsToSelectListItems(
                    centreTypes, model.CentreTypeId
                );
                return View(model);
            }

            centresDataService.UpdateCentreDetailsForSuperAdmin(
                model.CentreId,
                model.CentreName,
                model.CentreTypeId,
                model.RegionId,
                model.CentreEmail,
                model.IpPrefix,
                model.ShowOnMap
            );
            return RedirectToAction("ManageCentre", "Centres", new { centreId = model.CentreId });
        }
        
        [Route("SuperAdmin/Centres/{centreId=0:int}/DeactivateCentre")]
        public IActionResult DeactivateCentre(int centreId = 0)
        {
            this.centresService.DeactivateCentre(centreId);
            return RedirectToAction("Index", "Centres");
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/ReactivateCentre")]
        public IActionResult ReactivateCentre(int centreId = 0)
        {
            this.centresService.ReactivateCentre(centreId);
            return RedirectToAction("Index", "Centres");
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/ManageCentreManager")]
        public IActionResult ManageCentreManager(int centreId = 0)
        {
            Centre centre = centresDataService.GetCentreManagerDetailsByCentreId(centreId);
            EditCentreManagerDetailsViewModel editCentreManagerDetailsViewModel = new EditCentreManagerDetailsViewModel(centre);
            return View(editCentreManagerDetailsViewModel);
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/ManageCentreManager")]
        [HttpPost]
        public IActionResult ManageCentreManager(EditCentreManagerDetailsViewModel editCentreManagerDetailsViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editCentreManagerDetailsViewModel);
            }
            centresService.UpdateCentreManagerDetails(editCentreManagerDetailsViewModel.CentreId, editCentreManagerDetailsViewModel.FirstName, editCentreManagerDetailsViewModel.LastName,
                editCentreManagerDetailsViewModel.Email,
                editCentreManagerDetailsViewModel.Telephone);
            return RedirectToAction("ManageCentre", "Centres", new { centreId = editCentreManagerDetailsViewModel.CentreId });
        }

        [HttpGet]
        [NoCaching]
        [Route("SuperAdmin/Centres/{centreId=0:int}/CentreRoleLimits")]
        public IActionResult CentreRoleLimits(int centreId = 0)
        {
            ViewBag.CentreName = centresDataService.GetCentreName(centreId);

            var roleLimits = this.centresDataService.GetRoleLimitsForCentre(centreId);

            var centreRoleLimitsViewModel = new CentreRoleLimitsViewModel
            {
                CentreId = centreId,
            };

            if (!(roleLimits.RoleLimitCmsAdministrators != null && roleLimits.RoleLimitCmsAdministrators != -1))
            {
                centreRoleLimitsViewModel.IsRoleLimitSetCmsAdministrators = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitCmsAdministrators = roleLimits.RoleLimitCmsAdministrators.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetCmsAdministrators = true;
            }

            if (!(roleLimits.RoleLimitCmsManagers != null && roleLimits.RoleLimitCmsManagers != -1))
            {
                centreRoleLimitsViewModel.IsRoleLimitSetCmsManagers = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitCmsManagers = roleLimits.RoleLimitCmsManagers.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetCmsManagers = true;
            }

            if (!(roleLimits.RoleLimitCcLicences != null && roleLimits.RoleLimitCcLicences != -1))
            {
                centreRoleLimitsViewModel.IsRoleLimitSetContentCreatorLicences = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitContentCreatorLicences = roleLimits.RoleLimitCcLicences.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetContentCreatorLicences = true;
            }

            if (!(roleLimits.RoleLimitCustomCourses != null && roleLimits.RoleLimitCustomCourses != -1))
            {
                centreRoleLimitsViewModel.IsRoleLimitSetCustomCourses = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitCustomCourses = roleLimits.RoleLimitCustomCourses.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetCustomCourses = true;
            }

            if (!(roleLimits.RoleLimitTrainers != null && roleLimits.RoleLimitTrainers != -1))
            {
                centreRoleLimitsViewModel.IsRoleLimitSetTrainers = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitTrainers = roleLimits.RoleLimitTrainers.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetTrainers = true;
            }

            return View("CentreRoleLimits", centreRoleLimitsViewModel);
        }

        [HttpPost]
        [Route("SuperAdmin/Centres/{centreId=0:int}/CentreRoleLimits")]
        public IActionResult EditCentreRoleLimits(CentreRoleLimitsViewModel model)
        {
            if (model.IsRoleLimitSetCmsAdministrators && model.RoleLimitCmsAdministrators == null)
            {
                ModelState["RoleLimitCmsAdministrators.RoleLimitCmsAdministrators"]?.Errors.Clear();
            }
            if (!model.IsRoleLimitSetCmsAdministrators)
            {
                model.RoleLimitCmsAdministrators = -1;
            }
            model.RoleLimitCmsAdministrators ??= -1;

            if (model.IsRoleLimitSetCmsManagers && model.RoleLimitCmsManagers == null)
            {
                ModelState["RoleLimitCmsManagers.RoleLimitCmsManagers"]?.Errors.Clear();
            }
            if (!model.IsRoleLimitSetCmsManagers)
            {
                model.RoleLimitCmsManagers = -1;
            }
            model.RoleLimitCmsManagers ??= -1;

            if (model.IsRoleLimitSetContentCreatorLicences && model.RoleLimitContentCreatorLicences == null)
            {
                ModelState["RoleLimitContentCreatorLicences.RoleLimitContentCreatorLicences"]?.Errors.Clear();
            }
            if (!model.IsRoleLimitSetContentCreatorLicences)
            {
                model.RoleLimitContentCreatorLicences = -1;
            }
            model.RoleLimitContentCreatorLicences ??= -1;

            if (model.IsRoleLimitSetCustomCourses && model.RoleLimitCustomCourses == null)
            {
                ModelState["RoleLimitCustomCourses.RoleLimitCustomCourses"]?.Errors.Clear();
            }
            if (!model.IsRoleLimitSetCustomCourses)
            {
                model.RoleLimitCustomCourses = -1;
            }
            model.RoleLimitCustomCourses ??= -1;

            if (model.IsRoleLimitSetTrainers && model.RoleLimitTrainers == null)
            {
                ModelState["RoleLimitTrainers.RoleLimitTrainers"]?.Errors.Clear();
            }
            if (!model.IsRoleLimitSetTrainers)
            {
                model.RoleLimitTrainers = -1;
            }
            model.RoleLimitTrainers ??= -1;

            if (!ModelState.IsValid)
            {
                return View("CentreRoleLimits", model);
            }

            centresDataService.UpdateCentreRoleLimits(
                model.CentreId,
                model.RoleLimitCmsAdministrators,
                model.RoleLimitCmsManagers,
                model.RoleLimitContentCreatorLicences,
                model.RoleLimitCustomCourses,
                model.RoleLimitTrainers
            );

            return RedirectToAction("ManageCentre", "Centres", new { centreId = model.CentreId });
        }

        [Route("SuperAdmin/Centres/AddCentre")]
        public IActionResult AddCentre()
        {
            var regions = regionDataService.GetRegionsAlphabetical().ToList();
            var centreTypes = this.centresDataService.GetCentreTypes().ToList();

            var addCentreSuperAdminViewModel = new AddCentreSuperAdminViewModel();
            addCentreSuperAdminViewModel.IpPrefix = "194.176.105";

            addCentreSuperAdminViewModel.RegionNameOptions = SelectListHelper.MapOptionsToSelectListItems(regions);
            addCentreSuperAdminViewModel.CentreTypeOptions = SelectListHelper.MapOptionsToSelectListItems(centreTypes);

            return View(addCentreSuperAdminViewModel);
        }

        [HttpPost]
        [Route("SuperAdmin/Centres/AddCentre")]
        public IActionResult AddCentre(AddCentreSuperAdminViewModel model)
        {
            var centres = centresDataService.GetAllCentres().ToList();
            bool isCentreNamePresent = centres.Any(center => center.Item2 == model.CentreName);
            if (isCentreNamePresent)
            {
                ModelState.AddModelError("CentreName", CommonValidationErrorMessages.CentreNameAlreadyExist);
            }
            if (!ModelState.IsValid)
            {
                var centreTypes = this.centresDataService.GetCentreTypes().ToList();
                var regions = regionDataService.GetRegionsAlphabetical().ToList();
                model.RegionNameOptions = SelectListHelper.MapOptionsToSelectListItems(regions, model.RegionId);
                model.CentreTypeOptions = SelectListHelper.MapOptionsToSelectListItems(centreTypes, model.CentreTypeId);
                return View(model);
            }

            int insertedID = centresDataService.AddCentreForSuperAdmin(
                model.CentreName,
                model.ContactFirstName,
                model.ContactLastName,
                model.ContactEmail,
                model.ContactPhone,
                model.CentreTypeId,
                model.RegionId,
                model.RegistrationEmail,
                model.IpPrefix,
                model.ShowOnMap,
                model.AddITSPcourses
            );

            return RedirectToAction("ManageCentre", "Centres", new { centreId = insertedID });
        }

        [HttpGet]
        [NoCaching]
        [Route("SuperAdmin/Centres/{centreId=0:int}/EditContractInfo")]
        public IActionResult EditContractInfo(int centreId, int? day, int? month, int? year, int? ContractTypeID, long? ServerSpaceBytesInc, long? DelegateUploadSpace)
        {
            ContractInfo centre = this.centresDataService.GetContractInfo(centreId);
            var contractTypes = this.contractTypesDataService.GetContractTypes().ToList();
            var serverspace = this.contractTypesDataService.GetServerspace();
            var delegatespace = this.contractTypesDataService.Getdelegatespace();

            var model = new ContractTypeViewModel(centre.CentreID, centre.CentreName,
            centre.ContractTypeID, centre.ContractType,
                centre.ServerSpaceBytesInc, centre.DelegateUploadSpace,
                centre.ContractReviewDate, day, month, year);
            model.ServerSpaceOptions = SelectListHelper.MapLongOptionsToSelectListItems(
                serverspace, ServerSpaceBytesInc ?? centre.ServerSpaceBytesInc
            );
            model.PerDelegateUploadSpaceOptions = SelectListHelper.MapLongOptionsToSelectListItems(
                delegatespace, DelegateUploadSpace ?? centre.DelegateUploadSpace
            );
            model.ContractTypeOptions = SelectListHelper.MapOptionsToSelectListItems(
                contractTypes, ContractTypeID ?? centre.ContractTypeID
            );
            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = OldDateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }
            return View(model);
        }


        [Route("SuperAdmin/Centres/{centreId=0:int}/EditContractInfo")]
        [HttpPost]
        public IActionResult EditContractInfo(ContractTypeViewModel contractTypeViewModel, int? day, int? month, int? year)
        {
            if ((day != 0 && day != null) | (month != 0 && month != null) | (year != 0 && year != null))
            {
                var validationResult = DateValidator.ValidateDate(day ?? 0, month ?? 0, year ?? 0);
                if (validationResult.ErrorMessage != null)
                {
                    if (day == null) day = 0;
                    if (month == null) month = 0;
                    if (year == null) year = 0;
                    return RedirectToAction("EditContractInfo", new
                    {
                        contractTypeViewModel.CentreId,
                        day,
                        month,
                        year,
                        contractTypeViewModel.ContractTypeID,
                        contractTypeViewModel.ServerSpaceBytesInc,
                        contractTypeViewModel.DelegateUploadSpace
                    });
                }
            }
            if (!ModelState.IsValid)
            {
                ContractInfo centre = this.centresDataService.GetContractInfo(contractTypeViewModel.CentreId);
                var contractTypes = this.contractTypesDataService.GetContractTypes().ToList();
                var serverspace = this.contractTypesDataService.GetServerspace();
                var delegatespace = this.contractTypesDataService.Getdelegatespace();

                var model = new ContractTypeViewModel(centre.CentreID, centre.CentreName,
                centre.ContractTypeID, centre.ContractType,
                centre.ServerSpaceBytesInc, centre.DelegateUploadSpace,
                centre.ContractReviewDate, day, month, year);
                model.ServerSpaceOptions = SelectListHelper.MapLongOptionsToSelectListItems(
               serverspace, model.ServerSpaceBytesInc
           );
                model.PerDelegateUploadSpaceOptions = SelectListHelper.MapLongOptionsToSelectListItems(
                    delegatespace, model.DelegateUploadSpace
                );
                model.ContractTypeOptions = SelectListHelper.MapOptionsToSelectListItems(
                    contractTypes, model.ContractTypeID
                );
                return View(model);
            }
            DateTime? date = null;
            if ((day != 0 && day != null) && (month != 0 && month != null) && (year != 0 && year != null))
            {
                date = new DateTime(year ?? 0, month ?? 0, day ?? 0);
            }
            this.centresDataService.UpdateContractTypeandCenter(contractTypeViewModel.CentreId,
               contractTypeViewModel.ContractTypeID,
               contractTypeViewModel.DelegateUploadSpace,
               contractTypeViewModel.ServerSpaceBytesInc,
               date
               );
            return RedirectToAction("ManageCentre", new { centreId = contractTypeViewModel.CentreId });
        }
    }
}
