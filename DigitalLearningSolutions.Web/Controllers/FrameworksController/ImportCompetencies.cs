using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Data.Services;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/Structure/Import")]
        public IActionResult ImportCompetencies(int frameworkId)
        {
            var adminId = GetAdminID();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
            {
                return StatusCode(403);
            }
            var model = new ImportCompetenciesViewModel() {
                FrameworkId = frameworkId
            };
            return View("Developer/ImportCompetencies", model);
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/Structure/Import")]
        public IActionResult StartImport(ImportCompetenciesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Developer/ImportCompetencies", model);
            }
            try
            {
                var results = importCompetenciesFromFileService.ProcessCompetenciesFromFile(
                    model.ImportFile!,
                    GetAdminID(),
                    model.FrameworkId
                );
                var resultsModel = new ImportCompetenciesResultsViewModel(results);
                return View("ImportCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                return View("ImportFailed");
            }
        }
    }
}
