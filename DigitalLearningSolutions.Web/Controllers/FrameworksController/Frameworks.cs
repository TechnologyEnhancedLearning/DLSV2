using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using DigitalLearningSolutions.Data.Models.SessionData.Frameworks;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Data.Constants;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using GDS.MultiPageFormData.Enums;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using System.Net;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using System;

    public partial class FrameworksController
    {
        private const string CookieName = "DLSFrameworkService";

        [SetSelectedTab(nameof(NavMenuTab.Dashboard))]
        public IActionResult Index()
        {
            var adminId = GetAdminId();
            var username = GetUserFirstName();
            var isFrameworkDeveloper = GetIsFrameworkDeveloper();
            var isFrameworkContributor = GetIsFrameworkContributor();
            var isWorkforceManager = GetIsWorkforceManager();
            var isWorkforceContributor = GetIsWorkforceContributor();
            var dashboardData = frameworkService.GetDashboardDataForAdminID(adminId);
            var dashboardToDoItems = frameworkService.GetDashboardToDoItems(adminId);
            var model = new DashboardViewModel(
                username,
                isFrameworkDeveloper,
                isFrameworkContributor,
                isWorkforceManager,
                isWorkforceContributor,
                dashboardData,
                dashboardToDoItems
                );
            return View(model);
        }
        [Route("/Frameworks/View/{tabname}/{page=1:int}")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        public IActionResult ViewFrameworks(string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            int page = 1,
            string tabname = "All")
        {
            sortBy ??= FrameworkSortByOptions.FrameworkName.PropertyName;
            var adminId = GetAdminId();
            var isFrameworkDeveloper = GetIsFrameworkDeveloper();
            var isFrameworkContributor = GetIsFrameworkContributor();
            searchString = searchString == null ? null : searchString.Trim();
            IEnumerable<BrandedFramework> frameworks;

            if (tabname == "All") frameworks = frameworkService.GetAllFrameworks(adminId);
            else
            {
                if (!isFrameworkDeveloper && !isFrameworkContributor) return RedirectToAction("ViewFrameworks", "Frameworks", new { tabname = "All" });
                frameworks = frameworkService.GetFrameworksForAdminId(adminId);
            }
            MyFrameworksViewModel myFrameworksViewModel;
            AllFrameworksViewModel allFrameworksViewModel;

            var searchSortPaginateOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString, 60),
                new SortOptions(sortBy, sortDirection),
                null,
                new PaginationOptions(page, 12)
            );

            if (tabname == "All")
            {
                var myFrameworksResult = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                    new List<BrandedFramework>(),
                    searchSortPaginateOptions
                );
                myFrameworksViewModel = new MyFrameworksViewModel(myFrameworksResult, isFrameworkDeveloper);
                var allFrameworksResult = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                    frameworks,
                    searchSortPaginateOptions
                );
                allFrameworksViewModel = new AllFrameworksViewModel(allFrameworksResult);
            }
            else
            {
                var myFrameworksResult = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                    frameworks,
                    searchSortPaginateOptions
                );
                myFrameworksViewModel = new MyFrameworksViewModel(myFrameworksResult, isFrameworkDeveloper);
                var allFrameworksResult = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                    new List<BrandedFramework>(),
                    searchSortPaginateOptions
                );
                allFrameworksViewModel = new AllFrameworksViewModel(allFrameworksResult);
            }

            var currentTab = tabname == "All" ? FrameworksTab.AllFrameworks : FrameworksTab.MyFrameworks;
            var frameworksViewModel = new FrameworksViewModel(
                isFrameworkDeveloper,
                isFrameworkContributor,
                myFrameworksViewModel,
                allFrameworksViewModel,
                currentTab
                );
            return View("Developer/Frameworks", frameworksViewModel);
        }

        public IActionResult StartNewFrameworkSession()
        {
            var adminId = GetAdminId();
            TempData.Clear();
            var detailFramework = new DetailFramework()
            {
                BrandID = 6,
                OwnerAdminID = adminId,
                UpdatedByAdminID = adminId,
                TopicID = 1,
                CategoryID = 1,
                PublishStatusID = 1,
                UserRole = 3,
            };
            var sessionNewFramework = new SessionNewFramework() { DetailFramework = detailFramework };
            multiPageFormService.SetMultiPageFormData(
                sessionNewFramework,
                MultiPageFormDataFeature.AddNewFramework,
                TempData
            );
            return RedirectToAction("CreateNewFramework", "Frameworks", new { actionName = "New" });
        }

        [Route("/Frameworks/Name/{actionName}/{frameworkId}")]
        [Route("/Frameworks/Name/{actionName}")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewFramework) }
        )]
        public IActionResult CreateNewFramework(string actionName, int frameworkId = 0)
        {
            var adminId = GetAdminId();
            DetailFramework? detailFramework;
            if (frameworkId > 0)
            {
                detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (detailFramework == null)
                {
                    logger.LogWarning($"Failed to load name page for frameworkID: {frameworkId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (detailFramework.UserRole < 2) return StatusCode(403);
            }
            else
            {
                var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                ).GetAwaiter().GetResult();
                multiPageFormService.SetMultiPageFormData(sessionNewFramework, MultiPageFormDataFeature.AddNewFramework, TempData);
                detailFramework = sessionNewFramework.DetailFramework;
            }
            return View("Developer/Name", detailFramework);
        }
        [HttpPost]
        [Route("/Frameworks/Name/{actionName}/{frameworkId}")]
        [Route("/Frameworks/Name/{actionName}")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewFramework) }
        )]
        public IActionResult CreateNewFramework(DetailFramework detailFramework, string actionName, int frameworkId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(BaseFramework.FrameworkName));
                ModelState.AddModelError(nameof(BaseFramework.FrameworkName), "Please enter a valid framework name (between 3 and 255 characters)");
                return View("Developer/Name", detailFramework);
            }
            if (actionName == "New")
            {
                SessionNewFramework sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
                sessionNewFramework.DetailFramework = detailFramework;
                multiPageFormService.SetMultiPageFormData(
                    sessionNewFramework,
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                );
                return RedirectToAction("SetNewFrameworkName", new { frameworkname = detailFramework.FrameworkName, actionName });
            }
            var adminId = GetAdminId();
            var isUpdated = frameworkService.UpdateFrameworkName(detailFramework.ID, adminId, detailFramework.FrameworkName);
            if (isUpdated) return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
            ModelState.AddModelError(nameof(BaseFramework.FrameworkName), "Another framework exists with that name. Please choose a different name.");
            return View("Developer/Name", detailFramework);
        }

        [Route("/Frameworks/Similar/{actionName}")]
        public IActionResult SetNewFrameworkName(string frameworkname, string actionName)
        {
            if (actionName == "New")
            {
                var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
                multiPageFormService.SetMultiPageFormData(
                    sessionNewFramework,
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                );
            }
            var adminId = GetAdminId();
            var sameItems = frameworkService.GetFrameworkByFrameworkName(frameworkname, adminId);
            var frameworks = frameworkService.GetAllFrameworks(adminId);
            var sortedItems = GenericSortingHelper.SortAllItems(
               frameworks.AsQueryable(),
               FrameworkSortByOptions.FrameworkName.PropertyName,
               GenericSortingHelper.Ascending
           );
            var similarItems = GenericSearchHelper.SearchItems(sortedItems, frameworkname, 55, true);
            var brandedFrameworks = similarItems.ToList();
            var matchingSearchResults = brandedFrameworks.ToList().Count;
            if (matchingSearchResults > 0)
            {
                var model = new SimilarViewModel()
                {
                    FrameworkName = frameworkname,
                    MatchingSearchResults = matchingSearchResults,
                    SimilarFrameworks = brandedFrameworks,
                    SameFrameworks = sameItems
                };
                return View("Developer/Similar", model);
            }
            return RedirectToAction("SaveNewFramework", "Frameworks", new { frameworkname, actionName });

        }
        public IActionResult SaveNewFramework(string frameworkname, string actionName)
        {
            //var framework = frameworkService.CreateFramework(frameworkname, GetAdminID());
            //need to create framework and move to next step.
            if (actionName != "New")
            {
                return StatusCode(500);
            }
            var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
            multiPageFormService.SetMultiPageFormData(
                sessionNewFramework,
                MultiPageFormDataFeature.AddNewFramework,
                TempData
            );
            return RedirectToAction("FrameworkDescription", "Frameworks", new { actionName });
        }

        [Route("/Frameworks/Description/{actionName}/")]
        [Route("/Frameworks/Description/{actionName}/{frameworkId}/")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewFramework) }
        )]
        public IActionResult FrameworkDescription(string actionName, int frameworkId = 0)
        {
            var adminId = GetAdminId();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionName == "New")
            {
                var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
                framework = sessionNewFramework.DetailFramework;
                multiPageFormService.SetMultiPageFormData(
                    sessionNewFramework,
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                );
            }
            else
            {
                framework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (framework == null | centreId == null)
                {
                    logger.LogWarning($"Failed to load description page for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                    return StatusCode(500);
                }
                if (framework.UserRole < 2) return StatusCode(403);
            }
            return View("Developer/Description", framework);
        }

        [HttpPost]
        [Route("/Frameworks/Description/{actionName}/")]
        [Route("/Frameworks/Description/{actionName}/{frameworkId}/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewFramework) }
        )]
        public IActionResult FrameworkDescription(DetailFramework detailFramework, string actionName, int frameworkId = 0)
        {
            if (actionName == "New")
            {
                var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
                sessionNewFramework.DetailFramework = detailFramework;
                multiPageFormService.SetMultiPageFormData(
                    sessionNewFramework,
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                );
                return RedirectToAction("FrameworkType", "Frameworks", new { actionName });
            }
            detailFramework.Description = SanitizerHelper.SanitizeHtmlData(detailFramework.Description);
            frameworkService.UpdateFrameworkDescription(frameworkId, GetAdminId(), detailFramework.Description);
            return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });

        }

        [Route("/Frameworks/Type/{actionName}/")]
        [Route("/Frameworks/Type/{actionName}/{frameworkId}/")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewFramework) }
        )]
        public IActionResult FrameworkType(string actionName, int frameworkId = 0)
        {
            var adminId = GetAdminId();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionName == "New")
            {
                var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
                framework = sessionNewFramework.DetailFramework;
                multiPageFormService.SetMultiPageFormData(
                    sessionNewFramework,
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                );
            }
            else
            {
                framework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (framework == null | centreId == null)
                {
                    logger.LogWarning($"Failed to load branding page for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                    return StatusCode(500);
                }
                if (framework.UserRole < 2) return StatusCode(403);
            }
            return View("Developer/Type", framework);
        }

        [HttpPost]
        [Route("/Frameworks/Type/{actionName}/")]
        [Route("/Frameworks/Type/{actionName}/{frameworkId}/")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewFramework) }
        )]
        public IActionResult FrameworkType(DetailFramework detailFramework, string actionName, int frameworkId = 0)
        {
            if (actionName == "New")
            {
                var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
                sessionNewFramework.DetailFramework = detailFramework;
                multiPageFormService.SetMultiPageFormData(
                    sessionNewFramework,
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                );
                return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { actionName });
            }
            frameworkService.UpdateFrameworkConfig(frameworkId, GetAdminId(), detailFramework.FrameworkConfig);
            return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
        }

        [Route("/Frameworks/Categorise/{actionName}/")]
        [Route("/Frameworks/Categorise/{actionName}/{frameworkId}/")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddNewFramework) }
        )]
        public IActionResult SetNewFrameworkBrand(string actionName, int frameworkId = 0)
        {
            var adminId = GetAdminId();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionName == "New")
            {
                if (TempData[MultiPageFormDataFeature.AddNewFramework.TempDataKey] == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }
                var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
                framework = sessionNewFramework.DetailFramework;
                multiPageFormService.SetMultiPageFormData(
                    sessionNewFramework,
                    MultiPageFormDataFeature.AddNewFramework,
                    TempData
                );
            }
            else
            {
                framework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (framework == null | centreId == null)
                {
                    logger.LogWarning(
                        $"Failed to load branding page for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}"
                    );
                    return StatusCode(500);
                }

                if (framework.UserRole < 2) return StatusCode(403);
            }
            if (centreId == null) return StatusCode(404);
            var brandsList = commonService.GetBrandListForCentre((int)centreId).Select(b => new { b.BrandID, b.BrandName }).ToList();
            var categoryList = commonService.GetCategoryListForCentre((int)centreId).Select(c => new { c.CourseCategoryID, c.CategoryName }).ToList();
            var topicList = commonService.GetTopicListForCentre((int)centreId).Select(t => new { t.CourseTopicID, t.CourseTopic }).ToList();
            var brandSelectList = new SelectList(brandsList, "BrandID", "BrandName");
            var categorySelectList = new SelectList(categoryList, "CourseCategoryID", "CategoryName");
            var topicSelectList = new SelectList(topicList, "CourseTopicID", "CourseTopic");
            var model = new BrandingViewModel()
            {
                DetailFramework = framework,
                BrandSelectList = brandSelectList,
                CategorySelectList = categorySelectList,
                TopicSelectList = topicSelectList
            };
            return View("Developer/Branding", model);
        }

        [HttpPost]
        [Route("/Frameworks/Categorise/{actionName}/")]
        [Route("/Frameworks/Categorise/{actionName}/{frameworkId}/")]
        public IActionResult SetNewFrameworkBrand(DetailFramework? detailFramework, string actionName, int frameworkId = 0)
        {
            var adminId = GetAdminId();
            var centreId = GetCentreId();
            if (actionName != "New" && frameworkId > 0)
            {
                detailFramework = InsertBrandingCategoryTopicIfRequired(detailFramework);
                if (detailFramework == null) return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                if (detailFramework.BrandID == null | detailFramework.CategoryID == null) return StatusCode(404);
                var updatedFramework = frameworkService.UpdateFrameworkBranding(frameworkId, (int)detailFramework.BrandID, (int)detailFramework.CategoryID, detailFramework.TopicID, adminId);
                if (updatedFramework != null) return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
                logger.LogWarning($"Failed to update branding for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                return StatusCode(500);
            }
            var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
            sessionNewFramework.DetailFramework.BrandID = detailFramework.BrandID;
            sessionNewFramework.DetailFramework.Brand = detailFramework.Brand;
            sessionNewFramework.DetailFramework.CategoryID = detailFramework.CategoryID;
            sessionNewFramework.DetailFramework.Category = detailFramework.Category;
            sessionNewFramework.DetailFramework.TopicID = detailFramework.TopicID;
            sessionNewFramework.DetailFramework.Topic = detailFramework.Topic;
            if (sessionNewFramework.DetailFramework.Brand == null && sessionNewFramework.DetailFramework.BrandID > 0 && sessionNewFramework.DetailFramework.BrandID != null)
                sessionNewFramework.DetailFramework.Brand =
                    commonService.GetBrandNameById((int)sessionNewFramework.DetailFramework.BrandID);
            if (sessionNewFramework.DetailFramework.Category == null && sessionNewFramework.DetailFramework.CategoryID > 0)
                sessionNewFramework.DetailFramework.Category =
                    commonService.GetCategoryNameById((int)sessionNewFramework.DetailFramework.CategoryID);
            if (sessionNewFramework.DetailFramework.Topic == null && sessionNewFramework.DetailFramework.TopicID > 0)
                sessionNewFramework.DetailFramework.Topic =
                    commonService.GetCategoryNameById(sessionNewFramework.DetailFramework.TopicID);
            multiPageFormService.SetMultiPageFormData(
                sessionNewFramework,
                MultiPageFormDataFeature.AddNewFramework,
                TempData
            );
            return RedirectToAction("FrameworkSummary", "Frameworks");
        }

        public DetailFramework? InsertBrandingCategoryTopicIfRequired(DetailFramework? detailFramework)
        {
            if (detailFramework == null)
                return null;
            var centreId = GetCentreId();
            if (detailFramework.BrandID == null | detailFramework.CategoryID == null | centreId == null)
                return null;
            if (detailFramework.BrandID == 0)
            {
                if (detailFramework.Brand != null)
                    detailFramework.BrandID =
                        commonService.InsertBrandAndReturnId(detailFramework.Brand, (int)centreId);
                else
                    return null;
            }
            if (detailFramework.CategoryID == 0)
            {
                if (detailFramework.Category != null)
                    detailFramework.CategoryID = commonService.InsertCategoryAndReturnId(
                        detailFramework.Category,
                        (int)centreId
                    );
                else
                    return null;
            }
            if (detailFramework.TopicID != 0)
                return detailFramework;
            if (detailFramework.Topic != null)
                detailFramework.TopicID = commonService.InsertTopicAndReturnId(detailFramework.Topic, (int)centreId);
            else
                return null;

            return detailFramework;
        }

        [Route("/Frameworks/New/Summary")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult FrameworkSummary()
        {
            if (TempData[MultiPageFormDataFeature.AddNewFramework.TempDataKey] == null)
            {
                return RedirectToAction("Index");
            }

            var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
            multiPageFormService.SetMultiPageFormData(
                sessionNewFramework,
                MultiPageFormDataFeature.AddNewFramework,
                TempData
            );
            return View("Developer/Summary", sessionNewFramework.DetailFramework);
        }

        [Route("/Frameworks/Flags/{frameworkId}/")]
        public IActionResult EditFrameworkFlags(int frameworkId, bool error = false)
        {
            var flags = frameworkService.GetCustomFlagsByFrameworkId(frameworkId, null);
            var model = new CustomFlagsViewModel()
            {
                Flags = flags
            };
            return View("Developer/CustomFlags", model);
        }

        [HttpGet]
        [Route("/Frameworks/Flags/Delete/{frameworkId}/{flagId}")]
        public IActionResult RemoveFrameworkFlag(int flagId, int frameworkId)
        {
            var flag = frameworkService.GetCustomFlagsByFrameworkId(frameworkId, flagId).FirstOrDefault();
            if (flag == null)
            {
                return StatusCode((int)HttpStatusCode.Gone);
            }

            var model = new RemoveCustomFlagConfirmationViewModel()
            {
                FlagId = flagId,
                FlagName = flag.FlagName,
                FrameworkId = frameworkId
            };
            return View("Developer/RemoveCustomFlagConfirmation", model);

        }

        [HttpPost]
        [Route("/Frameworks/Flags/Delete/{frameworkId}/{flagId}")]
        public IActionResult RemoveFrameworkFlag(RemoveCustomFlagConfirmationViewModel model)
        {
            frameworkService.RemoveCustomFlag(model.FlagId);
            return RedirectToAction("EditFrameworkFlags", "Frameworks", new { model.FrameworkId });
        }

        [HttpPost]
        [Route("/Frameworks/Flags/{actionName}/{frameworkId}/{flagId}")]
        public IActionResult EditFrameworkFlag(CustomFlagViewModel model, int frameworkId, string actionName, int flagId)
        {
            if (ModelState.IsValid)
            {
                var flags = frameworkService.GetCustomFlagsByFrameworkId(frameworkId, null)
                    .Where(fn => fn.FlagName?.Trim().ToLower() == model.FlagName?.Trim().ToLower()).ToList();

                bool nameExists = flags.Any(x => x.FlagName?.Trim().ToLower() == model.FlagName?.Trim().ToLower());
                bool idExists = flags.Any(x => x.FlagId == flagId);

                if (actionName == "Edit")
                {
                    if (nameExists && !idExists)
                    {
                        ModelState.AddModelError(nameof(model.FlagName), "A custom flag already exists.");
                        return View("Developer/EditCustomFlag", model);
                    }
                    else
                        frameworkService.UpdateFrameworkCustomFlag(frameworkId, model.Id, model.FlagName?.Trim(), model.FlagGroup?.Trim(), model.FlagTagClass);
                }
                else
                {
                    if (nameExists)
                    {
                        ModelState.AddModelError(nameof(model.FlagName), "A custom flag already exists.");
                        return View("Developer/EditCustomFlag", model);
                    }
                    else
                        frameworkService.AddCustomFlagToFramework(frameworkId, model.FlagName?.Trim(), model.FlagGroup?.Trim(), model.FlagTagClass);
                }

                return RedirectToAction("EditFrameworkFlags", "Frameworks", new { frameworkId });
            }
            return View("Developer/EditCustomFlag", model);
        }

        [HttpGet]
        [Route("/Frameworks/Flags/{actionName:regex(Edit|New)}/{frameworkId}/{flagId}")]
        public IActionResult EditFrameworkFlag(int frameworkId, string actionName, int flagId)
        {
            var model = new CustomFlagViewModel();
            if (actionName == "Edit")
            {
                var flag = frameworkService.GetCustomFlagsByFrameworkId(frameworkId, (int?)flagId).FirstOrDefault();
                model = new CustomFlagViewModel()
                {
                    Id = flag.FlagId,
                    FlagGroup = flag.FlagGroup,
                    FlagName = flag.FlagName,
                    FlagTagClass = flag.FlagTagClass
                };
            }
            return View("Developer/EditCustomFlag", model);
        }

        [Route("/Frameworks/Collaborators/{actionName}/{frameworkId}/")]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult AddCollaborators(string actionName, int frameworkId, bool error = false)
        {
            var adminId = GetAdminId();
            var collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (framework == null) return StatusCode(404);
            if (framework.UserRole < 2)
                return StatusCode(403);
            var model = new CollaboratorsViewModel()
            {
                BaseFramework = framework,
                Collaborators = collaborators,
                Error = false,
            };
            if (TempData["FrameworkError"] != null)
            {
                ModelState.AddModelError("userEmail", TempData.Peek("FrameworkError").ToString());
            }
            return View("Developer/Collaborators", model);
        }

        [HttpPost]
        [Route("/Frameworks/Collaborators/{actionName}/{frameworkId}/")]
        public IActionResult AddCollaborator(string actionName, string userEmail, bool canModify, int frameworkId)
        {
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, GetAdminId());
            if (framework == null) return StatusCode(404);
            if (framework.UserRole < 2) return StatusCode(403);

            var collaboratorId = frameworkService.AddCollaboratorToFramework(frameworkId, userEmail, canModify);
            if (collaboratorId == CollaboratorResultCodes.AdminAccountSelectionRequired)
            {
                // The email has an admin account at more than one centre, so ask which one to add.
                var model = new SelectAdminAccountViewModel(
                    frameworkId,
                    framework.FrameworkName,
                    userEmail.Trim(),
                    canModify,
                    actionName,
                    userService.GetActiveAdminAccountsByEmail(userEmail)
                );
                return View("Developer/SelectCollaboratorAccount", model);
            }

            return HandleAddCollaboratorResult(collaboratorId, frameworkId, actionName);
        }

        [HttpPost]
        [Route("/Frameworks/Collaborators/{actionName}/{frameworkId}/SelectAccount")]
        public IActionResult SelectCollaboratorAccount(string actionName, int frameworkId, SelectAdminAccountViewModel model)
        {
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, GetAdminId());
            if (framework == null) return StatusCode(404);
            if (framework.UserRole < 2) return StatusCode(403);

            model.EntityId = frameworkId;
            model.EntityName = framework.FrameworkName;
            model.ActionName = actionName;

            if (!ModelState.IsValid)
            {
                model.AdminAccounts = userService.GetActiveAdminAccountsByEmail(model.Email);
                if (!model.AdminAccounts.Any())
                {
                    TempData["FrameworkError"] = CollaboratorResultMessageHelper.GetErrorMessage(CollaboratorResultCodes.NoActiveAdminAccount, "framework");
                    return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionName });
                }
                return View("Developer/SelectCollaboratorAccount", model);
            }

            var collaboratorId = frameworkService.AddCollaboratorToFramework(frameworkId, model.Email, model.CanModify, model.SelectedAdminId);
            return HandleAddCollaboratorResult(collaboratorId, frameworkId, actionName);
        }

        private IActionResult HandleAddCollaboratorResult(int collaboratorId, int frameworkId, string actionName)
        {
            if (collaboratorId > 0)
            {
                try
                {
                    frameworkNotificationService.SendFrameworkCollaboratorInvite(collaboratorId, GetAdminId());
                }
                catch (Exception ex)
                {
                    // The collaborator has been added; a failed notification should not lose that.
                    logger.LogError(ex, $"Failed to send framework collaborator invite. collaboratorId: {collaboratorId}, frameworkId: {frameworkId}");
                }
            }
            else
            {
                TempData["FrameworkError"] = CollaboratorResultMessageHelper.GetErrorMessage(collaboratorId, "framework");
            }
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionName });
        }

        public IActionResult RemoveCollaborator(int frameworkId, string actionName, int id)
        {
            frameworkService.RemoveCollaboratorFromFramework(frameworkId, id);
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionName });
        }

        [Route("/Framework/{frameworkId}/{tabname}/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Framework/{frameworkId}/{tabname}/{frameworkCompetencyGroupId}/")]
        [Route("/Framework/{frameworkId}/{tabname}/")]
        [SetSelectedTab(nameof(NavMenuTab.Frameworks))]
        public IActionResult ViewFramework(string tabname, int frameworkId, int? frameworkCompetencyGroupId = null, int? frameworkCompetencyId = null)
        {
            var adminId = GetAdminId();
            var detailFramework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var routeData = new Dictionary<string, string> { { "frameworkId", detailFramework?.ID.ToString() } };
            var model = new FrameworkViewModel()
            {
                DetailFramework = detailFramework,
            };
            switch (tabname)
            {
                case "Details":
                    model.Collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
                    model.Flags = frameworkService.GetCustomFlagsByFrameworkId(frameworkId, null);
                    model.FrameworkDefaultQuestions = frameworkService.GetFrameworkDefaultQuestionsById(frameworkId, adminId);
                    model.TabNavLinks = new TabsNavViewModel(FrameworkTab.Details, routeData);
                    break;
                case "Structure":
                    model.FrameworkCompetencyGroups = frameworkService.GetFrameworkCompetencyGroups(frameworkId, null).ToList();
                    model.CompetencyFlags = frameworkService.GetCompetencyFlagsByFrameworkId(frameworkId, null, selected: true);
                    model.FrameworkCompetencies = frameworkService.GetFrameworkCompetenciesUngrouped(frameworkId, null);
                    model.TabNavLinks = new TabsNavViewModel(FrameworkTab.Structure, routeData);
                    break;
                case "Comments":
                    model.CommentReplies = frameworkService.GetCommentsForFrameworkId(frameworkId, adminId);
                    model.TabNavLinks = new TabsNavViewModel(FrameworkTab.Comments, routeData);
                    break;
            }
            return View("Developer/Framework", model);
        }

        [Route("/Framework/{frameworkId}/Structure/PrintLayout")]
        public IActionResult PrintLayout(int frameworkId)
        {
            var adminId = GetAdminId();
            var detailFramework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var routeData = new Dictionary<string, string> { { "frameworkId", detailFramework?.ID.ToString() } };
            var model = new FrameworkViewModel()
            {
                DetailFramework = detailFramework,
            };
            model.FrameworkCompetencyGroups = frameworkService.GetFrameworkCompetencyGroups(frameworkId, null).ToList();
            model.CompetencyFlags = frameworkService.GetCompetencyFlagsByFrameworkId(frameworkId, null, selected: true);
            model.FrameworkCompetencies = frameworkService.GetFrameworkCompetenciesUngrouped(frameworkId, null);
            return View("Developer/FrameworkPrintLayout", model);
        }
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult InsertFramework()
        {
            var adminId = GetAdminId();
            if (TempData[MultiPageFormDataFeature.AddNewFramework.TempDataKey] == null)
            {
                return StatusCode((int)HttpStatusCode.Gone);
            }

            var sessionNewFramework = multiPageFormService.GetMultiPageFormData<SessionNewFramework>(MultiPageFormDataFeature.AddNewFramework, TempData).GetAwaiter().GetResult();
            var detailFramework = sessionNewFramework.DetailFramework;
            detailFramework = InsertBrandingCategoryTopicIfRequired(detailFramework);
            if (detailFramework == null || adminId < 1)
            {
                logger.LogWarning($"Failed to create framework: adminId: {adminId}");
                return StatusCode(500);
            }
            detailFramework.Description = SanitizerHelper.SanitizeHtmlData(detailFramework.Description);
            var newFramework = frameworkService.CreateFramework(detailFramework, adminId);
            TempData.Clear();
            return RedirectToAction("AddCollaborators", "Frameworks", new { actionName = "New", frameworkId = newFramework.ID });
        }

        [HttpGet]
        [Route("/Frameworks/ChangeOwner/{frameworkId}")]
        public IActionResult ChangeOwner(int frameworkId)
        {
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, GetAdminId());
            if (framework == null) return StatusCode(404);
            if (!IsCurrentUserFrameworkOwner(framework)) return StatusCode(403);

            var model = new ChangeOwnerViewModel
            {
                FrameworkId = frameworkId,
                FrameworkName = framework.FrameworkName
            };
            return View("Developer/ChangeOwner", model);
        }

        [HttpPost]
        [Route("/Frameworks/ChangeOwner/{frameworkId}")]
        public IActionResult ChangeOwner(int frameworkId, ChangeOwnerViewModel model)
        {
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, GetAdminId());
            if (framework == null) return StatusCode(404);
            if (!IsCurrentUserFrameworkOwner(framework)) return StatusCode(403);

            model.FrameworkId = frameworkId;
            model.FrameworkName = framework.FrameworkName;
            model.NewOwnerEmail = model.NewOwnerEmail?.Trim();
            if (!ModelState.IsValid)
            {
                return View("Developer/ChangeOwner", model);
            }
            var checkEmail = frameworkService.CheckIfEmailExistAsCollaboratorsOrReviewer(frameworkId, model.NewOwnerEmail);
            if (checkEmail)
            {
                ModelState.AddModelError(nameof(model.NewOwnerEmail), "This user is already a collaborator or reviewer, so they cannot be made the owner.");
                return View("Developer/ChangeOwner", model);
            }
            if (string.Equals(User.GetUserPrimaryEmail(), model.NewOwnerEmail, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.NewOwnerEmail), "The new owner email address cannot be the same as the current owner's email address.");
                return View("Developer/ChangeOwner", model);
            }

            var adminAccounts = userService.GetActiveAdminAccountsByEmail(model.NewOwnerEmail!).ToList();
            if (!adminAccounts.Any())
            {
                ModelState.AddModelError(nameof(model.NewOwnerEmail), "No active admin user found with that email.");
                return View("Developer/ChangeOwner", model);
            }
            if (adminAccounts.Count > 1)
            {
                // The email has an admin account at more than one centre, so ask which one should own the framework.
                var selectModel = new SelectAdminAccountViewModel(
                    frameworkId,
                    framework.FrameworkName,
                    model.NewOwnerEmail!,
                    canModify: true,
                    actionName: null,
                    adminAccounts
                );
                return View("Developer/SelectNewOwnerAccount", selectModel);
            }

            return TryChangeFrameworkOwner(
                frameworkId,
                model.NewOwnerEmail!,
                adminAccounts[0].AdminId,
                errorMessage =>
                {
                    ModelState.AddModelError(nameof(model.NewOwnerEmail), errorMessage);
                    return View("Developer/ChangeOwner", model);
                }
            );
        }

        [HttpPost]
        [Route("/Frameworks/ChangeOwner/{frameworkId}/SelectAccount")]
        public IActionResult SelectNewOwnerAccount(int frameworkId, SelectAdminAccountViewModel model)
        {
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, GetAdminId());
            if (framework == null) return StatusCode(404);
            if (!IsCurrentUserFrameworkOwner(framework)) return StatusCode(403);

            model.EntityId = frameworkId;
            model.EntityName = framework.FrameworkName;
            model.Email = model.Email?.Trim() ?? string.Empty;
            model.AdminAccounts = userService.GetActiveAdminAccountsByEmail(model.Email).ToList();

            if (!model.AdminAccounts.Any())
            {
                return ChangeOwnerViewWithError(frameworkId, framework.FrameworkName, model.Email, "No active admin user found with that email.");
            }
            if (!ModelState.IsValid)
            {
                return View("Developer/SelectNewOwnerAccount", model);
            }
            if (model.AdminAccounts.All(a => a.AdminId != model.SelectedAdminId))
            {
                // The posted account does not belong to this email (stale page or tampered form).
                ModelState.AddModelError(nameof(model.SelectedAdminId), SelectAdminAccountViewModel.SelectionRequiredErrorMessage);
                model.SelectedAdminId = null;
                return View("Developer/SelectNewOwnerAccount", model);
            }
            if (frameworkService.CheckIfEmailExistAsCollaboratorsOrReviewer(frameworkId, model.Email))
            {
                return ChangeOwnerViewWithError(frameworkId, framework.FrameworkName, model.Email, "This user is already a collaborator or reviewer, so they cannot be made the owner.");
            }

            return TryChangeFrameworkOwner(
                frameworkId,
                model.Email,
                model.SelectedAdminId!.Value,
                errorMessage =>
                {
                    ModelState.AddModelError(nameof(model.SelectedAdminId), errorMessage);
                    return View("Developer/SelectNewOwnerAccount", model);
                }
            );
        }

        private IActionResult TryChangeFrameworkOwner(int frameworkId, string newOwnerEmail, int newOwnerAdminId, Func<string, IActionResult> onError)
        {
            try
            {
                frameworkService.ChangeFrameworkOwner(frameworkId, newOwnerEmail, GetAdminId(), newOwnerAdminId);
                return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, $"Failed to change framework owner. frameworkId: {frameworkId}, newOwnerAdminId: {newOwnerAdminId}");
                return onError(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unexpected error changing framework owner. frameworkId: {frameworkId}, newOwnerAdminId: {newOwnerAdminId}");
                return onError("The owner could not be changed. Please try again.");
            }
        }

        private IActionResult ChangeOwnerViewWithError(int frameworkId, string? frameworkName, string email, string errorMessage)
        {
            ModelState.Clear();
            var changeOwnerModel = new ChangeOwnerViewModel
            {
                FrameworkId = frameworkId,
                FrameworkName = frameworkName,
                NewOwnerEmail = email,
            };
            ModelState.AddModelError(nameof(changeOwnerModel.NewOwnerEmail), errorMessage);
            return View("Developer/ChangeOwner", changeOwnerModel);
        }

        private bool IsCurrentUserFrameworkOwner(BaseFramework framework)
        {
            var userAdminIds = frameworkService.GetAdminIdsForUserId(User.GetUserIdKnownNotNull());
            return userAdminIds.Contains(framework.OwnerAdminID);
        }
    }
}
