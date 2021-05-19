﻿using DigitalLearningSolutions.Data.Models.Frameworks;
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
        public IActionResult SubmitReviewers(int frameworkId, List<int> userChecked, List<int> signOffRequiredChecked)
        {
            var adminId = GetAdminID();
            foreach(var user in userChecked)
            {
                var required = signOffRequiredChecked.IndexOf(user) != -1;
                frameworkService.InsertFrameworkReview(frameworkId, user, required);
                frameworkNotificationService.SendReviewRequest(user, adminId, required);
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
            var frameworkReviews = frameworkService.GetFrameworkReviewsForFrameworkId(frameworkId);
            var model = new PublishViewModel()
            {
                BaseFramework = framework,
                FrameworkReviews = frameworkReviews
            };
            return View("Developer/Publish", model);
        }
        [Route("/Framework/{frameworkId}/Review/{reviewId}")]
        public IActionResult LoadReview(int frameworkId, int reviewId)
        {
            var adminId = GetAdminID();
            var framework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (framework.UserRole < 1)
            {
                return StatusCode(403);
            }
            var frameworkName = (string)framework.FrameworkName;
            var frameworkReview = frameworkService.GetFrameworkReview(frameworkId, adminId, reviewId);
            if (frameworkReview == null)
            {
                return StatusCode(403);
            }
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
            var adminId = GetAdminID();
            int? commentId = null;
            if(comment != null)
            {
                commentId = frameworkService.InsertComment(frameworkId, adminId, comment, null);
            }
            frameworkService.SubmitFrameworkReview(frameworkId, reviewId, signedOff, commentId);
            frameworkNotificationService.SendReviewOutcomeNotification(reviewId);
            return RedirectToAction("ViewFramework", "Frameworks", new { frameworkId , tabname = "Structure"});
        }
    }
}
