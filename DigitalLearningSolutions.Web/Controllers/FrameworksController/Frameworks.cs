using DigitalLearningSolutions.Data.Models.Frameworks;
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
    public partial class FrameworksController
    {
        private const string CookieName = "DLSFrameworkService";
        [Route("/Frameworks/MyFrameworks/{page=1:int}")]
        public IActionResult FrameworksDashboard(string? searchString = null,
            string sortBy = FrameworkSortByOptionTexts.FrameworkCreatedDate,
            string sortDirection = BaseFrameworksPageViewModel.DescendingText,
            int page = 1)
        {
            var adminId = GetAdminID();
            var isFrameworkDeveloper = GetIsFrameworkDeveloper();
            if (isFrameworkDeveloper == false | isFrameworkDeveloper == null)
            {
                logger.LogWarning($"Attempt to access framework developer interface for admin {adminId} without Framework Developer role.");
                return StatusCode(403);
            }


            var frameworks = frameworkService.GetFrameworksForAdminId(adminId);
            if (frameworks == null)
            {
                logger.LogWarning($"Attempt to display frameworks for admin {adminId} returned null.");
                return StatusCode(403);
            }

            var model = new MyFrameworksViewModel(frameworks,
                searchString,
                sortBy,
                sortDirection,
                page);
            return View("Developer/MyFrameworks", model);
        }
        [Route("/Frameworks/AllFrameworks/{page=1:int}")]
        public IActionResult FrameworksViewAll(string? searchString = null,
            string sortBy = FrameworkSortByOptionTexts.FrameworkName,
            string sortDirection = BaseFrameworksPageViewModel.AscendingText,
            int page = 1)
        {
            var adminId = GetAdminID();
            var isFrameworkDeveloper = GetIsFrameworkDeveloper();
            if (isFrameworkDeveloper == false | isFrameworkDeveloper == null)
            {
                logger.LogWarning($"Attempt to access framework developer interface for admin {adminId} without Framework Developer role.");
                return StatusCode(403);
            }
            var frameworks = frameworkService.GetAllFrameworks(adminId);
            var model = new AllFrameworksViewModel(frameworks,
                searchString,
                sortBy,
                sortDirection,
                page);
            return View("Developer/AllFrameworks", model);
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
                PublishStatusID = 1
            };
            sessionNewFramework.DetailFramework = detailFramework;
            TempData.Set(sessionNewFramework);
            return RedirectToAction("CreateNewFramework", "Frameworks", new { actionname = "New" });
        }
        [Route("/Frameworks/{frameworkId}/{actionname}/Name")]
        [Route("/Frameworks/{actionname}/Name")]
        public IActionResult CreateNewFramework(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            DetailFramework? detailFramework;
            if (frameworkId > 0)
            {
                detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
            }
            else
            {
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
                TempData.Set(sessionNewFramework);
                detailFramework = sessionNewFramework.DetailFramework;
                TempData.Set(sessionNewFramework);
            }
            return View("Developer/Name", detailFramework);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/{actionname}/Name")]
        [Route("/Frameworks/{actionname}/Name")]
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
                    SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
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
        [Route("/Frameworks/{actionname}/Similar")]
        public IActionResult SetNewFrameworkName(string frameworkname, string actionname)
        {
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
                TempData.Set(sessionNewFramework);
            }
            var adminId = GetAdminID();
            var sameItems = frameworkService.GetFrameworkByFrameworkName(frameworkname, adminId);
            var frameworks = frameworkService.GetAllFrameworks(adminId);
            var sortedItems = SortingHelper.SortFrameworkItems(
               frameworks,
               FrameworkSortByOptionTexts.FrameworkName,
               BaseFrameworksPageViewModel.AscendingText
           );
            var similarItems = SearchHelper.FilterFrameworks(sortedItems, frameworkname, 55, true);
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
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
                TempData.Set(sessionNewFramework);
                return RedirectToAction("FrameworkDescription", "Frameworks", new { actionname });
            }
            else
            {
                return StatusCode(500);
            }
        }
        [Route("/Frameworks/{actionname}/Description/")]
        [Route("/Frameworks/{frameworkId}/{actionname}/Description/")]
        public IActionResult FrameworkDescription(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
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
            }
            return View("Developer/Description", framework);
        }
        [HttpPost]
        [Route("/Frameworks/{actionname}/Description/")]
        [Route("/Frameworks/{frameworkId}/{actionname}/Description/")]
        public IActionResult FrameworkDescription(DetailFramework detailFramework, string actionname, int frameworkId = 0)
        {
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
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
        [Route("/Frameworks/{actionname}/Type/")]
        [Route("/Frameworks/{frameworkId}/{actionname}/Type/")]
        public IActionResult FrameworkType(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
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
            }
            return View("Developer/Type", framework);
        }
        [HttpPost]
        [Route("/Frameworks/{actionname}/Type/")]
        [Route("/Frameworks/{frameworkId}/{actionname}/Type/")]
        public IActionResult FrameworkType(DetailFramework detailFramework, string actionname, int frameworkId = 0)
        {
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
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
            [Route("/Frameworks/{actionname}/Brand/")]
        [Route("/Frameworks/{frameworkId}/{actionname}/Brand/")]
        public IActionResult SetNewFrameworkBrand(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            DetailFramework? framework;
            if (actionname == "New")
            {
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
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
        [Route("/Frameworks/{actionname}/Brand/")]
        [Route("/Frameworks/{frameworkId}/{actionname}/Brand/")]
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
                SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
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
            var centreId = GetCentreId();
            if (detailFramework.BrandID == null | detailFramework.CategoryID == null | detailFramework.TopicID == null)
            {
                return null;
            }
            if (detailFramework.BrandID == 0)
            {
                if (detailFramework.Brand != null)
                {
                    //create brand and set brand id
                    detailFramework.BrandID = commonService.InsertBrandAndReturnId(detailFramework.Brand, (int)centreId);
                }
                else
                {
                    return null;
                    //return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                }
            }
            if (detailFramework.CategoryID == 0)
            {
                if (detailFramework.Category != null)
                {
                    //create category and set category id to new category
                    detailFramework.CategoryID = commonService.InsertCategoryAndReturnId(detailFramework.Category, (int)centreId);
                }
                else
                {
                    return null;
                    //return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                }
            }
            if (detailFramework.TopicID == 0)
            {
                if (detailFramework.Topic != null)
                {
                    //create topic and set topic id to new topic
                    detailFramework.TopicID = commonService.InsertTopicAndReturnId(detailFramework.Topic, (int)centreId);
                }
                else
                {
                    return null;
                    //return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                }
            }
            return detailFramework;
        }
        [Route("/Frameworks/New/Summary")]
        public IActionResult FrameworkSummary()
        {
            SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
            TempData.Set(sessionNewFramework);
            return View("Developer/Summary", sessionNewFramework.DetailFramework);
        }
        [Route("/Frameworks/{frameworkId}/{actionname}/Collaborators")]
        public IActionResult AddCollaborators(string actionname, int frameworkId)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var adminList = commonService.GetOtherAdministratorsForCentre((int)centreId, adminId).Select(a => new { a.AdminID, a.Email }).ToList();
            var adminSelectList = new SelectList(adminList, "AdminID", "Email");
            string frameworkName = "";
            var collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            frameworkName = (string)framework.FrameworkName;
            var model = new CollaboratorsViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkName = frameworkName,
                AdminSelectList = adminSelectList,
                Collaborators = collaborators
            };
            return View("Developer/Collaborators", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/{actionname}/Collaborators")]
        public IActionResult AddCollaborator(string actionname, int adminId, bool canModify, int frameworkId)
        {
            frameworkService.AddCollaboratorToFramework(frameworkId, adminId, canModify);
            notificationService.SendFrameworkCollaboratorInvite(adminId, frameworkId, GetAdminID());
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionname });
        }
        public IActionResult RemoveCollaborator(int frameworkId, string actionname, int adminId)
        {
            frameworkService.RemoveCollaboratorFromFramework(frameworkId, adminId);
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionname });
        }
        [Route("/Framework/{frameworkId}/{tabname}/")]
        public IActionResult ViewFramework(string tabname, int frameworkId)
        {
            var adminId = GetAdminID();
            var detailFramework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
            var frameworkCompetencyGroups = frameworkService.GetFrameworkCompetencyGroups(frameworkId).ToList();
            var frameworkCompetencies = frameworkService.GetFrameworkCompetenciesUngrouped(frameworkId);
            var frameworkDefaultQuestions = frameworkService.GetFrameworkDefaultQuestionsById(frameworkId, adminId);
            var model = new FrameworkViewModel()
            {
                DetailFramework = detailFramework,
                Collaborators = collaborators,
                FrameworkCompetencyGroups = frameworkCompetencyGroups,
                FrameworkCompetencies = frameworkCompetencies,
                FrameworkDefaultQuestions = frameworkDefaultQuestions
            };
            return View("Developer/Framework", model);
        }
        public IActionResult InsertFramework()
        {
            var adminId = GetAdminID();
            SessionNewFramework sessionNewFramework = TempData.Get<SessionNewFramework>();
            DetailFramework? detailFramework = sessionNewFramework.DetailFramework;
            detailFramework = InsertBrandingCategoryTopicIfRequired(detailFramework);
            if (detailFramework == null || adminId < 1)
            {
                logger.LogWarning($"Failed to create framework: adminId: {adminId}");
                return StatusCode(500);
            }
            var newFramework = frameworkService.CreateFramework(detailFramework, adminId);
             return RedirectToAction("AddCollaborators", "Frameworks", new { actionname = "New", frameworkId = newFramework.ID });
        }

    }
}
