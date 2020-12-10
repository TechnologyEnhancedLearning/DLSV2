using DigitalLearningSolutions.Data.Models.Common;
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
        [Route("/Frameworks/MyFrameworks")]
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
        [Route("/Frameworks/AllFrameworks")]
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
        [Route("/Frameworks/New")]
        public IActionResult CreateNewFramework()
        {
            var adminId = GetAdminID();
            var newFramework = new BaseFramework()
            {
                BrandID = 6,
                OwnerAdminID = adminId,
                UpdatedByAdminID = adminId,
                TopicID = 1,
                CategoryID = 1,
                PublishStatusID = 1
            };
            return View("Developer/CreateName", newFramework);
        }
        [HttpPost]
        [Route("/Frameworks/New")]
        public IActionResult CreateNewFramework(BaseFramework baseFramework)
        {
            if (!ModelState.IsValid)
            {
                // do something
                return View("Developer/CreateName", baseFramework);
            }
            else
            {

                return RedirectToAction("SetNewFrameworkName", new { frameworkname = baseFramework.FrameworkName });
            }
        }
        [Route("/Frameworks/New/Similar")]
        public IActionResult SetNewFrameworkName(string frameworkname)
        {
            var adminId = GetAdminID();
            var sameItems = frameworkService.GetFrameworkDetailByFrameworkName(frameworkname, adminId);
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
                var model = new CreateSimilarViewModel()
                {
                    FrameworkName = frameworkname,
                    MatchingSearchResults = matchingSearchResults,
                    SimilarFrameworks = similarItems,
                    SameFrameworks = sameItems
                };
                return View("Developer/CreateSimilar", model);
            }
            else
            {
                return RedirectToAction("SaveNewFramework", "Frameworks", new { frameworkname });
            }

        }
        public IActionResult SaveNewFramework(string frameworkname)
        {
            var framework = frameworkService.CreateFramework(frameworkname, GetAdminID());
            //need to create framework and move to next step.
            return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkId = framework.Id });
        }
        [Route("/Frameworks/New/Brand/{frameworkId}")]
        public IActionResult SetNewFrameworkBrand(int frameworkId)
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
            var model = new CreateBrandingViewModel()
            {
                BrandedFramework = framework,
                BrandSelectList = brandSelectList,
                CategorySelectList = categorySelectList,
                TopicSelectList = topicSelectList
            };
            return View("Developer/CreateBranding", model);
        }
        [HttpPost]
        [Route("/Frameworks/New/Brand/{frameworkId}")]
        public IActionResult SetNewFrameworkBrand(BrandedFramework brandedFramework, int frameworkId)
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
            return RedirectToAction("AddContributors", "Frameworks", new { frameworkId });
        }
        [Route("/Frameworks/New/Contributors/{frameworkId}")]
        public IActionResult AddContributors(int frameworkId)
        {
            return StatusCode(500);
        }
    }
}
