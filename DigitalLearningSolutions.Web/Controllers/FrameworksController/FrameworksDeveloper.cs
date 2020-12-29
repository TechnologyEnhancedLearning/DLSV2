using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
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
        [Route("/Frameworks/{actionname}/Name/{frameworkId}")]
        [Route("/Frameworks/{actionname}/Name")]
        public IActionResult CreateNewFramework(string actionname, int frameworkId = 0)
        {
            var adminId = GetAdminID();
            BaseFramework? baseFramework;
            if(frameworkId > 0)
            {
                baseFramework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            }
            else
            {
                baseFramework = new BaseFramework()
            {
                BrandID = 6,
                OwnerAdminID = adminId,
                UpdatedByAdminID = adminId,
                TopicID = 1,
                CategoryID = 1,
                PublishStatusID = 1
            };
            }
            return View("Developer/Name", baseFramework);
        }
        [HttpPost]
        [Route("/Frameworks/{actionname}/Name/{frameworkId}")]
        [Route("/Frameworks/{actionname}/Name")]
        public IActionResult CreateNewFramework(BaseFramework baseFramework, string actionname, int frameworkId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(BaseFramework.FrameworkName));
                ModelState.AddModelError(nameof(BaseFramework.FrameworkName), "Please enter a valid framework name (between 3 and 255 characters)");
                // do something
                return View("Developer/Name", baseFramework);
            }
            else
            {
                if(actionname == "New")
                {
                    return RedirectToAction("SetNewFrameworkName", new { frameworkname = baseFramework.FrameworkName, actionname });
                }
                else
                {
                    var adminId = GetAdminID();
                    var isUpdated =  frameworkService.UpdateFrameworkName(baseFramework.ID, adminId, baseFramework.FrameworkName);
                    if(isUpdated)
                    {
                        return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(BaseFramework.FrameworkName), "Another framework exists with that name. Please choose a different name.");
                        // do something
                        return View("Developer/Name", baseFramework);
                    }
                }
                
            }
        }
        [Route("/Frameworks/{actionname}/Similar")]
        public IActionResult SetNewFrameworkName(string frameworkname, string actionname)
        {
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
            var framework = frameworkService.CreateFramework(frameworkname, GetAdminID());
            //need to create framework and move to next step.
            if (actionname == "New")
            {
                return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId = framework.ID, actionname });
            }
            else
            {
                return StatusCode(500);
            }
        }
        [Route("/Frameworks/{actionname}/Brand/{frameworkId}")]
        public IActionResult SetNewFrameworkBrand(int frameworkId, string actionname)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var framework = frameworkService.GetBrandedFrameworkByFrameworkId(frameworkId, adminId);
            if (framework == null | centreId == null)
            {
                logger.LogWarning($"Failed to load branding page for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                return StatusCode(500);
            }
            var brandsList = commonService.GetBrandListForCentre((int)centreId).Select(b => new { b.BrandID, b.BrandName }).ToList();
            var categoryList = commonService.GetCategoryListForCentre((int)centreId).Select(c => new { c.CourseCategoryID, c.CategoryName }).ToList();
            var topicList = commonService.GetTopicListForCentre((int)centreId).Select(t => new { t.CourseTopicID, t.CourseTopic }).ToList();
            var brandSelectList = new SelectList(brandsList, "BrandID", "BrandName");
            var categorySelectList = new SelectList(categoryList, "CourseCategoryID", "CategoryName");
            var topicSelectList = new SelectList(topicList, "CourseTopicID", "CourseTopic");
            var model = new BrandingViewModel()
            {
                BrandedFramework = framework,
                BrandSelectList = brandSelectList,
                CategorySelectList = categorySelectList,
                TopicSelectList = topicSelectList
            };
            return View("Developer/Branding", model);
        }
        [HttpPost]
        [Route("/Frameworks/{actionname}/Brand/{frameworkId}")]
        public IActionResult SetNewFrameworkBrand(BrandedFramework brandedFramework, int frameworkId, string actionname)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            if (brandedFramework.BrandID == 0)
            {
                if (brandedFramework.Brand != null)
                {
                    //create brand and set brand id
                    brandedFramework.BrandID = commonService.InsertBrandAndReturnId(brandedFramework.Brand, (int)centreId);
                }
                else
                {
                    return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                }
            }
            if (brandedFramework.CategoryID == 0)
            {
                if (brandedFramework.Category != null)
                {
                    //create category and set category id to new category
                    brandedFramework.CategoryID = commonService.InsertCategoryAndReturnId(brandedFramework.Category, (int)centreId);
                }
                else
                {
                    return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                }
            }
            if (brandedFramework.TopicID == 0)
            {
                if (brandedFramework.Topic != null)
                {
                    //create topic and set topic id to new topic
                    brandedFramework.TopicID = commonService.InsertTopicAndReturnId(brandedFramework.Topic, (int)centreId);
                }
                else
                {
                    return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId });
                }
            }
            if (brandedFramework.BrandID == null | brandedFramework.CategoryID == null | brandedFramework.TopicID == null)
            {
                logger.LogWarning($"Failed to update branding for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                return StatusCode(500);
            }
            var updatedFramework = frameworkService.UpdateFrameworkBranding(frameworkId, (int)brandedFramework.BrandID, (int)brandedFramework.CategoryID, (int)brandedFramework.TopicID, adminId);
            if (updatedFramework == null)
            {
                logger.LogWarning($"Failed to update branding for frameworkID: {frameworkId} adminId: {adminId}, centreId: {centreId}");
                return StatusCode(500);
            }
            if (actionname == "New")
            {
                return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionname });
            }
            else
            {
                return RedirectToAction("ViewFramework", new { tabname = "Details", frameworkId });
            }
        }
        [Route("/Frameworks/{actionname}/Collaborators/{frameworkId}")]
        public IActionResult AddCollaborators(int frameworkId, string actionname)
        {
            var adminId = GetAdminID();
            var centreId = GetCentreId();
            var adminList = commonService.GetOtherAdministratorsForCentre((int)centreId, adminId).Select(a => new { a.AdminID, a.Email}).ToList();
            var adminSelectList = new SelectList(adminList, "AdminID", "Email");
            var collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            var model = new CollaboratorsViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkName = (string)framework.FrameworkName,
                AdminSelectList = adminSelectList,
                Collaborators = collaborators
            };
            return View("Developer/Collaborators", model);
        }
        [HttpPost]
        [Route("/Frameworks/{actionname}/Collaborators/{frameworkId}")]
        public IActionResult AddCollaborator(int frameworkId, string actionname, int adminId, bool canModify)
        {
            frameworkService.AddCollaboratorToFramework(frameworkId, adminId, canModify);
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionname });
        }
        public IActionResult RemoveCollaborator(int frameworkId, string actionname, int adminId)
        {
            frameworkService.RemoveCollaboratorFromFramework(frameworkId, adminId);
            return RedirectToAction("AddCollaborators", "Frameworks", new { frameworkId, actionname });
        }
        [Route("/Framework/{tabname}/{frameworkId}")]
        public IActionResult ViewFramework(string tabname, int frameworkId)
        {
            var adminId = GetAdminID();
            var detailFramework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var collaborators = frameworkService.GetCollaboratorsForFrameworkId(frameworkId);
            var frameworkCompetencyGroups = frameworkService.GetFrameworkCompetencyGroups(frameworkId).ToList();
            var frameworkCompetencies = frameworkService.GetFrameworkCompetenciesUngrouped(frameworkId);
            var model = new FrameworkViewModel()
            {
                DetailFramework = detailFramework,
                Collaborators = collaborators,
                FrameworkCompetencyGroups = frameworkCompetencyGroups,
                FrameworkCompetencies = frameworkCompetencies
            };
            return View("Developer/Framework", model);
        }
    }
}
