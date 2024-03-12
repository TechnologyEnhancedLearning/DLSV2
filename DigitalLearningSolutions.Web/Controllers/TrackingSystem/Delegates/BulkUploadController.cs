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
            TempData.Clear();
            var model = new UploadDelegatesViewModel();
            model.MaxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            int MaxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            TempData["MaxBulkUploadRows"] = MaxBulkUploadRows;
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
            int MaxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            var centreId = User.GetCentreIdKnownNotNull();
            model.MaxBulkUploadRows = MaxBulkUploadRows;
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
            try
            {
                var table = delegateUploadFileService.OpenDelegatesTable(model.DelegatesFile!);
                var results = delegateUploadFileService.PreProcessDelegatesFile(
                    table
                );
                var resultsModel = new BulkUploadPreProcessViewModel(results);
                return View("UploadCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                return View("UploadFailed");
            }
        }
    }
}
