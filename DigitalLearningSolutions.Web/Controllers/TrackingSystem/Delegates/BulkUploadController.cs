namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
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
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;
    using ClosedXML.Excel;

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
        public BulkUploadController(
            IDelegateDownloadFileService delegateDownloadFileService,
            IDelegateUploadFileService delegateUploadFileService,
            IClockUtility clockUtility, IConfiguration configuration
        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
            this.delegateUploadFileService = delegateUploadFileService;
            this.clockUtility = clockUtility;
            this.configuration = configuration;
        }

        public IActionResult Index()
        {
            int MaxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            TempData["MaxBulkUploadRows"] = MaxBulkUploadRows;
            return View();
        }
        private int GetMaxBulkUploadRowsLimit()
        {
            return ConfigurationExtensions.GetMaxBulkUploadRowsLimit(configuration);

        }
        [Route("DownloadDelegates")]
        public IActionResult DownloadDelegates()
        {
            var content =
                delegateDownloadFileService.GetDelegatesAndJobGroupDownloadFileForCentre(
                    User.GetCentreIdKnownNotNull()
                );
            var fileName = $"DLS Delegates for Bulk Update {clockUtility.UtcToday:yyyy-MM-dd}.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }

        [Route("StartUpload")]
        [HttpGet]
        public IActionResult StartUpload()
        {
            TempData.Clear();
            var model = new UploadDelegatesViewModel(clockUtility.UtcToday);
            model.MaxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            return View("StartUpload", model);
        }

        [Route("StartUpload")]
        [HttpPost]
        public IActionResult StartUpload(UploadDelegatesViewModel model)
        {
            int MaxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            model.MaxBulkUploadRows = MaxBulkUploadRows;
            if (model.DelegatesFile != null)
            {
                var workbook = new XLWorkbook(model.DelegatesFile.OpenReadStream());
                if (!workbook.Worksheets.Contains(DelegateDownloadFileService.DelegatesSheetName))
                {
                    ModelState.AddModelError("MaxBulkUploadRows", CommonValidationErrorMessages.InvalidBulkUploadExcelFile);
                    return View("StartUpload", model);
                }
                int ExcelRowsCount = delegateUploadFileService.GetBulkUploadExcelRowCount(model.DelegatesFile);
                if (ExcelRowsCount > MaxBulkUploadRows)
                {
                    ModelState.AddModelError("MaxBulkUploadRows", string.Format(CommonValidationErrorMessages.MaxBulkUploadRowsLimit, MaxBulkUploadRows));
                }
            }
            if (!ModelState.IsValid)
            {
                return View("StartUpload", model);
            }

            try
            {
                var results = delegateUploadFileService.ProcessDelegatesFile(
                    model.DelegatesFile!,
                    User.GetCentreIdKnownNotNull(),
                    model.GetWelcomeEmailDate()
                );
                var resultsModel = new BulkUploadResultsViewModel(results);
                return View("UploadCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                return View("UploadFailed");
            }
        }
    }
}
