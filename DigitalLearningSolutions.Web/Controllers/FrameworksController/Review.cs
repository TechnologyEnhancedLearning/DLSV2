namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Frameworks;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/Review")]
        public IActionResult SendForReview(int frameworkId)
        {
            var adminId = GetAdminId();
            var collaborators = frameworkService.GetReviewersForFrameworkId(frameworkId);
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (framework == null) return StatusCode(404);
            if (framework.UserRole < 2)
                return StatusCode(403);
            var model = new CollaboratorsViewModel()
            {
                BaseFramework = framework,
                Collaborators = collaborators
            };
            return View("Developer/Review", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/Review")]
        public IActionResult SubmitReviewers(int frameworkId, List<int> userChecked, List<int> signOffRequiredChecked)
        {
            var adminId = GetAdminId();
            foreach (var user in userChecked)
            {
                var required = signOffRequiredChecked.IndexOf(user) != -1;
                frameworkService.InsertFrameworkReview(frameworkId, user, required);
                frameworkNotificationService.SendReviewRequest(user, adminId, required, false, User.GetCentreIdKnownNotNull());
            }
            frameworkService.UpdateFrameworkStatus(frameworkId, 2, adminId);
            return RedirectToAction("ViewFrameworkReviews", "Frameworks", new { frameworkId });
        }
        [Route("/Framework/{frameworkId}/Publish")]
        public IActionResult ViewFrameworkReviews(int frameworkId)
        {
            var adminId = GetAdminId();
            var framework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
            if (framework == null) return StatusCode(404);
            if (framework.UserRole < 2) return StatusCode(403);
            var frameworkReviews = frameworkService.GetFrameworkReviewsForFrameworkId(frameworkId);
            var canPublish = true;
            var enumerableFrameworkReviews = frameworkReviews.ToList();
            foreach (var unused in enumerableFrameworkReviews.Where(frameworkReview => frameworkReview.SignOffRequired &&
                         (frameworkReview.ReviewComplete == null | !frameworkReview.SignedOff))) canPublish = false;
            var model = new PublishViewModel()
            {
                DetailFramework = framework,
                FrameworkReviews = enumerableFrameworkReviews,
                CanPublish = canPublish
            };
            return View("Developer/Publish", model);
        }
        [Route("/Framework/{frameworkId}/Review/{reviewId}")]
        public IActionResult LoadReview(int frameworkId, int reviewId)
        {
            var adminId = GetAdminId();
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (framework == null) return StatusCode(404);
            if (framework.UserRole < 1) return StatusCode(403);
            var frameworkName = framework.FrameworkName;
            var frameworkReview = frameworkService.GetFrameworkReview(frameworkId, adminId, reviewId);
            if (frameworkReview == null) return StatusCode(403);
            var model = new SubmitReviewViewModel()
            {
                FrameworkId = frameworkId,
                FrameworkName = frameworkName,
                FrameworkReview = frameworkReview
            };
            return View("Developer/SubmitReview", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/Review/{reviewId}")]
        public IActionResult SubmitFrameworkReview(int frameworkId, int reviewId, string? comment, bool signedOff)
        {
            var adminId = GetAdminId();
            int? commentId = null;
            if (!string.IsNullOrWhiteSpace(comment)) commentId = frameworkService.InsertComment(frameworkId, adminId, comment, null);
            frameworkService.SubmitFrameworkReview(frameworkId, reviewId, signedOff, commentId);
            frameworkNotificationService.SendReviewOutcomeNotification(reviewId, User.GetCentreIdKnownNotNull());
            return RedirectToAction("ViewFramework", "Frameworks", new { frameworkId, tabname = "Structure" });
        }
        public IActionResult ResendRequest(int reviewId, int frameworkId, int frameworkCollaboratorId, bool required)
        {
            var adminId = GetAdminId();
            frameworkNotificationService.SendReviewRequest(frameworkCollaboratorId, adminId, required, true, User.GetCentreIdKnownNotNull());
            frameworkService.UpdateReviewRequestedDate(reviewId);
            return RedirectToAction("ViewFrameworkReviews", "Frameworks", new { frameworkId });
        }
        public IActionResult RequestReReview(int frameworkId, int reviewId)
        {
            var adminId = GetAdminId();
            frameworkService.InsertFrameworkReReview(reviewId);
            var review = frameworkService.GetFrameworkReviewNotification(reviewId);
            if (review == null) return StatusCode(404);
            frameworkNotificationService.SendReviewRequest(review.FrameworkCollaboratorID, adminId, review.SignOffRequired, false, User.GetCentreIdKnownNotNull());
            return RedirectToAction("ViewFrameworkReviews", "Frameworks", new { frameworkId });
        }
        public IActionResult RemoveRequest(int frameworkId, int reviewId)
        {
            frameworkService.ArchiveReviewRequest(reviewId);
            return RedirectToAction("ViewFrameworkReviews", "Frameworks", new { frameworkId });
        }
        public IActionResult PublishFramework(int frameworkId)
        {
            var adminId = GetAdminId();
            frameworkService.UpdateFrameworkStatus(frameworkId, 3, adminId);
            return RedirectToAction("ViewFramework", "Frameworks", new { frameworkId, tabname = "Structure" });
        }
    }
}
