using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [HttpPost]
        [Route("/Framework/{frameworkId}/Comments/")]
        public IActionResult InsertComment(int frameworkId, string comment)
        {
            var adminId = GetAdminID();
            frameworkService.InsertComment(frameworkId, adminId, comment, null);
            return RedirectToAction("ViewFramework", new { tabname = "Comments", frameworkId });
        }
        [Route("/Framework/{frameworkId}/Comments/{commentId}")]
        public IActionResult ViewThread(int frameworkId, int commentId)
        {
            var commentReplies = frameworkService.GetCommentById(commentId, GetAdminID());
            return View("Developer/CommentThread", commentReplies);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/Comments/{commentId}")]
        public IActionResult InsertReply(int frameworkId, int commentId, string comment)
        {
            var adminId = GetAdminID();
            frameworkService.InsertComment(frameworkId, adminId, comment, commentId);
            return RedirectToAction("ViewThread", new { frameworkId, commentId });
        }
    }
    
}
