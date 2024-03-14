namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using Microsoft.Extensions.Configuration;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using ConfigurationExtensions = Data.Extensions.ConfigurationExtensions;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Hosting;
    using DocumentFormat.OpenXml.EMMA;
    using System.IO;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/BulkUpload")]
    public class BulkUploadController : Controller
    {
        private readonly IDelegateDownloadFileService delegateDownloadFileService;
        private readonly IDelegateUploadFileService delegateUploadFileService;
        private readonly IClockUtility clockUtility;
        private readonly IConfiguration configuration;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IWebHostEnvironment webHostEnvironment;
        public BulkUploadController(
            IDelegateDownloadFileService delegateDownloadFileService
            , IDelegateUploadFileService delegateUploadFileService
            , IClockUtility clockUtility
            , IConfiguration configuration
            , IMultiPageFormService multiPageFormService
            , IWebHostEnvironment webHostEnvironment
        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
            this.delegateUploadFileService = delegateUploadFileService;
            this.clockUtility = clockUtility;
            this.configuration = configuration;
            this.multiPageFormService = multiPageFormService;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            TempData.Clear();
            var model = new UploadDelegatesViewModel();
            return View(model);
        }
        private int GetMaxBulkUploadRowsLimit()
        {
            return ConfigurationExtensions.GetMaxBulkUploadRowsLimit(configuration);

        }

        [Route("DownloadDelegates")]
        public IActionResult DownloadDelegates(int DownloadOption)
        {
            string fileName = DownloadOption == 2 ? $"DLS Delegates for Bulk Update {clockUtility.UtcToday:yyyy-MM-dd}.xlsx" : "DLS Delegates for Bulk Registration.xlsx";
            var content = delegateDownloadFileService.GetDelegatesAndJobGroupDownloadFileForCentre(
                    User.GetCentreIdKnownNotNull(), DownloadOption == 2 ? false : true
                );
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
        [Route("StartUpload")]
        [HttpPost]
        public IActionResult StartUpload(UploadDelegatesViewModel model)
        {

            var centreId = User.GetCentreIdKnownNotNull();
            var adminUserID = User.GetAdminIdKnownNotNull();
            if (!ModelState.IsValid)
            {
                return View("StartUpload", model);
            }
            var workbook = new XLWorkbook(model.DelegatesFile.OpenReadStream());
            if (!workbook.Worksheets.Contains(DelegateDownloadFileService.DelegatesSheetName))
            {
                ModelState.AddModelError("MaxBulkUploadRows", CommonValidationErrorMessages.InvalidBulkUploadExcelFile);
                return View("StartUpload", model);
            }
            var delegateFileName = FileHelper.UploadFile(webHostEnvironment, model.DelegatesFile);
            setupBulkUploadData(centreId, adminUserID, delegateFileName);

            return RedirectToAction("UploadComplete");
        }

        [Route("UploadComplete")]
        public IActionResult UploadComplete()
        {
            var data = GetBulkUploadData();
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            var filePath = Path.Combine(uploadDir, data.DelegatesFileName);
            var workbook = new XLWorkbook(filePath);
            var table = delegateUploadFileService.OpenDelegatesTable(workbook);
            try
            {
                var results = delegateUploadFileService.PreProcessDelegatesFile(
                  table
              );
                var resultsModel = new BulkUploadPreProcessViewModel(results);
                return View("UploadCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                FileHelper.DeleteFile(webHostEnvironment, data.DelegatesFileName);
                return View("UploadFailed");
            }            
        }

        [Route("SendUsersWelcomeEmail")]
        public IActionResult SendUsersWelcomeEmail()
        {
            return View();
        }

        [Route("AddUsersToGroup")]
        public IActionResult AddUsersToGroup()
        {
            return View();
        }

        private void setupBulkUploadData(int centreId, int adminUserID, string delegatesFileName)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("BulkUploadDataCWF"), TempData);
            int maxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            var bulkUploadData = new BulkUploadData(centreId, adminUserID, delegatesFileName, maxBulkUploadRows);
            setBulkUploadData(bulkUploadData);
        }
        private void setBulkUploadData(BulkUploadData bulkUploadData)
        {
           multiPageFormService.SetMultiPageFormData(
                bulkUploadData,
                MultiPageFormDataFeature.AddCustomWebForm("BulkUploadDataCWF"),
                TempData
            );
        }

        private BulkUploadData GetBulkUploadData()
        {
            var data = multiPageFormService.GetMultiPageFormData<BulkUploadData>(
               MultiPageFormDataFeature.AddCustomWebForm("BulkUploadDataCWF"),
               TempData
           ).GetAwaiter().GetResult();
            return data;
        }
    }
}
