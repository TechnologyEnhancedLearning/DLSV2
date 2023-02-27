namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    using Microsoft.AspNetCore.Mvc;
    public partial class FrameworksController
    {
        [HttpPost]
        [Route("/Framework/{frameworkId}/Comments/")]
        public IActionResult InsertComment(int frameworkId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return RedirectToAction("ViewFramework", new { tabname = "Comments", frameworkId });
            var adminId = GetAdminId();
            var newCommentId = frameworkService.InsertComment(frameworkId, adminId, comment, null);
            frameworkNotificationService.SendCommentNotifications(adminId, frameworkId, newCommentId, comment, null, null);
            return RedirectToAction("ViewFramework", new { tabname = "Comments", frameworkId });
        }
        [Route("/Framework/{frameworkId}/Comments/{commentId}")]
        public IActionResult ViewThread(int frameworkId, int commentId)
        {
            var adminId = GetAdminId();
            var baseFramework = frameworkService.GetBaseFrameworkByFrameworkId(frameworkId, adminId);
            if (baseFramework == null) return StatusCode(404);
            if (baseFramework.UserRole == 0)
            {
                return StatusCode(403);
            }
            var commentReplies = frameworkService.GetCommentRepliesById(commentId, GetAdminId());
            return View("Developer/CommentThread", commentReplies);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/Comments/{commentId}")]
        public IActionResult InsertReply(int frameworkId, int commentId, string comment, string parentComment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return RedirectToAction("ViewThread", new { frameworkId, commentId });
            var adminId = GetAdminId();
            var newCommentId = frameworkService.InsertComment(frameworkId, adminId, comment, commentId);
            frameworkNotificationService.SendCommentNotifications(adminId, frameworkId, newCommentId, comment, commentId, parentComment);
            return RedirectToAction("ViewThread", new { frameworkId, commentId });
        }
        public IActionResult ArchiveReply(int commentId, int replyToCommentId, int frameworkId)
        {
            frameworkService.ArchiveComment(commentId);
            return RedirectToAction("ViewThread", new { frameworkId, commentId = replyToCommentId });
        }
        public IActionResult ArchiveComment(int commentId, int frameworkId)
        {
            frameworkService.ArchiveComment(commentId);
            return RedirectToAction("ViewFramework", new { tabname = "Comments", frameworkId });
        }
    }

}
