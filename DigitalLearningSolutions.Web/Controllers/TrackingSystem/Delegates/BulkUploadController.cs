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
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Web.Models;

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
        public BulkUploadController(
            IDelegateDownloadFileService delegateDownloadFileService
            , IDelegateUploadFileService delegateUploadFileService
            , IClockUtility clockUtility
            , IConfiguration configuration
            , IMultiPageFormService multiPageFormService
        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
            this.delegateUploadFileService = delegateUploadFileService;
            this.clockUtility = clockUtility;
            this.configuration = configuration;
            this.multiPageFormService = multiPageFormService;
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
        private void setupBulkUploadData(int centreId, int adminUserID, IXLTable delegateTable)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("RequestSupportTicketCWF"), TempData);
            int maxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            var bulkUploadData = new BulkUploadData(centreId, adminUserID, delegateTable, maxBulkUploadRows);
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
            try
            {
                var table = delegateUploadFileService.OpenDelegatesTable(model.DelegatesFile!);
                setupBulkUploadData(centreId, adminUserID, table);
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
