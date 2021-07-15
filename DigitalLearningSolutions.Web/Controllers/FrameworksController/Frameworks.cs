﻿using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;
using DigitalLearningSolutions.Web.Extensions;
using DigitalLearningSolutions.Data.Models.SessionData.Frameworks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public partial class FrameworksController
    {
        private const string CookieName = "DLSFrameworkService";
        public IActionResult Index()
        {
            var adminId = GetAdminID();
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
        public IActionResult ViewFrameworks(string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            int page = 1,
            string tabname = "All")
        {
            sortBy ??= FrameworkSortByOptions.FrameworkName.PropertyName;

            var adminId = GetAdminID();
            var isFrameworkDeveloper = GetIsFrameworkDeveloper();
            var isFrameworkContributor = GetIsFrameworkContributor();
            IEnumerable<BrandedFramework> frameworks;

            if (tabname == "All")
            {
                frameworks = frameworkService.GetAllFrameworks(adminId);
            }
            else
            {
                if (!isFrameworkDeveloper && !isFrameworkContributor)
                {
                    return RedirectToAction("ViewFrameworks", "Frameworks", new { tabname = "All" });
                }
                frameworks = frameworkService.GetFrameworksForAdminId(adminId);
            }
            if (frameworks == null)
            {
                logger.LogWarning($"Attempt to display frameworks for admin {adminId} returned null.");
                return StatusCode(403);
            }
            MyFrameworksViewModel myFrameworksViewModel;
            AllFrameworksViewModel allFrameworksViewModel;
            if (tabname == "All")
            {
                myFrameworksViewModel = new MyFrameworksViewModel(
                new List<BrandedFramework>(),
                searchString,
                sortBy,
                sortDirection,
                page,
                isFrameworkDeveloper);
                allFrameworksViewModel = new AllFrameworksViewModel(
                    frameworks,
                searchString,
                sortBy,
                sortDirection,
                page);
            }
            else
            {
                myFrameworksViewModel = new MyFrameworksViewModel(
                frameworks,
                 searchString,
                 sortBy,
                 sortDirection,
                 page,
                 isFrameworkDeveloper);
                allFrameworksViewModel = new AllFrameworksViewModel(
                     new List<BrandedFramework>(),
                searchString,
                sortBy,
                sortDirection,
                page);
            }
            var frameworksViewModel = new FrameworksViewModel(
                isFrameworkDeveloper,
                isFrameworkContributor,
                myFrameworksViewModel,
                allFrameworksViewModel
                );
            return View("Developer/Frameworks", frameworksViewModel);
        }
        public IActionResult StartNewFrameworkSession()
        {
            var adminId = GetAdminID();
            TempData.Clear();
            var sessionNewFramework = new SessionNewFramework();
            if (!Request.Cookies.ContainsKey(CookieName))
            {
                var id = Guid.NewGuid();

                Response.Cookies.Append(
                    CookieName,
                    id.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    });

                sessionNewFramework.Id = id;
            }
            else
            {
                if (Request.Cookies.TryGetValue(CookieName, out string idString))
                {
                    sessionNewFramework.Id = Guid.Parse(idString);
                }
                else
                {
                    var id = Guid.NewGuid();

                    Response.Cookies.Append(
                        CookieName,
                        id.ToString(),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(30)
                        });
                    sessionNewFramework.Id = id;
                }
            }
            DetailFramework detailFramework = new DetailFramework()
            {
                BrandID = 6,
                OwnerAdminID = adminId,
                UpdatedByAdminID = adminId,
                TopicID = 1,
                CategoryID = 1,
                PublishStatusID = 1,
                UserRole = 3
            };
            sessionNewFramework.DetailFramework = detailFramework;
            TempData.Set(sessionNewFramework);
            return RedirectToAction("CreateNewFramework", "Frameworks", new { actionname = "New" });
        }
        [Route("/Frameworks/Name/{actionname}/{frameworkId}")]
        [Route("/Frameworks/Name/{actionname}")]
        public IActionResult CreateNewFramework(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            DetailFramework? detailFramework;
            if (frameworkId > 0)
            {
                detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (detailFramework == null)
                {
                    logger.LogWarning($"Failed to load name page for frameworkID: {frameworkId} adminId: {adminId}");
                    return StatusCode(500);
                }
                if (detailFramework.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            else
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                TempData.Set(sessionNewFramework);
                detailFramework = sessionNewFramework.DetailFramework;
                TempData.Set(sessionNewFramework);
            }
            return View("Developer/Name", detailFramework);
        }
        [HttpPost]
        [Route("/Frameworks/Name/{actionname}/{frameworkId}")]
        [Route("/Frameworks/Name/{actionname}")]
        public IActionResult CreateNewFramework(DetailFramework detailFramework, string actionname, int frameworkId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(BaseFramework.FrameworkName));
                ModelState.AddModelError(nameof(BaseFramework.FrameworkName), "Please enter a valid framework name (between 3 and 255 characters)");
                // do something
                return View("Developer/Name", detailFramework);
            }
            else
            {
                if (actionname == "New")
                {
                    SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                    sessionNewFramework.DetailFramework = detailFramework;
                    TempData.Set(sessionNewFramework);
                    return RedirectToAction("SetNewFrameworkName", new { frameworkname = detailFramework.FrameworkName, actionname });
                }
                else
                {
                    var adminId = GetAdminID();
                    var isUpdated = frameworkService.UpdateFrameworkName(detailFramework.ID, adminId, detailFramework.FrameworkName);
                    if (isUpdated)
                    {
                        return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(BaseFramework.FrameworkName), "Another framework exists with that name. Please choose a different name.");
                        // do something
                        return View("Developer/Name", detailFramework);
                    }
                }

            }
        }
        [Route("/Frameworks/Similar/{actionname}")]
        public IActionResult SetNewFrameworkName(string frameworkname, string actionname)
        {
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                TempData.Set(sessionNewFramework);
            }
            var adminId = GetAdminID();
            var sameItems = frameworkService.GetFrameworkByFrameworkName(frameworkname, adminId);
            var frameworks = frameworkService.GetAllFrameworks(adminId);
            var sortedItems = GenericSortingHelper.SortAllItems(
               frameworks.AsQueryable(),
               FrameworkSortByOptions.FrameworkName.PropertyName,
               BaseSearchablePageViewModel.Ascending
           );
            var similarItems = GenericSearchHelper.SearchItems(sortedItems, frameworkname, 55, true);
            var matchingSearchResults = similarItems.ToList().Count;
            if (matchingSearchResults > 0)
            {
                var model = new SimilarViewModel()
                {
                    FrameworkName = frameworkname,
                    MatchingSearchResults = matchingSearchResults,
                    SimilarFrameworks = similarItems,
                    SameFrameworks = sameItems
                };
                return View("Developer/Similar", model);
            }
            else
            {
                return RedirectToAction("SaveNewFramework", "Frameworks", new { frameworkname, actionname });
            }

        }
        public IActionResult SaveNewFramework(string frameworkname, string actionname)
        {
            //var framework = frameworkService.CreateFramework(frameworkname, GetAdminID());
            //need to create framework and move to next step.
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                TempData.Set(sessionNewFramework);
                return RedirectToAction("FrameworkDescription", "Frameworks", new { actionname });
            }
            else
            {
                return StatusCode(500);
            }
        }
        [Route("/Frameworks/Description/{actionname}/")]
        [Route("/Frameworks/Description/{actionname}/{frameworkId}/")]
        public IActionResult FrameworkDescription(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                framework = sessionNewFramework.DetailFramework;
                TempData.Set(sessionNewFramework);
            }
            else
            {
                framework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (framework == null | centreId == null)
                {
                    logger.LogWarning($"Failed to load description page for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                    return StatusCode(500);
                }
                if (framework.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            return View("Developer/Description", framework);
        }
        [HttpPost]
        [Route("/Frameworks/Description/{actionname}/")]
        [Route("/Frameworks/Description/{actionname}/{frameworkId}/")]
        public IActionResult FrameworkDescription(DetailFramework detailFramework, string actionname, int frameworkId = 0)
        {
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                sessionNewFramework.DetailFramework = detailFramework;
                TempData.Set(sessionNewFramework);
                return RedirectToAction("FrameworkType", "Frameworks", new { actionname });
            }
            else
            {
                frameworkService.UpdateFrameworkDescription(frameworkId, GetAdminID(), detailFramework.Description);
                return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
            }

        }
        [Route("/Frameworks/Type/{actionname}/")]
        [Route("/Frameworks/Type/{actionname}/{frameworkId}/")]
        public IActionResult FrameworkType(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                framework = sessionNewFramework.DetailFramework;
                TempData.Set(sessionNewFramework);
            }
            else
            {
                framework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (framework == null | centreId == null)
                {
                    logger.LogWarning($"Failed to load branding page for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                    return StatusCode(500);
                }
                if (framework.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
            return View("Developer/Type", framework);
        }
        [HttpPost]
        [Route("/Frameworks/Type/{actionname}/")]
        [Route("/Frameworks/Type/{actionname}/{frameworkId}/")]
        public IActionResult FrameworkType(DetailFramework detailFramework, string actionname, int frameworkId = 0)
        {
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                sessionNewFramework.DetailFramework = detailFramework;
                TempData.Set(sessionNewFramework);
                return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { actionname });
            }
            else
            {
                frameworkService.UpdateFrameworkConfig(frameworkId, GetAdminID(), detailFramework.FrameworkConfig);
                return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
            }
        }
        [Route("/Frameworks/Categorise/{actionname}/")]
        [Route("/Frameworks/Categorise/{actionname}/{frameworkId}/")]
        public IActionResult SetNewFrameworkBrand(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                framework = sessionNewFramework.DetailFramework;
                TempData.Set(sessionNewFramework);
            }
            else
            {
                framework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
                if (framework == null | centreId == null)
                {
                    logger.LogWarning($"Failed to load branding page for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                    return StatusCode(500);
                }
                if (framework.UserRole < 2)
                {
                    return StatusCode(403);
                }
            }
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
        [Route("/Frameworks/Categorise/{actionname}/")]
        [Route("/Frameworks/Categorise/{actionname}/{frameworkId}/")]
        public IActionResult SetNewFrameworkBrand(DetailFramework? detailFramework, string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            if (actionname != "New" && frameworkId > 0)
            {
                detailFramework = InsertBrandingCategoryTopicIfRequired(detailFramework);
                if (detailFramework == null)
                {
                    return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                }
                var updatedFramework = frameworkService.UpdateFrameworkBranding(frameworkId, (int)detailFramework.BrandID, (int)detailFramework.CategoryID, (int)detailFramework.TopicID, adminId);
                if (updatedFramework == null)
                {
                    logger.LogWarning($"Failed to update branding for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                    return StatusCode(500);
                }
                return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
            }
            else
            {
                SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
                sessionNewFramework.DetailFramework.BrandID = detailFramework.BrandID;
                sessionNewFramework.DetailFramework.Brand = detailFramework.Brand;
                sessionNewFramework.DetailFramework.CategoryID = detailFramework.CategoryID;
                sessionNewFramework.DetailFramework.Category = detailFramework.Category;
                sessionNewFramework.DetailFramework.TopicID = detailFramework.TopicID;
                sessionNewFramework.DetailFramework.Topic = detailFramework.Topic;
                if (sessionNewFramework.DetailFramework.Brand == null && sessionNewFramework.DetailFramework.BrandID > 0 && sessionNewFramework.DetailFramework.BrandID != null)
                {
                    sessionNewFramework.DetailFramework.Brand = commonService.GetBrandNameById((int)sessionNewFramework.DetailFramework.BrandID);
                }
                if (sessionNewFramework.DetailFramework.Category == null && sessionNewFramework.DetailFramework.CategoryID > 0)
                {
                    sessionNewFramework.DetailFramework.Category = commonService.GetCategoryNameById((int)sessionNewFramework.DetailFramework.CategoryID);
                }
                if (sessionNewFramework.DetailFramework.Topic == null && sessionNewFramework.DetailFramework.TopicID > 0)
                {
                    sessionNewFramework.DetailFramework.Topic = commonService.GetCategoryNameById((int)sessionNewFramework.DetailFramework.TopicID);
                }
                TempData.Set(sessionNewFramework);
                return RedirectToAction("FrameworkSummary", "Frameworks");
            }
        }
        public DetailFramework? InsertBrandingCategoryTopicIfRequired(DetailFramework? detailFramework)
        {
            if (detailFramework == null)
            {
                return null;
            }
            var centreId = GetCentreId();
            if (detailFramework.BrandID == null | detailFramework.CategoryID == null | detailFramework.TopicID == null)
            {
                return null;
            }
            if (detailFramework.BrandID == 0)
            {
                if (detailFramework.Brand != null)
                {
                    detailFramework.BrandID = commonService.InsertBrandAndReturnId(detailFramework.Brand, (int)centreId);
                }
                else
                {
                    return null;
                }
            }
            if (detailFramework.CategoryID == 0)
            {
                if (detailFramework.Category != null)
                {
                    detailFramework.CategoryID = commonService.InsertCategoryAndReturnId(detailFramework.Category, (int)centreId);
                }
                else
                {
                    return null;
                }
            }
            if (detailFramework.TopicID == 0)
            {
                if (detailFramework.Topic != null)
                {
                    detailFramework.TopicID = commonService.InsertTopicAndReturnId(detailFramework.Topic, (int)centreId);
                }
                else
                {
                    return null;
                }
            }
            return detailFramework;
        }
        [Route("/Frameworks/New/Summary")]
        public IActionResult FrameworkSummary()
        {
            SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
            if (sessionNewFramework == null)
            {
                return RedirectToAction("Index");
            }
            TempData.Set(sessionNewFramework);
            return View("Developer/Summary", sessionNewFramework.DetailFramework);
        }
        [Route("/Frameworks/Collaborators/{actionname}/{frameworkId}/")]
        public IActionResult AddCollaborators(string actionname, int frameworkId)
        {
            var adminId = GetAdminID();
            var collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (framework.UserRole < 2)
            {
                return StatusCode(403);
            }
            var frameworkName = (string)framework.FrameworkName;
            var model = new CollaboratorsViewModel()
            {
                BaseFramework = framework,
                Collaborators = collaborators
            };
            return View("Developer/Collaborators", model);
        }
        [HttpPost]
        [Route("/Frameworks/Collaborators/{actionname}/{frameworkId}/")]
        public IActionResult AddCollaborator(string actionname, string userEmail, bool canModify, int frameworkId)
        {
            var collaboratorId = frameworkService.AddCollaboratorToFramework(frameworkId, userEmail, canModify);
            if (collaboratorId > 0)
            {
                frameworkNotificationService.SendFrameworkCollaboratorInvite(collaboratorId, GetAdminID());
            }
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionname });
        }
        public IActionResult RemoveCollaborator(int frameworkId, string actionname, int id)
        {
            frameworkService.RemoveCollaboratorFromFramework(frameworkId, id);
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionname });
        }
        [Route("/Framework/{frameworkId}/{tabname}/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Framework/{frameworkId}/{tabname}/{frameworkCompetencyGroupId}/")]
        [Route("/Framework/{frameworkId}/{tabname}/")]
        public IActionResult ViewFramework(string tabname, int frameworkId, int? frameworkCompetencyGroupId = null, int? frameworkCompetencyId = null)
        {
            var adminId = GetAdminID();
            IEnumerable<CollaboratorDetail> collaboratorDetails;
            List<FrameworkCompetencyGroup>? frameworkCompetencyGroups;
            IEnumerable<FrameworkCompetency>? frameworkCompetencies;
            IEnumerable<AssessmentQuestion>? frameworkDefaultQuestions;
            IEnumerable<CommentReplies>? commentReplies;
            var detailFramework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var model = new FrameworkViewModel()
            {
                DetailFramework = detailFramework
            };
            if (tabname == "Details")
            {
                model.Collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
                model.FrameworkDefaultQuestions = frameworkService.GetFrameworkDefaultQuestionsById(frameworkId, adminId);
            }
            if (tabname == "Structure")
            {
                model.FrameworkCompetencyGroups = frameworkService.GetFrameworkCompetencyGroups(frameworkId).ToList();
                model.FrameworkCompetencies = frameworkService.GetFrameworkCompetenciesUngrouped(frameworkId);
            }
            if (tabname == "Comments")
            {
                model.CommentReplies = frameworkService.GetCommentsForFrameworkId(frameworkId, adminId);
            }
            return View("Developer/Framework", model);
        }
        public IActionResult InsertFramework()
        {
            var adminId = GetAdminID();
            SessionNewFramework sessionNewFramework = TempData.Peek<SessionNewFramework>();
            DetailFramework? detailFramework = sessionNewFramework.DetailFramework;
            detailFramework = InsertBrandingCategoryTopicIfRequired(detailFramework);
            if (detailFramework == null || adminId < 1)
            {
                logger.LogWarning($"Failed to create framework: adminId: {adminId}");
                return StatusCode(500);
            }
            var newFramework = frameworkService.CreateFramework(detailFramework, adminId);
            TempData.Clear();
            return RedirectToAction("AddCollaborators", "Frameworks", new { actionname = "New", frameworkId = newFramework.ID });
        }

    }
}
