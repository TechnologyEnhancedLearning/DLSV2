using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            if(isFrameworkDeveloper == false | isFrameworkDeveloper == null)
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
            return View("Developer/CreateFramework");
        }
        [HttpPost]
        [Route("/Frameworks/New/Similar")]
        public IActionResult SetNewFrameworkName(string frameworkname)
        {
            var adminId = GetAdminID();
            var frameworks = frameworkService.GetAllFrameworks(adminId);
            var model = new AllFrameworksViewModel(frameworks,
               frameworkname,
               FrameworkSortByOptionTexts.FrameworkName,
               BaseFrameworksPageViewModel.AscendingText,
               1);
            return View("Developer/Similar", model);
        }
    }
}
