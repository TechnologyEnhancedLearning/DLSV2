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
            return View("Developer/CompetencyGroup");
        }
        [HttpPost]
        [Route("/Frameworks/{frameworkId}/CompetencyGroup/{frameworkCompetencyGroupId}")]
        [Route("/Frameworks/{frameworkId}/CompetencyGroup")]
        public IActionResult AddEditFrameworkCompetencyGroup(int frameworkId, FrameworkCompetencyGroup frameworkCompetencyGroup, int frameworkCompetencyGroupId = 0)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
        public IActionResult MoveUpFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
        public IActionResult MoveDownFrameworkCompetencyGroup(int frameworkId, int frameworkCompetencyGroupId)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
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
        public IActionResult MoveUpFrameworkCompetency(int frameworkId, int frameworkCompetencyId)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
        public IActionResult MoveDownFrameworkCompetency(int frameworkId, int frameworkCompetencyId)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
        public IActionResult DeleteFrameworkCompetency(int frameworkId, int frameworkCompetencyId)
        {
            return RedirectToAction("ViewFramework", new { tabname = "Structure", frameworkId });
        }
    }
}
