using ClosedXML.Excel;
using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.Frameworks.Import;
using GDS.MultiPageFormData.Enums;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace DigitalLearningSolutions.Web.Controllers.FrameworksController
{
    public partial class FrameworksController
    {
        [Route("/Framework/{frameworkId}/{tabname}/Import")]
        public IActionResult ImportCompetencies(int frameworkId, string tabname, bool isNotBlank)
        {
            var adminId = GetAdminId();
            var framework = frameworkService.GetFrameworkDetailByFrameworkId(frameworkId, adminId);
            var userRole = frameworkService.GetAdminUserRoleForFrameworkId(adminId, frameworkId);
            if (userRole < 2)
                return StatusCode(403);
            var model = new ImportCompetenciesViewModel()
            {
                Framework = framework,
                IsNotBlank = isNotBlank
            };
            return View("Developer/Import/Index", model);
        }
        public IActionResult DownloadCompetencies(int frameworkId, int DownloadOption)
        {
            string fileName = DownloadOption == 2 ? $"DLS Competencies for Bulk Update {clockUtility.UtcToday:yyyy-MM-dd}.xlsx" : "DLS Competencies for Bulk Upload.xlsx";
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
        [Route("/Framework/{frameworkId}/{tabname}/Import/Uploaded")]
        public IActionResult StartImport(ImportCompetenciesFormData model, int frameworkId, string tabname, bool isNotBlank)
        {
            if (!ModelState.IsValid)
                return View("Developer/Import/Index", model);
            try
            {
                var adminUserID = User.GetAdminIdKnownNotNull();
                var workbook = new XLWorkbook(model.ImportFile.OpenReadStream());
                if (!workbook.Worksheets.Contains(ImportCompetenciesFromFileService.CompetenciesSheetName))
                {
                    ModelState.AddModelError("ImportFile", CommonValidationErrorMessages.InvalidCompetenciesUploadExcelFile);
                    return View("Developer/Import/Index", model);
                }
                var competenciesFileName = FileHelper.UploadFile(webHostEnvironment, model.ImportFile);
                setupBulkUploadData(frameworkId, adminUserID, competenciesFileName, tabname, isNotBlank);

                return RedirectToAction("ImportCompleted", "Frameworks", new { frameworkId, tabname });
            }
            catch (DocumentFormat.OpenXml.Packaging.OpenXmlPackageException)
            {
                ModelState.AddModelError("DelegatesFile", "The Excel file has at least one cell containing an invalid hyperlink or email address.");
                return View("Index", model);
            }
            catch (InvalidHeadersException)
            {
                return View("Developer/Import/ImportFailed");
            }
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/Uploaded")]
        public IActionResult ImportCompleted()
        {
            var data = GetBulkUploadData();
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            var filePath = Path.Combine(uploadDir, data.CompetenciesFileName);
            var workbook = new XLWorkbook(filePath);
            try
            {
                var results = importCompetenciesFromFileService.PreProcessCompetenciesTable(workbook);
                var resultsModel = new ImportCompetenciesPreProcessViewModel(results) { IsNotBlank = data.IsNotBlank, TabName = data.TabName };
                data.CompetenciesToProcessCount = resultsModel.ToProcessCount;
                data.CompetenciesToAddCount = resultsModel.CompetenciesToAddCount;
                data.CompetenciesToUpdateCount = resultsModel.CompetenciesToUpdateCount;
                setBulkUploadData(data);
                return View("Developer/Import/ImportCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                FileHelper.DeleteFile(webHostEnvironment, data.CompetenciesFileName);
                return View("ImportFailed");
            }
        }
        [Route("/Framework/{frameworkId}/{tabname}/Import/AssessmentQuestions")]
        public IActionResult AddAssessmentQuestions()
        {
            var data = GetBulkUploadData();

            return View();
        }
        private void setupBulkUploadData(int frameworkId, int adminUserID, string competenciessFileName, string tabName, bool isNotBlank)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("BulkCompetencyDataCWF"), TempData);
            var today = clockUtility.UtcToday;
            var bulkUploadData = new BulkCompetenciesData(frameworkId, adminUserID, competenciessFileName, tabName, isNotBlank);
            setBulkUploadData(bulkUploadData);
        }
        private void setBulkUploadData(BulkCompetenciesData bulkUploadData)
        {
            multiPageFormService.SetMultiPageFormData(
                 bulkUploadData,
                 MultiPageFormDataFeature.AddCustomWebForm("BulkCompetencyDataCWF"),
                 TempData
             );
        }

        private BulkCompetenciesData GetBulkUploadData()
        {
            var data = multiPageFormService.GetMultiPageFormData<BulkCompetenciesData>(
               MultiPageFormDataFeature.AddCustomWebForm("BulkCompetencyDataCWF"),
               TempData
           ).GetAwaiter().GetResult();
            return data;
        }
    }
}
