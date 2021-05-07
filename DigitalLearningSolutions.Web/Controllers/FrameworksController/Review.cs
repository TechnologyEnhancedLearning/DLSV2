using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/Review")]
        public IActionResult SendForReview(int frameworkId)
        {
            var adminId = GetAdminID();
            var collaborators = frameworkService.GetReviewersForFrameworkId(frameworkId);
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (framework.UserRole < 2)
            {
                return StatusCode(403);
            }
            var frameworkName = (string)framework.FrameworkName;
            var model = new CollaboratorsViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkName = frameworkName,
                Collaborators = collaborators
            };
            return View("Developer/Review", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/Review")]
        public IActionResult SubmitReviewers(int frameworkId, List<int> userChecked)
        {
            var adminId = GetAdminID();
            foreach(var user in userChecked)
            {
                frameworkService.InsertFrameworkReview(frameworkId, user);
                frameworkNotificationService.SendReviewRequest(user, adminId);
            }
            frameworkService.UpdateFrameworkStatus(frameworkId, 2, adminId);
            return RedirectToAction("PublishFramework", "Frameworks", new { frameworkId });
        }
        [Route("/Framework/{frameworkId}/Publish")]
        public IActionResult PublishFramework(int frameworkId)
        {
            var adminId = GetAdminID();
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (framework.UserRole < 2)
            {
                return StatusCode(403);
            }
            var frameworkName = (string)framework.FrameworkName;
            var frameworkReviews = frameworkService.GetFrameworkReviewsForFrameworkId(frameworkId);
            var model = new PublishViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkName = frameworkName,
                FrameworkReviews = frameworkReviews
            };
            return View("Developer/Publish", model);
        }
    }
}
