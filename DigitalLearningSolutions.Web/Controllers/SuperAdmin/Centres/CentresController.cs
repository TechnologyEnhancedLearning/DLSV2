namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres
{
    using DigitalLearningSolutions.Data.DataServices;
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
          string? Search = "",
          int? itemsPerPage = 10,
          int Region = 0,
          int CentreType = 0,
          int ContractType = 0,
          string CentreStatus = "",
          string? SearchString = "",
          string? ExistingFilterString = ""
        )
        {
            if (string.IsNullOrEmpty(SearchString) && string.IsNullOrEmpty(ExistingFilterString))
            {
                page = 1;
            }

            int offSet = ((page - 1) * itemsPerPage) ?? 0;
            CentreStatus = (string.IsNullOrEmpty(CentreStatus) ? "Any" : CentreStatus);

            if (!string.IsNullOrEmpty(SearchString))
            {
                List<string> searchFilters = SearchString.Split("-").ToList();
                if (searchFilters.Count == 1)
                {
                    string searchFilter = searchFilters[0];
                    if (searchFilter.Contains("SearchQuery|"))
                    {
                        Search = searchFilter.Split("|")[1];
                    }
                }
            }
            if (!string.IsNullOrEmpty(ExistingFilterString))
            {
                List<string> selectedFilters = ExistingFilterString.Split("-").ToList();
                if (selectedFilters.Count == 4)
                {
                    string regionFilter = selectedFilters[0];
                    if (regionFilter.Contains("Region|"))
                    {
                        Region = Convert.ToInt16(regionFilter.Split("|")[1]);
                    }

                    string centreTypeFilter = selectedFilters[1];
                    if (centreTypeFilter.Contains("CentreType|"))
                    {
                        CentreType = Convert.ToInt16(centreTypeFilter.Split("|")[1]);
                    }

                    string contractTypeFilter = selectedFilters[2];
                    if (contractTypeFilter.Contains("ContractType|"))
                    {
                        ContractType = Convert.ToInt16(contractTypeFilter.Split("|")[1]);
                    }

                    string centreStatusFilter = selectedFilters[3];
                    if (centreStatusFilter.Contains("CentreStatus|"))
                    {
                        CentreStatus = centreStatusFilter.Split("|")[1];
                    }
                }
            }
            (var Centres, var ResultCount) = this.centresService.GetAllCentreSummariesForSuperAdmin(Search ?? string.Empty, offSet, itemsPerPage ?? 10, Region, CentreType, ContractType, CentreStatus);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(GenericSortingHelper.DefaultSortOption, GenericSortingHelper.Ascending),
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                Centres,
                searchSortPaginationOptions
            );
            result.Page = page;
            if (
                !string.IsNullOrEmpty(Search) ||
                Region != 0 ||
                CentreType != 0 ||
                ContractType != 0 ||
                !string.IsNullOrEmpty(CentreStatus)
            )
            {
                result.SearchString = "SearchQuery|" + Search + "";
                result.FilterString = "Region|" + Region + "-CentreType|" + CentreType + "-ContractType|" + ContractType + "-CentreStatus|" + CentreStatus;
            }

            var model = new CentresViewModel(result);
            model.TotalPages = (int)(ResultCount / itemsPerPage) + ((ResultCount % itemsPerPage) > 0 ? 1 : 0);
            model.MatchingSearchResults = ResultCount;
            model.Search = Search;
            model.Region = Region;
            model.ContractType = ContractType;
            model.CentreStatus = CentreStatus;
            model.CentreType = CentreType;
            model.JavascriptSearchSortFilterPaginateEnabled = false;

            var regions = regionDataService.GetRegionsAlphabetical().ToList();
            regions.Insert(0, (0, "Any"));
            ViewBag.Regions = SelectListHelper.MapOptionsToSelectListItems(
                regions, Region
            );

            var centreTypes = this.centresDataService.GetCentreTypes().ToList();
            centreTypes.Insert(0, (0, "Any"));
            ViewBag.CentreTypes = SelectListHelper.MapOptionsToSelectListItems(
                centreTypes, CentreType
            );

            var contractTypes = this.contractTypesDataService.GetContractTypes().ToList();
            contractTypes.Insert(0, (0, "Any"));
            ViewBag.ContractTypes = SelectListHelper.MapOptionsToSelectListItems(
                contractTypes, ContractType
            );

            ViewBag.CentreStatus = SelectListHelper.MapOptionsToSelectListItems(
                GetCentreStatus(), CentreStatus
            );
            ModelState.ClearAllErrors();
            return View(model);
        }
        public List<string> GetCentreStatus()
        {
            return new List<string>(new string[] { "Any", "Active", "Inactive" });
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
            List<CentreCoursesViewModel> centreCoursesViewModel = new List<CentreCoursesViewModel>();
            centreCoursesViewModel = courses.GroupBy(x => x.ApplicationId).Select(
                application => new CentreCoursesViewModel
                {
                    ApplicationID = application.FirstOrDefault().ApplicationId,
                    ApplicationName = application.FirstOrDefault().ApplicationName,
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

        [Route("SuperAdmin/Centres/{centreId=0:int}/CentreRoleLimits")]
        public IActionResult CentreRoleLimits(int centreId = 0)
        {
            //var courses = this.courseDataService.GetApplicationsAvailableToCentre(centreId);
            //List<CentreCoursesViewModel> centreCoursesViewModel = new List<CentreCoursesViewModel>();
            //centreCoursesViewModel = courses.GroupBy(x => x.ApplicationId).Select(
            //    application => new CentreCoursesViewModel
            //    {
            //        ApplicationID = application.FirstOrDefault().ApplicationId,
            //        ApplicationName = application.FirstOrDefault().ApplicationName,
            //        CentreCourseCustomisations = application.Select(courseCustomisation => new CentreCourseCustomisation
            //        {
            //            CustomisationID = courseCustomisation.CustomisationId,
            //            CustomisationName = courseCustomisation.CustomisationName,
            //            DelegateCount = courseCustomisation.DelegateCount
            //        }).ToList()
            //    }).ToList();

            ViewBag.CentreName = centresDataService.GetCentreName(centreId);

            // var setLimits = this.courseDataService.GetSetLimitsForCentre(centreId);



            CentreRoleLimitsViewModel centreRoleLimitsViewModel = new CentreRoleLimitsViewModel();

            centreRoleLimitsViewModel.CentreId = centreId;
            centreRoleLimitsViewModel.IsLimitSetCMSAdministrators = false;
            centreRoleLimitsViewModel.LimitSetCMSAdministrators = 0;
            centreRoleLimitsViewModel.IsLimitSetCMSManagers = false;
            centreRoleLimitsViewModel.LimitSetCMSManagers = 0;
            centreRoleLimitsViewModel.IsLimitSetContentCreatorLicenses = false;
            centreRoleLimitsViewModel.LimitSetContentCreatorLicenses = 0;
            centreRoleLimitsViewModel.IsLimitSetCustomCourses = false;
            centreRoleLimitsViewModel.LimitSetCustomCourses = 0;
            centreRoleLimitsViewModel.IsLimitSetTrainers = false;
            centreRoleLimitsViewModel.LimitSetTrainers = 0;

            return View("CentreRoleLimits", centreRoleLimitsViewModel);
        }

        //TODO: Post save method here


    }
}
