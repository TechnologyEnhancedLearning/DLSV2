﻿using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Dashboard;
using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {

        [Route("/Frameworks/{frameworkId}/CompetencyGroup/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/CompetencyGroup")]
        public IActionResult AddEditFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId = 0)
        {
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminID());
            var frameworkConfig = frameworkService.GetFrameworkConfigForFrameworkId(frameworkId);
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                    return StatusCode(403);
            }
            CompetencyGroupBase competencyGroupBase = new CompetencyGroupBase();
            if (frameworkCompetencyGroupId > 0)
            {
                competencyGroupBase = frameworkService.GetCompetencyGroupBaseById(frameworkCompetencyGroupId);
            }
            var model = new CompetencyGroupViewModel()
            {
                DetailFramework = detailFramework,
                CompetencyGroupBase = competencyGroupBase
            };
            return View("Developer/CompetencyGroup", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/CompetencyGroup/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/CompetencyGroup")]
        public IActionResult AddEditFrameworkCompetencyGroup(int frameworkId, CompetencyGroupBase competencyGroupBase, int frameworkCompetencyGroupId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(CompetencyGroupBase.Name));
                ModelState.AddModelError(nameof(CompetencyGroupBase.Name), "Please enter a valid competency group name (between 3 and 255 characters)");
                // do something
                var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminID());
                var model = new CompetencyGroupViewModel()
                {
                    DetailFramework = detailFramework,
                    CompetencyGroupBase = competencyGroupBase
                };
                return View("Developer/CompetencyGroup", model);
            }
            else
            {
                var adminId = GetAdminID();
                var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
                if (userRole < 2)
                {
                    return StatusCode(403);
                }
                if (competencyGroupBase.ID > 0)
                {
                    frameworkService.UpdateFrameworkCompetencyGroup(frameworkCompetencyGroupId, competencyGroupBase.CompetencyGroupID, competencyGroupBase.Name, adminId);
                    return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
                }
                else
                {
                    var newCompetencyGroupId = frameworkService.InsertCompetencyGroup(competencyGroupBase.Name, adminId);
                    if (newCompetencyGroupId > 0)
                    {
                        var newFrameworkCompetencyGroupId = frameworkService.InsertFrameworkCompetencyGroup(newCompetencyGroupId, frameworkId, adminId);
                        return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId = newFrameworkCompetencyGroupId }) + "#fcgroup-" + newFrameworkCompetencyGroupId.ToString());
                    }
                }
                logger.LogWarning($"Attempt to add framework competency group failed for admin {adminId}.");
                return StatusCode(403);
            }
        }
        public IActionResult MoveFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId, bool step, string direction)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.MoveFrameworkCompetencyGroup(frameworkCompetencyGroupId, step, direction);
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
        }

        public IActionResult DeleteFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId, int competencyGroupId)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.DeleteFrameworkCompetencyGroup(frameworkCompetencyGroupId, competencyGroupId, GetAdminID());
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/Competency/")]
        public IActionResult AddEditFrameworkCompetency(int frameworkId, int? frameworkCompetencyGroupId, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            FrameworkCompetency frameworkCompetency = new FrameworkCompetency();
            if (frameworkCompetencyId > 0)
            {
                frameworkCompetency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            }
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
            var model = new FrameworkCompetencyViewModel()
            {
                DetailFramework = detailFramework,
                FrameworkCompetencyGroupId = frameworkCompetencyGroupId,
                FrameworkCompetency = frameworkCompetency
            };
            return View("Developer/Competency", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/Competency/")]
        public IActionResult AddEditFrameworkCompetency(int frameworkId, FrameworkCompetency frameworkCompetency, int? frameworkCompetencyGroupId, int frameworkCompetencyId = 0)
        {
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(FrameworkCompetency.Name));
                ModelState.AddModelError(nameof(FrameworkCompetency.Name), "Please enter a valid competency statement (between 3 and 500 characters)");
                // do something
                var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminID());
                var model = new FrameworkCompetencyViewModel()
                {
                    DetailFramework = detailFramework,
                    FrameworkCompetencyGroupId = frameworkCompetencyId,
                    FrameworkCompetency = frameworkCompetency
                };
                return View("Developer/Competency", model);
            }
            else
            {
                var adminId = GetAdminID();
                var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
                if (userRole < 2)
                {
                    return StatusCode(403);
                }
                if (frameworkCompetency.Id > 0)
                {
                    frameworkService.UpdateFrameworkCompetency(frameworkCompetencyId, frameworkCompetency.Name, frameworkCompetency.Description, adminId);
                    return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId, frameworkCompetencyId }) + "#fc-" + frameworkCompetencyId.ToString());
                }
                else
                {
                    var newCompetencyId = frameworkService.InsertCompetency(frameworkCompetency.Name, frameworkCompetency.Description, adminId);
                    if (newCompetencyId > 0)
                    {
                        var newFrameworkCompetencyId = frameworkService.InsertFrameworkCompetency(newCompetencyId, frameworkCompetencyGroupId, adminId, frameworkId);
                        return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId, frameworkCompetencyId }) + "#fc-" + newFrameworkCompetencyId.ToString());
                    }
                }
                logger.LogWarning($"Attempt to add framework competency failed for admin {adminId}.");
                return StatusCode(403);
            }
        }
        public IActionResult MoveFrameworkCompetency(int frameworkId, int frameworkCompetencyGroupId, int frameworkCompetencyId, bool step, string direction)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.MoveFrameworkCompetency(frameworkCompetencyId, step, direction);
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId, frameworkCompetencyId }) + "#fc-" + frameworkCompetencyId.ToString());
        }
        public IActionResult DeleteFrameworkCompetency(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminID(), frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            frameworkService.DeleteFrameworkCompetency(frameworkCompetencyId, GetAdminID());
            if (frameworkCompetencyGroupId != null)
            {
                return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId , frameworkCompetencyGroupId}) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
            }
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fc-ungrouped");
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}/Preview/")]
        public IActionResult PreviewCompetency(int frameworkId, int frameworkCompetencyGroupId, int frameworkCompetencyId)
        {
            var adminId = GetAdminID();
            var assessment = new CurrentSelfAssessment()
            {
                LaunchCount = 0,
                UnprocessedUpdates = false
            };
            var competency = frameworkService.GetFrameworkCompetencyForPreview(frameworkCompetencyId);
            if (competency != null)
            {
                foreach (var assessmentQuestion in competency.AssessmentQuestions)
                {
                    assessmentQuestion.LevelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(assessmentQuestion.Id, adminId, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.MinValue == 0).ToList();
                }
                var model = new SelfAssessmentCompetencyViewModel(assessment, competency, 1, 1);
                return View("Developer/CompetencyPreview", model);
            }
            logger.LogWarning($"Attempt to preview competency failed for frameworkCompetencyId {frameworkCompetencyId}.");
            return StatusCode(500);
        }
    }
}
