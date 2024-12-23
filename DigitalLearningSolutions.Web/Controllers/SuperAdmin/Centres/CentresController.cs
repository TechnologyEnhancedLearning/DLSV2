﻿using DigitalLearningSolutions.Data.Enums;
using DigitalLearningSolutions.Data.Extensions;
using DigitalLearningSolutions.Data.Helpers;
using DigitalLearningSolutions.Data.Models.Centres;
using DigitalLearningSolutions.Data.Models.Courses;
using DigitalLearningSolutions.Data.Models.Email;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Extensions;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Centres;
using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement.Mvc;
using Org.BouncyCastle.Asn1.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Centres
{
    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Centres))]
    public class CentresController : Controller
    {
        private readonly ICentresService centresService;
        private readonly ICentreApplicationsService centreApplicationsService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IRegionService regionService;
        private readonly IContractTypesService contractTypesService;
        private readonly ICourseService courseService;
        private readonly ICentresDownloadFileService centresDownloadFileService;
        private readonly ICentreSelfAssessmentsService centreSelfAssessmentsService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IConfiguration config;
        private readonly IConfigService configService;


        public CentresController(ICentresService centresService, ICentreApplicationsService centreApplicationsService, ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IRegionService regionService, IContractTypesService contractTypesService, ICourseService courseService, ICentresDownloadFileService centresDownloadFileService,
            ICentreSelfAssessmentsService centreSelfAssessmentsService, IPasswordResetService passwordResetService, IConfiguration config, IConfigService configService)
        {
            this.centresService = centresService;
            this.centreApplicationsService = centreApplicationsService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.regionService = regionService;
            this.contractTypesService = contractTypesService;
            this.courseService = courseService;
            this.centresDownloadFileService = centresDownloadFileService;
            this.centreSelfAssessmentsService = centreSelfAssessmentsService;
            this.passwordResetService = passwordResetService;
            this.config = config;
            this.configService = configService;
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

            search = search == null ? string.Empty : search.Trim();

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
            var modifiedCentres = centres.Select(item =>
            {
                var centreEntity = this.centresService.UpdateCentreWithCounts(item);
                if (centreEntity.Centre.RegisterUser == 0 && centreEntity.Centre.AutoRegisterManagerEmail != null)
                {
                    var baseUrl = config.GetAppRootPath();
                    var supportEmail = this.configService.GetConfigValue("SupportEmail");
                    baseUrl = baseUrl+"/RegisterAdmin?centreId={centreId}".Replace("{centreId}", item.Centre.CentreId.ToString());
                    Email welcomeEmail = this.passwordResetService.GenerateEmailInviteForCentreManager(centreEntity.Centre.CentreName, centreEntity.Centre.AutoRegisterManagerEmail, baseUrl, supportEmail);
                    centreEntity.Centre.EmailInvite = "mailto:" + string.Join(",", welcomeEmail.To) + "?subject=" + welcomeEmail.Subject + "&body=" + welcomeEmail.Body.TextBody.Replace("&", "%26");
                }
                return centreEntity;
            }).ToList();
            centres = modifiedCentres;
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
                result.SearchString = "SearchQuery|" + search.Trim() + "";
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

            var regions = regionService.GetRegionsAlphabetical().ToList();
            regions.Insert(0, (0, "Any"));
            ViewBag.Regions = SelectListHelper.MapOptionsToSelectListItems(
                regions, region
            );

            var centreTypes = this.centresService.GetCentreTypes().ToList();
            centreTypes.Insert(0, (0, "Any"));
            ViewBag.CentreTypes = SelectListHelper.MapOptionsToSelectListItems(
                centreTypes, centreType
            );

            var contractTypes = this.contractTypesService.GetContractTypes().ToList();
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
            Centre centre = centresService.GetFullCentreDetailsById(centreId);
            centre.CandidateByteLimit = centre.CandidateByteLimit / 1048576;
            centre.ServerSpaceBytes = centre.ServerSpaceBytes / 1073741824;
            return View(centre);
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/Courses")]
        public IActionResult Courses(int centreId = 0)
        {
            var courses = this.courseService.GetApplicationsAvailableToCentre(centreId);
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

            ViewBag.CentreName = centresService.GetCentreName(centreId) + "  (" + centreId + ")";
            return View(centreCoursesViewModel);
        }

        [HttpGet]
        [NoCaching]
        [Route("SuperAdmin/Centres/{centreId=0:int}/EditCentreDetails")]
        public IActionResult EditCentreDetails(int centreId = 0)
        {
            var centreDetails = centresService.GetCentreDetailsById(centreId)!;

            var regions = regionService.GetRegionsAlphabetical().ToList();
            ViewBag.Regions = SelectListHelper.MapOptionsToSelectListItems(
                regions, centreDetails.RegionId
            );

            var centreTypes = this.centresService.GetCentreTypes().ToList();
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
            if (!string.IsNullOrEmpty(model.CentreName))
            {
                var centres = centresService.GetAllCentres().ToList();
                bool isExistingCentreName = centres.Where(center => center.Item1 == model.CentreId)
                    .Select(center => center.Item2)
                    .FirstOrDefault()
                    .Equals(model.CentreName.Trim());
                bool isCentreNamePresent = centres.Any(center => string.Equals(center.Item2.Trim(), model.CentreName?.Trim(), StringComparison.OrdinalIgnoreCase));

                if (isCentreNamePresent && !isExistingCentreName)
                {
                    ModelState.AddModelError("CentreName", CommonValidationErrorMessages.CentreNameAlreadyExist);
                }
            }
            if (!ModelState.IsValid)
            {
                var regions = regionService.GetRegionsAlphabetical().ToList();
                ViewBag.Regions = SelectListHelper.MapOptionsToSelectListItems(
                    regions, model.RegionId
                );

                var centreTypes = this.centresService.GetCentreTypes().ToList();
                ViewBag.CentreTypes = SelectListHelper.MapOptionsToSelectListItems(
                    centreTypes, model.CentreTypeId
                );
                model.CentreName = model.CentreName == null ? string.Empty : model.CentreName.Trim();
                model.IpPrefix = model.IpPrefix == null ? string.Empty : model.IpPrefix.Trim();
                return View(model);
            }

            centresService.UpdateCentreDetailsForSuperAdmin(
                model.CentreId,
                model.CentreName.Trim(),
                model.CentreTypeId,
                model.RegionId,
                model.CentreEmail,
                model.IpPrefix?.Trim(),
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
            Centre centre = centresService.GetCentreManagerDetailsByCentreId(centreId);
            EditCentreManagerDetailsViewModel editCentreManagerDetailsViewModel = new EditCentreManagerDetailsViewModel(centre);
            return View(editCentreManagerDetailsViewModel);
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/ManageCentreManager")]
        [HttpPost]
        public IActionResult ManageCentreManager(EditCentreManagerDetailsViewModel editCentreManagerDetailsViewModel)
        {
            editCentreManagerDetailsViewModel.FirstName = editCentreManagerDetailsViewModel.FirstName == null ? string.Empty : editCentreManagerDetailsViewModel.FirstName.Trim();
            editCentreManagerDetailsViewModel.LastName = editCentreManagerDetailsViewModel.LastName == null ? string.Empty : editCentreManagerDetailsViewModel.LastName.Trim();
            editCentreManagerDetailsViewModel.Telephone = editCentreManagerDetailsViewModel.Telephone?.Trim() ?? string.Empty;
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
            ViewBag.CentreName = centresService.GetCentreName(centreId);

            var roleLimits = this.centresService.GetRoleLimitsForCentre(centreId);

            var centreRoleLimitsViewModel = new CentreRoleLimitsViewModel
            {
                CentreId = centreId,
            };

            if (!(roleLimits.RoleLimitCmsAdministrators != null && roleLimits.RoleLimitCmsAdministrators != -1))
            {
                if (roleLimits.RoleLimitCmsAdministrators != -1)
                {
                    centreRoleLimitsViewModel.RoleLimitCmsAdministrators = null;
                }
                else
                {
                    centreRoleLimitsViewModel.RoleLimitCmsAdministrators = -1;
                }
                centreRoleLimitsViewModel.IsRoleLimitSetCmsAdministrators = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitCmsAdministrators = roleLimits.RoleLimitCmsAdministrators.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetCmsAdministrators = true;
            }

            if (!(roleLimits.RoleLimitCmsManagers != null && roleLimits.RoleLimitCmsManagers != -1))
            {
                if (roleLimits.RoleLimitCmsManagers != -1)
                {
                    centreRoleLimitsViewModel.RoleLimitCmsManagers = null;
                }
                else
                {
                    centreRoleLimitsViewModel.RoleLimitCmsManagers = -1;
                }
                centreRoleLimitsViewModel.IsRoleLimitSetCmsManagers = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitCmsManagers = roleLimits.RoleLimitCmsManagers.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetCmsManagers = true;
            }

            if (!(roleLimits.RoleLimitCcLicences != null && roleLimits.RoleLimitCcLicences != -1))
            {
                if (roleLimits.RoleLimitCcLicences != -1)
                {
                    centreRoleLimitsViewModel.RoleLimitContentCreatorLicences = null;
                }
                else
                {
                    centreRoleLimitsViewModel.RoleLimitContentCreatorLicences = -1;
                }
                centreRoleLimitsViewModel.IsRoleLimitSetContentCreatorLicences = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitContentCreatorLicences = roleLimits.RoleLimitCcLicences.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetContentCreatorLicences = true;
            }

            if (!(roleLimits.RoleLimitCustomCourses != null && roleLimits.RoleLimitCustomCourses != -1))
            {
                if (roleLimits.RoleLimitCustomCourses != -1)
                {
                    centreRoleLimitsViewModel.RoleLimitCustomCourses = null;
                }
                else
                {
                    centreRoleLimitsViewModel.RoleLimitCustomCourses = -1;
                }
                centreRoleLimitsViewModel.IsRoleLimitSetCustomCourses = false;
            }
            else
            {
                centreRoleLimitsViewModel.RoleLimitCustomCourses = roleLimits.RoleLimitCustomCourses.Value;
                centreRoleLimitsViewModel.IsRoleLimitSetCustomCourses = true;
            }

            if (!(roleLimits.RoleLimitTrainers != null && roleLimits.RoleLimitTrainers != -1))
            {
                if (roleLimits.RoleLimitTrainers != -1)
                {
                    centreRoleLimitsViewModel.RoleLimitTrainers = null;
                }
                else
                {
                    centreRoleLimitsViewModel.RoleLimitTrainers = -1;
                }
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
            if (!model.IsRoleLimitSetCmsAdministrators)
            {
                model.RoleLimitCmsAdministrators = -1;
                ModelState.Remove("RoleLimitCmsAdministrators");
            }

            if (!model.IsRoleLimitSetCmsManagers)
            {
                model.RoleLimitCmsManagers = -1;
                ModelState.Remove("RoleLimitCmsManagers");
            }

            if (!model.IsRoleLimitSetContentCreatorLicences)
            {
                model.RoleLimitContentCreatorLicences = -1;
                ModelState.Remove("RoleLimitContentCreatorLicences");
            }

            if (!model.IsRoleLimitSetCustomCourses)
            {
                model.RoleLimitCustomCourses = -1;
                ModelState.Remove("RoleLimitCustomCourses");
            }

            if (!model.IsRoleLimitSetTrainers)
            {
                model.RoleLimitTrainers = -1;
                ModelState.Remove("RoleLimitTrainers");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CentreName = centresService.GetCentreName(model.CentreId);
                return View("CentreRoleLimits", model);
            }

            centresService.UpdateCentreRoleLimits(
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
            var regions = regionService.GetRegionsAlphabetical().ToList();
            var centreTypes = this.centresService.GetCentreTypes().ToList();

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
            var centres = centresService.GetAllCentres().ToList();
            bool isCentreNamePresent = centres.Any(center => string.Equals(center.Item2.Trim(), model.CentreName?.Trim(), StringComparison.OrdinalIgnoreCase));
            if (isCentreNamePresent)
            {
                ModelState.AddModelError("CentreName", CommonValidationErrorMessages.CentreNameAlreadyExist);
            }
            if (!ModelState.IsValid)
            {
                var centreTypes = this.centresService.GetCentreTypes().ToList();
                var regions = regionService.GetRegionsAlphabetical().ToList();
                model.RegionNameOptions = SelectListHelper.MapOptionsToSelectListItems(regions, model.RegionId);
                model.CentreTypeOptions = SelectListHelper.MapOptionsToSelectListItems(centreTypes, model.CentreTypeId);
                model.CentreName = model.CentreName?.Trim() ?? string.Empty;
                model.ContactFirstName = model.ContactFirstName?.Trim() ?? string.Empty;
                model.ContactLastName = model.ContactLastName?.Trim() ?? string.Empty;
                model.ContactEmail = model.ContactEmail?.Trim() ?? string.Empty;
                model.ContactPhone = model.ContactPhone?.Trim() ?? string.Empty;
                model.IpPrefix = model.IpPrefix?.Trim() ?? string.Empty;
                return View(model);
            }

            int insertedID = centresService.AddCentreForSuperAdmin(
                model.CentreName.Trim(),
                model.ContactFirstName,
                model.ContactLastName,
                model.ContactEmail,
                model.ContactPhone,
                model.CentreTypeId,
                model.RegionId,
                model.RegistrationEmail,
                model.IpPrefix?.Trim(),
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
            ContractInfo centre = this.centresService.GetContractInfo(centreId);
            var contractTypes = this.contractTypesService.GetContractTypes().ToList();
            var serverspace = this.contractTypesService.GetServerspace();
            var delegatespace = this.contractTypesService.Getdelegatespace();

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
            if ((day.GetValueOrDefault() != 0) || (month.GetValueOrDefault() != 0) || (year.GetValueOrDefault() != 0))
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
                        year
                    });
                }
            }
            if (!ModelState.IsValid)
            {
                ContractInfo centre = this.centresService.GetContractInfo(contractTypeViewModel.CentreId);
                var contractTypes = this.contractTypesService.GetContractTypes().ToList();
                var serverspace = this.contractTypesService.GetServerspace();
                var delegatespace = this.contractTypesService.Getdelegatespace();
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
            if ((day.GetValueOrDefault() != 0) || (month.GetValueOrDefault() != 0) || (year.GetValueOrDefault() != 0))
            {
                date = new DateTime(year ?? 0, month ?? 0, day ?? 0);
            }
            this.centresService.UpdateContractTypeandCenter(contractTypeViewModel.CentreId,
               contractTypeViewModel.ContractTypeID,
               contractTypeViewModel.DelegateUploadSpace,
               contractTypeViewModel.ServerSpaceBytesInc,
               date
               );
            return RedirectToAction("ManageCentre", new { centreId = contractTypeViewModel.CentreId });
        }

        [Route("SuperAdmin/Centres/Export")]
        public IActionResult Export(
            string? searchString = null,
            string? existingFilterString = null
        )
        {
            var content = centresDownloadFileService.GetAllCentresFile(
                searchString,
                existingFilterString
            );

            const string fileName = "DLS Centres Export.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
        [NoCaching]
        [Route("SuperAdmin/Centres/{centreId=0:int}/Courses/{applicationId}/ConfirmRemove")]
        public IActionResult ConfirmRemoveCourse(int centreId = 0, int applicationId = 0)
        {
            var centreApplication = centreApplicationsService.GetCentreApplicationByCentreAndApplicationID(centreId, applicationId);
            if (centreApplication != null)
            {
                var model = new ConfirmRemoveCourseViewModel();
                model.CentreApplication = centreApplication;
                return View("ConfirmRemoveCourse", model);
            }
            else
            {
                return RedirectToAction("Courses", new { centreId });
            }

        }
        public IActionResult RemoveCourse(int centreId = 0, int applicationId = 0)
        {
            centreApplicationsService.DeleteCentreApplicationByCentreAndApplicationID(centreId, applicationId);
            return RedirectToAction("Courses", new { centreId });
        }
        [Route("SuperAdmin/Centres/{centreId=0:int}/Courses/Add")]
        public IActionResult CourseAddChooseFlow(int centreId = 0)
        {
            ViewBag.CentreName = centresService.GetCentreName(centreId) + "  (" + centreId + ")";
            return View();
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/Courses/Add")]
        [HttpPost]
        public IActionResult CourseAddChooseFlow(CourseAddChooseFlowViewModel model)
        {
            return RedirectToAction(nameof(CourseAdd), new { centreId = model.CentreId, courseType = model.AddCourseOption, searchTerm = (model.SearchTerm != null ? model.SearchTerm.Replace(" ", "%") : "") });
        }

        private CourseAddViewModel SetupCommonModel(int centreId, string courseType, IEnumerable<CourseForPublish> courses)
        {
            return new CourseAddViewModel
            {
                CourseType = courseType,
                CentreId = centreId,
                CentreName = $"{centresService.GetCentreName(centreId)} ({centreId})",
                Courses = courses
            };
        }

        [NoCaching]
        [Route("SuperAdmin/Centres/{centreId}/Courses/Add/{courseType}")]
        public IActionResult CourseAdd(int centreId, string courseType, string? searchTerm)
        {
            CourseAddViewModel model;
            switch (courseType)
            {
                case "Core":
                    model = SetupCommonModel(centreId, "Core", centreApplicationsService.GetCentralCoursesForPublish(centreId));
                    break;
                case "Other":
                    model = SetupCommonModel(centreId, "Other", centreApplicationsService.GetOtherCoursesForPublish(centreId, searchTerm));
                    break;
                default:
                    model = SetupCommonModel(centreId, "Pathways", centreApplicationsService.GetPathwaysCoursesForPublish(centreId));
                    break;
            }
            model.SearchTerm = searchTerm;
            return View("CourseAdd", model);
        }

        [HttpPost]
        [Route("SuperAdmin/Centres/{centreId}/Courses/Add/{courseType}")]
        public IActionResult CourseAddCommit(CourseAddViewModel model, int centreId, string courseType)
        {
            if (!ModelState.IsValid)
            {
                switch (courseType)
                {
                    case "Core":
                        model.Courses = centreApplicationsService.GetCentralCoursesForPublish(centreId);
                        break;
                    case "Other":
                        model.Courses = centreApplicationsService.GetOtherCoursesForPublish(centreId, model.SearchTerm);
                        break;
                    default:
                        model.Courses = centreApplicationsService.GetPathwaysCoursesForPublish(centreId);
                        break;
                }
                model.CentreName = centresService.GetCentreName(centreId);
                return View("CourseAdd", model);
            }
            foreach (var id in model.ApplicationIds)
            {
                centreApplicationsService.InsertCentreApplication(model.CentreId, id);
            }
            return RedirectToAction("Courses", new { centreId = model.CentreId });
        }

        [Route("SuperAdmin/Centres/{centreId=0:int}/SelfAssessments")]
        public IActionResult SelfAssessments(int centreId = 0)
        {
            var selfAssessments = centreSelfAssessmentsService.GetCentreSelfAssessments(centreId);
            var model = new CentreSelfAssessmentsViewModel() { CentreSelfAssessments = selfAssessments };
            ViewBag.CentreName = centresService.GetCentreName(centreId) + "  (" + centreId + ")";
            return View(model);
        }
        [NoCaching]
        [Route("SuperAdmin/Centres/{centreId=0:int}/SelfAssessments/{selfAssessmentId}/ConfirmRemove")]
        public IActionResult ConfirmRemoveSelfAssessment(int centreId = 0, int selfAssessmentId = 0)
        {
            var centreSelfAssessment = centreSelfAssessmentsService.GetCentreSelfAssessmentByCentreAndID(centreId, selfAssessmentId);
            if (centreSelfAssessment != null)
            {
                var model = new ConfirmRemoveSelfAssessmentViewModel();
                model.CentreSelfAssessment = centreSelfAssessment;
                return View("ConfirmRemoveSelfAssessment", model);
            }
            else
            {
                return RedirectToAction("SelfAssessments", new { centreId });
            }

        }
        public IActionResult RemoveSelfAssessment(int centreId = 0, int selfAssessmentId = 0)
        {
            centreSelfAssessmentsService.DeleteCentreSelfAssessment(centreId, selfAssessmentId);
            return RedirectToAction("SelfAssessments", new { centreId });
        }

        [HttpGet]
        [Route("SuperAdmin/Centres/{centreId}/SelfAssessments/Add")]
        public IActionResult SelfAssessmentAdd(int centreId = 0)
        {
            var selfAssessmentsForPublish = centreSelfAssessmentsService.GetCentreSelfAssessmentsForPublish(centreId);
            var centreName = centresService.GetCentreName(centreId) + "  (" + centreId + ")";
            var model = new SelfAssessmentAddViewModel() { SelfAssessments = selfAssessmentsForPublish, CentreId = centreId, CentreName = centreName, SelfAssessmentIds = new List<int>() };
            return View(model);
        }

        [HttpPost]
        [Route("SuperAdmin/Centres/{centreId}/SelfAssessments/Add")]
        public IActionResult SelfAssessmentAddSubmit(int centreId, SelfAssessmentAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var selfAssessmentsForPublish = centreSelfAssessmentsService.GetCentreSelfAssessmentsForPublish(centreId);
                var centreName = centresService.GetCentreName(centreId) + "  (" + centreId + ")";
                model.SelfAssessmentIds = model.SelfAssessmentIds ?? new List<int>();
                model.CentreName = centreName;
                model.SelfAssessments = selfAssessmentsForPublish;
                return View("SelfAssessmentAdd", model);
            }
            var selfEnrol = model.EnableSelfEnrolment;
            if (selfEnrol != null)
            {
                foreach (var id in model.SelfAssessmentIds)
                {
                    centreSelfAssessmentsService.InsertCentreSelfAssessment(model.CentreId, id, (bool)selfEnrol);
                }
            }

            return RedirectToAction("SelfAssessments", new { centreId = model.CentreId });
        }
    }
}
