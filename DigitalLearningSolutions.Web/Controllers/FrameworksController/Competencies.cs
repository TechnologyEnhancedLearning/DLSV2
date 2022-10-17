using DigitalLearningSolutions.Data.Models.Frameworks;
using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Web;
using System.Net;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Frameworks/{frameworkId}/CompetencyGroup/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/CompetencyGroup")]
        public IActionResult AddEditFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId = 0)
        {
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminId());
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2) return StatusCode(403);
            var competencyGroupBase = new CompetencyGroupBase();
            if (frameworkCompetencyGroupId > 0) competencyGroupBase = frameworkService.GetCompetencyGroupBaseById(frameworkCompetencyGroupId);
            if (detailFramework == null || competencyGroupBase == null) return StatusCode(404);
            var model = new CompetencyGroupViewModel()
            {
                DetailFramework = detailFramework,
                CompetencyGroupBase = competencyGroupBase,
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
                if (ModelState["Name"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState.Remove(nameof(CompetencyGroupBase.Name));
                    ModelState.AddModelError(nameof(CompetencyGroupBase.Name), "Please enter a valid competency group name (between 3 and 255 characters)");
                }

                if (ModelState["Description"].ValidationState == ModelValidationState.Invalid)
                {
                    ModelState.Remove(nameof(CompetencyGroupBase.Description));
                    ModelState.AddModelError(nameof(CompetencyGroupBase.Description), "Please enter a valid competency group description (between 0 and 1000 characters)");
                }

                // do something
                var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminId());
                if (detailFramework == null) return StatusCode(404);
                var model = new CompetencyGroupViewModel()
                {
                    DetailFramework = detailFramework,
                    CompetencyGroupBase = competencyGroupBase
                };
                return View("Developer/CompetencyGroup", model);
            }
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            if (competencyGroupBase.ID > 0)
            {
                frameworkService.UpdateFrameworkCompetencyGroup(frameworkCompetencyGroupId, competencyGroupBase.CompetencyGroupID, competencyGroupBase.Name, SanitizerHelper.SanitizeHtmlData
             (competencyGroupBase.Description), adminId);
                return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
            }
            var newCompetencyGroupId = frameworkService.InsertCompetencyGroup(competencyGroupBase.Name, SanitizerHelper.SanitizeHtmlData(competencyGroupBase.Description), adminId);
            if (newCompetencyGroupId > 0)
            {
                var newFrameworkCompetencyGroupId = frameworkService.InsertFrameworkCompetencyGroup(newCompetencyGroupId, frameworkId, adminId);
                return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId = newFrameworkCompetencyGroupId }) + "#fcgroup-" + newFrameworkCompetencyGroupId.ToString());
            }
            logger.LogWarning($"Attempt to add framework competency group failed for admin {adminId}.");
            return StatusCode(403);
        }
        public IActionResult MoveFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId, bool step, string direction)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2) return StatusCode(403);
            frameworkService.MoveFrameworkCompetencyGroup(frameworkCompetencyGroupId, step, direction);
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
        }
        [Route("/Frameworks/{frameworkId}/CompetencyGroup/{frameworkCompetencyGroupId}/{competencyGroupId}/Remove/{competencyCount}/Confirm")]
        public IActionResult CompetencyGroupRemoveConfirm(int frameworkId, int frameworkCompetencyGroupId, int competencyGroupId, int competencyCount)
        {
            var model = new CompetencyGroupRemoveConfirmViewModel(frameworkId, frameworkCompetencyGroupId, competencyGroupId, competencyCount);

            return View("Developer/CompetencyGroupRemoveConfirm", model);
        }

        public IActionResult DeleteFrameworkCompetencyGroup(int frameworkId, int competencyGroupId, int frameworkCompetencyGroupId)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2) return StatusCode(403);

            var adminId = GetAdminId();

            frameworkService.DeleteFrameworkCompetencyGroup(frameworkCompetencyGroupId, competencyGroupId, adminId);

            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId }));
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/Competency/")]
        public IActionResult AddEditFrameworkCompetency(int frameworkId, int? frameworkCompetencyGroupId, int frameworkCompetencyId = 0)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode(403);
            var frameworkCompetency = new FrameworkCompetency();
            if (frameworkCompetencyId > 0)
            {
                frameworkCompetency = frameworkService.GetFrameworkCompetencyById(frameworkCompetencyId);
            }
            var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, adminId);
            var competencyFlags = frameworkService.GetCompetencyFlagsByFrameworkId(frameworkId, frameworkCompetency?.CompetencyID);
            if (detailFramework == null || frameworkCompetency == null) return StatusCode(404);
            var model = new FrameworkCompetencyViewModel()
            {
                DetailFramework = detailFramework,
                FrameworkCompetencyGroupId = frameworkCompetencyGroupId,
                FrameworkCompetency = frameworkCompetency,
                CompetencyFlags = competencyFlags
            };
            return View("Developer/Competency", model);
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/Competency/")]
        public IActionResult AddEditFrameworkCompetency(int frameworkId, FrameworkCompetency frameworkCompetency, int? frameworkCompetencyGroupId, int frameworkCompetencyId = 0, int[] selectedFlagIds = null)
        {
            frameworkCompetency.Description = SanitizerHelper.SanitizeHtmlData(frameworkCompetency.Description);
            frameworkCompetency.Description?.Trim();
            var description = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(frameworkCompetency.Description));
            if (string.IsNullOrWhiteSpace(description)) { frameworkCompetency.Description = null; }
            if (!ModelState.IsValid)
            {
                ModelState.Remove(nameof(FrameworkCompetency.Name));
                ModelState.AddModelError(nameof(FrameworkCompetency.Name), "Please enter a valid competency statement (between 3 and 500 characters)");
                var detailFramework = frameworkService.GetDetailFrameworkByFrameworkId(frameworkId, GetAdminId());
                var competencyFlags = frameworkService.GetCompetencyFlagsByFrameworkId(frameworkId, frameworkCompetency?.CompetencyID).ToList();
                if (competencyFlags != null)
                    competencyFlags.ForEach(f => f.Selected = selectedFlagIds.Contains(f.FlagId));
                if (detailFramework == null)
                    return StatusCode((int)HttpStatusCode.NotFound);
                var model = new FrameworkCompetencyViewModel()
                {
                    DetailFramework = detailFramework,
                    FrameworkCompetencyGroupId = frameworkCompetencyId,
                    FrameworkCompetency = frameworkCompetency,
                    CompetencyFlags = competencyFlags
                };
                return View("Developer/Competency", model);
            }
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2) return StatusCode((int)HttpStatusCode.Forbidden);
            if (frameworkCompetency.Id > 0)
            {
                

                frameworkService.UpdateFrameworkCompetency(frameworkCompetencyId, frameworkCompetency.Name, frameworkCompetency.Description, adminId);
                frameworkService.UpdateCompetencyFlags(frameworkId, frameworkCompetency.CompetencyID, selectedFlagIds);
                return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId, frameworkCompetencyId }) + "#fc-" + frameworkCompetencyId.ToString());
            }
            var newCompetencyId = frameworkService.InsertCompetency(frameworkCompetency.Name, frameworkCompetency.Description, adminId);
            if (newCompetencyId > 0)
            {
                var newFrameworkCompetencyId = frameworkService.InsertFrameworkCompetency(newCompetencyId, frameworkCompetencyGroupId, adminId, frameworkId);
                frameworkService.UpdateCompetencyFlags(frameworkId, newCompetencyId, selectedFlagIds);
                return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId, frameworkCompetencyId }) + "#fc-" + newFrameworkCompetencyId.ToString());
            }
            logger.LogWarning($"Attempt to add framework competency failed for admin {adminId}.");
            return StatusCode((int)HttpStatusCode.Forbidden);
        }
        public IActionResult MoveFrameworkCompetency(int frameworkId, int frameworkCompetencyGroupId, int frameworkCompetencyId, bool step, string direction)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2) return StatusCode(403);
            frameworkService.MoveFrameworkCompetency(frameworkCompetencyId, step, direction);
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId, frameworkCompetencyId }) + "#fc-" + frameworkCompetencyId.ToString());
        }
        public IActionResult DeleteFrameworkCompetency(int frameworkId, int frameworkCompetencyId, int? frameworkCompetencyGroupId)
        {
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(GetAdminId(), frameworkId);
            if (userRole < 2) return StatusCode(403);
            frameworkService.DeleteFrameworkCompetency(frameworkCompetencyId, GetAdminId());
            return frameworkCompetencyGroupId != null ? new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId, frameworkCompetencyGroupId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString()) : new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fc-ungrouped");
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}/Preview/")]
        public IActionResult PreviewCompetency(int frameworkId, int frameworkCompetencyGroupId, int frameworkCompetencyId)
        {
            var adminId = GetAdminId();
            var assessment = new CurrentSelfAssessment()
            {
                LaunchCount = 0,
                UnprocessedUpdates = false,
            };
            var competency = frameworkService.GetFrameworkCompetencyForPreview(frameworkCompetencyId);
            if (competency != null)
            {
                foreach (var assessmentQuestion in competency.AssessmentQuestions)
                {
                    assessmentQuestion.LevelDescriptors = frameworkService.GetLevelDescriptorsForAssessmentQuestionId(assessmentQuestion.Id, adminId, assessmentQuestion.MinValue, assessmentQuestion.MaxValue, assessmentQuestion.MinValue == 0).ToList();
                }

                var model = new SelfAssessmentCompetencyViewModel(assessment, competency, 1, 1);
                competency.CompetencyFlags = frameworkService.GetCompetencyFlagsByFrameworkId(frameworkId, competency.Id, selected: true);
                return View("Developer/CompetencyPreview", model);
            }
            logger.LogWarning($"Attempt to preview competency failed for frameworkCompetencyId {frameworkCompetencyId}.");
            return StatusCode(500);
        }
    }
}
