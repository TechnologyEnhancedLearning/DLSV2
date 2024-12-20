using ClosedXML.Excel;
using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Data.Migrations;
using DigitalLearningSolutions.Data.Utilities;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models;
using DigitalLearningSolutions.Web.Services;
using DigitalLearningSolutions.Web.ViewModels.Frameworks;
using GDS.MultiPageFormData.Enums;
using Microsoft.AspNetCore.Hosting;
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
        public IActionResult StartImport(ImportCompetenciesViewModel model, string tabname)
        {
            if (!ModelState.IsValid)
                return View("Developer/ImportCompetencies", model);
            try
            {
                var adminUserID = User.GetAdminIdKnownNotNull();
                var workbook = new XLWorkbook(model.ImportFile.OpenReadStream());
                if (!workbook.Worksheets.Contains(ImportCompetenciesFromFileService.CompetenciesSheetName))
                {
                    ModelState.AddModelError("InvalidWorksheet", CommonValidationErrorMessages.InvalidCompetenciesUploadExcelFile);
                    return View("Developer/ImportCompetencies", model);
                }
                var competenciesFileName = FileHelper.UploadFile(webHostEnvironment, model.ImportFile);
                setupBulkUploadData(model.FrameworkId, adminUserID, competenciesFileName, tabname);

                return RedirectToAction("ImportCompleted", "Frameworks", new { frameworkId = model.FrameworkId, tabname });
            }
            catch (DocumentFormat.OpenXml.Packaging.OpenXmlPackageException)
            {
                ModelState.AddModelError("DelegatesFile", "The Excel file has at least one cell containing an invalid hyperlink or email address.");
                return View("Index", model);
            }
            catch (InvalidHeadersException)
            {
                return View("Developer/ImportFailed");
            }
        }
        [Route("/Framework/{frameworkId}/{tabname}/ImportCompleted")]
        public IActionResult ImportCompleted()
        {
            return View("Developer/ImportCompleted");
        }

        private void setupBulkUploadData(int frameworkId, int adminUserID, string competenciessFileName, string tabName)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("BulkCompetencyDataCWF"), TempData);
            var today = clockUtility.UtcToday;
            var bulkUploadData = new BulkCompetenciesData(frameworkId, adminUserID, competenciessFileName, tabName);
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
