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
        [Route("/Frameworks/{frameworkId}/CompetencyGroup/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/CompetencyGroup")]
        public IActionResult AddEditFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId = 0)
        {
            CompetencyGroupBase competencyGroupBase = new CompetencyGroupBase();
            if(frameworkCompetencyGroupId > 0)
            {
                competencyGroupBase = frameworkService.GetCompetencyGroupBaseById(frameworkCompetencyGroupId);
            }
            var model = new CompetencyGroupViewModel()
            {
                FrameworkId = frameworkId,
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
                var model = new CompetencyGroupViewModel()
                {
                    FrameworkId = frameworkId,
                    CompetencyGroupBase = competencyGroupBase
                };
                return View("Developer/CompetencyGroup", model);
            }
            else
            {
                var adminId = GetAdminID();
                if (competencyGroupBase.ID > 0)
                {
                    frameworkService.UpdateFrameworkCompetencyGroup(frameworkCompetencyGroupId, competencyGroupBase.CompetencyGroupID, competencyGroupBase.Name, adminId);
                }
                else
                {
                    var newCompetencyGroupId = frameworkService.InsertCompetencyGroup(competencyGroupBase.Name, adminId);
                    if (newCompetencyGroupId > 0)
                    {
                        var newFrameworkCompetencyGroupId = frameworkService.InsertFrameworkCompetencyGroup(newCompetencyGroupId, frameworkId, adminId);
                    }
                }
                return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
            }
        }
        public IActionResult MoveFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId, bool step, string direction)
        {
            frameworkService.MoveFrameworkCompetencyGroup(frameworkCompetencyGroupId, step, direction);
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fcgroup-" + frameworkCompetencyGroupId.ToString());
        }

        public IActionResult DeleteFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}")]
        public IActionResult AddEditFrameworkCompetency(int frameworkId, int frameworkCompetencyGroupId = 0, int frameworkCompetencyId = 0)
        {
            return View("Developer/Competency");
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}/{frameworkCompetencyId}")]
        [Route("/Frameworks/{frameworkId}/Competency/{frameworkCompetencyGroupId}")]
        public IActionResult AddEditFrameworkCompetency(int frameworkId, FrameworkCompetency frameworkCompetency, int frameworkCompetencyGroupId, int frameworkCompetencyId = 0)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
        public IActionResult MoveFrameworkCompetency(int frameworkId, int frameworkCompetencyId, bool step, string direction)
        {
            frameworkService.MoveFrameworkCompetency(frameworkCompetencyId, step, direction);
            return new RedirectResult(Url.Action("ViewFramework", new { tabname = "Structure", frameworkId }) + "#fc-" + frameworkCompetencyId.ToString());
        }
        public IActionResult DeleteFrameworkCompetency(int frameworkId, int frameworkCompetencyId)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
    }
}
