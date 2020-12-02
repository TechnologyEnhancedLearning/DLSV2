using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using Microsoft.AspNetCore.Mvc;
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
            return View("Developer/CreateFramework", newFramework);
        }
        [HttpPost]
        [Route("/Frameworks/New")]
        public IActionResult CreateNewFramework(BaseFramework baseFramework)
        {
            if (!ModelState.IsValid)
            {
                // do something
                return View("Developer/CreateFramework", baseFramework);
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
            var similarItems = SearchHelper.FilterFrameworks(sortedItems, frameworkname, 50, true);
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
                return View("Developer/SimilarFrameworks", model);
            }
            else
            {
                //need to create framework and move to next step.
                return RedirectToAction("SetNewFrameworkBrand", "Frameworks", new { frameworkname });
            }

        }
        [Route("/Frameworks/New/Brand")]
        public IActionResult SetNewFrameworkBrand(string frameworkname)
        {
            var adminId = GetAdminID();
            var frameworkId = frameworkService.CreateFramework(frameworkname, adminId);
            if (frameworkId <= 0)
            {
                logger.LogWarning($"Failed to insert n {adminId} without Framework Developer role.");
                return StatusCode(500);
            }
            return StatusCode(403);
        }
    }
}
