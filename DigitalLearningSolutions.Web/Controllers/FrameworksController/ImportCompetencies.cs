using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/{tabname}/Import")]
        public IActionResult ImportCompetencies(int frameworkId, string tabname)
        {
            var adminId = GetAdminId();
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
                return StatusCode(403);
            var model = new ImportCompetenciesViewModel()
            {
                FrameworkId = frameworkId
            };
            return View("Developer/ImportCompetencies", model);
        }
        [Route("DownloadDelegates")]
        public IActionResult DownloadCompetencies(int frameworkId, int DownloadOption)
        {
            string fileName = DownloadOption == 2 ? $"DLS Competencies for Bulk Update {clockUtility.UtcToday:yyyy-MM-dd}.xlsx" : "DLS Delegates for Bulk Registration.xlsx";
            var content = importCompetenciesFromFileService.GetCompetencyFileForFramework(
                    frameworkId, DownloadOption == 2 ? false : true
                );
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
        [HttpPost]
        [Route("/Framework/{frameworkId}/{tabname}/Import")]
        public IActionResult StartImport(ImportCompetenciesViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Developer/ImportCompetencies", model);
            try
            {
                var results = importCompetenciesFromFileService.ProcessCompetenciesFromFile(
                    model.ImportFile!,
                    GetAdminId(),
                    model.FrameworkId
                );
                var resultsModel = new ImportCompetenciesResultsViewModel(results);
                return View("Developer/ImportCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                return View("Developer/ImportFailed");
            }
        }
    }
}
