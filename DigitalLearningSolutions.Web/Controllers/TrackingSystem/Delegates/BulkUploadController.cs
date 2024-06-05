namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using Microsoft.Extensions.Configuration;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using ConfigurationExtensions = Data.Extensions.ConfigurationExtensions;
    using ClosedXML.Excel;
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Hosting;
    using System.IO;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload;
    using System;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using System.Linq;
    using System.Transactions;

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
        private readonly IGroupsService groupsService;
        public BulkUploadController(
            IDelegateDownloadFileService delegateDownloadFileService
            , IDelegateUploadFileService delegateUploadFileService
            , IClockUtility clockUtility
            , IConfiguration configuration
            , IMultiPageFormService multiPageFormService
            , IWebHostEnvironment webHostEnvironment
            , IGroupsService groupsService

        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
            this.delegateUploadFileService = delegateUploadFileService;
            this.clockUtility = clockUtility;
            this.configuration = configuration;
            this.multiPageFormService = multiPageFormService;
            this.webHostEnvironment = webHostEnvironment;
            this.groupsService = groupsService;
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
                return View("Index", model);
            }
            try
            {
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
            catch (DocumentFormat.OpenXml.Packaging.OpenXmlPackageException)
            {
                ModelState.AddModelError("DelegatesFile", "The Excel file has at least one cell containing an invalid hyperlink or email address.");
                return View("Index", model);
            }
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
                data.ToProcessCount = resultsModel.ToProcessCount;
                data.ToRegisterActiveCount = resultsModel.ToRegisterActiveCount;
                data.ToRegisterInactiveCount = resultsModel.ToRegisterInactiveCount;
                data.ToUpdateActiveCount = resultsModel.ToUpdateOrSkipActiveCount;
                data.ToUpdateInactiveCount = resultsModel.ToUpdateOrSkipInactiveCount;
                setBulkUploadData(data);
                return View("UploadCompleted", resultsModel);
            }
            catch (InvalidHeadersException)
            {
                FileHelper.DeleteFile(webHostEnvironment, data.DelegatesFileName);
                return View("UploadFailed");
            }
        }

        [Route("WelcomeEmail")]
        public IActionResult WelcomeEmail()
        {
            var data = GetBulkUploadData();
            var model = new WelcomeEmailViewModel() { Day = data.Day, Month = data.Month, Year = data.Year, DelegatesToRegister = data.ToRegisterActiveCount + data.ToRegisterInactiveCount };
            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitWelcomeEmail(WelcomeEmailViewModel model)
        {
            var data = GetBulkUploadData();
            model.DelegatesToRegister = data.ToRegisterActiveCount + data.ToRegisterInactiveCount;
            if (!ModelState.IsValid)
            {
                return View("WelcomeEmail", model);
            }
            data.Day = model.Day;
            data.Month = model.Month;
            data.Year = model.Year;
            setBulkUploadData(data);
            return RedirectToAction("AddToGroup");
        }

        [Route("AddToGroup")]
        public IActionResult AddToGroup()
        {
            var data = GetBulkUploadData();
            var centreId = User.GetCentreIdKnownNotNull();
            var groupSelect = groupsService.GetUnlinkedGroupsSelectListForCentre(centreId, data.ExistingGroupId);
            var model = new AddToGroupViewModel(data.AddToGroupOption, existingGroups: groupSelect, data.ExistingGroupId, data.NewGroupName, data.NewGroupDescription, registeringActiveDelegates: data.ToRegisterActiveCount, updatingActiveDelegates: data.ToUpdateActiveCount, registeringInactiveDelegates: data.ToRegisterInactiveCount, updatingInactiveDelegates: data.ToUpdateInactiveCount);
            return View(model);
        }

        [HttpPost]
        [Route("SubmitAddToGroup")]
        public IActionResult SubmitAddToGroup(AddToGroupViewModel model)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var data = GetBulkUploadData();
            if (model.AddToGroupOption == 2)
            {
                if (!string.IsNullOrEmpty(model.NewGroupName))
                {
                    if (groupsService.IsDelegateGroupExist(model.NewGroupName.Trim(), centreId))
                    {
                        ModelState.AddModelError(nameof(model.NewGroupName), "A group with the same name already exists (if it does not appear in the list of groups, it may be linked to a centre registration field)");
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                var groupSelect = groupsService.GetUnlinkedGroupsSelectListForCentre(centreId, data.ExistingGroupId);
                model.ExistingGroups = groupSelect;
                model.RegisteringActiveDelegates = data.ToRegisterActiveCount;
                model.UpdatingActiveDelegates = data.ToUpdateActiveCount;
                return View("AddToGroup", model);
            }
            data.AddToGroupOption = model.AddToGroupOption;
            if (model.AddToGroupOption == 1)
            {
                data.ExistingGroupId = model.ExistingGroupId;
            }
            if (model.AddToGroupOption == 2)
            {
                data.NewGroupName = model.NewGroupName;
                data.NewGroupDescription = model.NewGroupDescription;
            }

            if (data.ToUpdateActiveCount > 0)
            {
                setBulkUploadData(data);
                return RedirectToAction("AddWhoToGroup");
            }
            else
            {
                if (data.ToUpdateActiveCount > 0)
                {
                    data.IncludeUpdatedDelegates = true;
                }
                setBulkUploadData(data);
                return RedirectToAction("UploadSummary");
            }
        }

        [Route("AddWhoToGroup")]
        public IActionResult AddWhoToGroup()
        {
            var data = GetBulkUploadData();
            var centreId = User.GetCentreIdKnownNotNull();
            var groupName = data.NewGroupName;
            if (groupName == null && data.ExistingGroupId != null)
            {
                var group = groupsService.GetGroupAtCentreById((int)data.ExistingGroupId, centreId);
                if (group != null)
                {
                    groupName = group.GroupLabel;
                }
            }
            if (groupName == null)
            {
                return RedirectToAction("UploadSummary");
            }
            var model = new AddWhoToGroupViewModel(groupName!, data.IncludeUpdatedDelegates, data.IncludeSkippedDelegates, data.ToUpdateActiveCount, data.ToRegisterActiveCount);
            return View(model);
        }

        [HttpPost]
        [Route("AddWhoToGroup")]
        public IActionResult SubmitAddWhoToGroup(AddWhoToGroupViewModel model)
        {
            var data = GetBulkUploadData();
            data.IncludeUpdatedDelegates = model.AddWhoToGroupOption>=2;
            data.IncludeSkippedDelegates = model.AddWhoToGroupOption == 3;
            setBulkUploadData(data);
            return RedirectToAction("UploadSummary");
        }

        [Route("UploadSummary")]
        public IActionResult UploadSummary()
        {
            var data = GetBulkUploadData();
            var centreId = User.GetCentreIdKnownNotNull();
            string? groupName;
            if (data.AddToGroupOption == 1 && data.ExistingGroupId != null)
            {
                groupName = groupsService.GetGroupName((int)data.ExistingGroupId, centreId);
            }
            else
            {
                groupName = data.NewGroupName;
            }
            var model = new UploadSummaryViewModel(
                data.ToProcessCount,
                data.ToRegisterActiveCount + data.ToRegisterInactiveCount,
                data.ToUpdateActiveCount + data.ToRegisterInactiveCount,
                data.MaxRowsToProcess,
                (int)data.AddToGroupOption,
                groupName,
                data.Day,
                data.Month,
                data.Year,
                data.IncludeUpdatedDelegates
                );
            return View(model);
        }

        [Route("StartProcessing")]
        [HttpPost]
        public IActionResult StartProcessing()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var data = GetBulkUploadData();
            var adminId = User.GetAdminIdKnownNotNull();
            if (data.AddToGroupOption == 2 && data.NewGroupName != null)
            {
                data.ExistingGroupId = groupsService.AddDelegateGroup(centreId, data.NewGroupName, data.NewGroupDescription, adminId);
                setBulkUploadData(data);
            }
            return RedirectToAction("ProcessNextStep");
        }
        private BulkUploadResult ProcessRowsAndReturnResults()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var data = GetBulkUploadData();
            var adminId = User.GetAdminIdKnownNotNull();
            var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "Uploads\\");
            var filePath = Path.Combine(uploadDir, data.DelegatesFileName);
            var workbook = new XLWorkbook(filePath);
            var welcomeEmailDate = new DateTime(data.Year!.Value, data.Month!.Value, data.Day!.Value);
            var table = delegateUploadFileService.OpenDelegatesTable(workbook);
            var results = delegateUploadFileService.ProcessDelegatesFile(
                  table,
                  centreId,
                  welcomeEmailDate,
                  data.LastRowProcessed,
                  data.MaxRowsToProcess,
                  data.IncludeUpdatedDelegates,
                  data.IncludeSkippedDelegates,
                  adminId,
                  data.ExistingGroupId
                  );
            return results;
        }
        [Route("ProcessNextStep")]
        public IActionResult ProcessNextStep()
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var centreId = User.GetCentreIdKnownNotNull();
                var data = GetBulkUploadData();
                var adminId = User.GetAdminIdKnownNotNull();
                int processSteps = (int)Math.Ceiling((double)data.ToProcessCount / data.MaxRowsToProcess);
                int step = data.LastRowProcessed / data.MaxRowsToProcess + 1;
                var results = ProcessRowsAndReturnResults();
                data.SubtotalDelegatesRegistered += results.RegisteredActiveCount + results.RegisteredInactiveCount;
                data.SubtotalDelegatesUpdated += results.UpdatedActiveCount + results.UpdatedInactiveCount;
                data.SubTotalSkipped += results.SkippedCount;
                data.Errors = data.Errors.Concat(results.Errors.Select(x => (x.RowNumber, MapReasonToErrorMessage(x.Reason))));
                if (step < processSteps)
                {
                    data.LastRowProcessed = data.LastRowProcessed + data.MaxRowsToProcess;
                }
                else
                {
                    data.LastRowProcessed = data.ToProcessCount;
                }
                setBulkUploadData(data);
                if (data.LastRowProcessed >= data.ToProcessCount)
                {
                    return RedirectToAction("BulkUploadResults");
                }
                return RedirectToAction("ProcessBulkDelegates", new { step, totalSteps = processSteps });
            }
        }

        [Route("ProcessBulkDelegates/step/{step}/{totalSteps}/")]
        public IActionResult ProcessBulkDelegates(int step, int totalSteps)
        {
            var data = GetBulkUploadData();
            var model = new ProcessBulkDelegatesViewModel(
                stepNumber: step,
                totalSteps: totalSteps,
                rowsProcessed: data.LastRowProcessed - 1, //Adjusted because last row processed includes header row
                totalRows: data.ToProcessCount,
                maxRowsPerStep: data.MaxRowsToProcess,
                delegatesRegistered: data.SubtotalDelegatesRegistered,
                delegatesUpdated: data.SubtotalDelegatesUpdated,
                rowsSkipped: data.SubTotalSkipped,
                errorCount: data.Errors.Count()
                );
            return View(model);
        }

        [Route("BulkUploadResults")]
        public IActionResult BulkUploadResults()
        {
            var data = GetBulkUploadData();
            FileHelper.DeleteFile(webHostEnvironment, data.DelegatesFileName);
            int processSteps = (int)Math.Ceiling((double)data.ToProcessCount / data.MaxRowsToProcess);
            var model = new BulkUploadResultsViewModel(
                processedCount: data.ToProcessCount,
                registeredCount: data.SubtotalDelegatesRegistered,
                updatedCount: data.SubtotalDelegatesUpdated,
                skippedCount: data.SubTotalSkipped,
                errors: data.Errors,
                day: (int)data.Day,
                month: (int)data.Month,
                year: (int)data.Year,
                totalSteps: processSteps
                );
            TempData.Clear();
            return View(model);
        }

        [Route("CancelUpload")]
        public IActionResult CancelUpload()
        {
            var data = GetBulkUploadData();
            FileHelper.DeleteFile(webHostEnvironment, data.DelegatesFileName);
            TempData.Clear();
            return RedirectToAction("Index");
        }



        private void setupBulkUploadData(int centreId, int adminUserID, string delegatesFileName)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("BulkUploadDataCWF"), TempData);
            int maxBulkUploadRows = GetMaxBulkUploadRowsLimit();
            var today = clockUtility.UtcToday;
            var bulkUploadData = new BulkUploadData(centreId, adminUserID, delegatesFileName, maxBulkUploadRows, today);
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

        private static string MapReasonToErrorMessage(BulkUploadResult.ErrorReason reason)
        {
            return reason switch
            {
                BulkUploadResult.ErrorReason.InvalidJobGroupId =>
                    "JobGroupID was not valid, please ensure a valid job group is selected from the provided list",
                BulkUploadResult.ErrorReason.MissingLastName =>
                    "LastName was not provided. LastName is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.MissingFirstName =>
                    "FirstName was not provided. FirstName is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.MissingEmail =>
                    "EmailAddress was not provided. EmailAddress is a required field and cannot be left blank",
                BulkUploadResult.ErrorReason.InvalidActive =>
                    "Active field could not be read. The Active field should contain 'TRUE' or 'FALSE'",
                BulkUploadResult.ErrorReason.NoRecordForDelegateId =>
                    "No existing delegate record was found with the DelegateID provided",
                BulkUploadResult.ErrorReason.UnexpectedErrorForCreate =>
                    "Unexpected error when creating delegate",
                BulkUploadResult.ErrorReason.UnexpectedErrorForUpdate =>
                    "Unexpected error when updating delegate details",
                BulkUploadResult.ErrorReason.ParameterError => "Parameter error when updating delegate details",
                BulkUploadResult.ErrorReason.EmailAddressInUse =>
                    "The EmailAddress is already in use by another delegate",
                BulkUploadResult.ErrorReason.TooLongFirstName => "FirstName must be 250 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongLastName => "LastName must be 250 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongEmail => "EmailAddress must be 250 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer1 => "Answer1 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer2 => "Answer2 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer3 => "Answer3 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer4 => "Answer4 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer5 => "Answer5 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.TooLongAnswer6 => "Answer6 must be 100 characters or fewer",
                BulkUploadResult.ErrorReason.BadFormatEmail =>
                    "EmailAddress must be in the correct format, like name@example.com",
                BulkUploadResult.ErrorReason.WhitespaceInEmail =>
                    "EmailAddress must not contain any whitespace characters",
                BulkUploadResult.ErrorReason.HasPrnButMissingPrnValue =>
                    "HasPRN was set to true, but PRN was not provided. When HasPRN is set to true, PRN is a required field",
                BulkUploadResult.ErrorReason.PrnButHasPrnIsFalse =>
                    "HasPRN was set to false, but PRN was provided. When HasPRN is set to false, PRN is required to be empty",
                BulkUploadResult.ErrorReason.InvalidPrnLength => "PRN must be between 5 and 20 characters",
                BulkUploadResult.ErrorReason.InvalidPrnCharacters =>
                    "Invalid PRN format - Only alphanumeric characters (a-z, A-Z and 0-9) and hyphens (-) allowed",
                BulkUploadResult.ErrorReason.InvalidHasPrnValue => "HasPRN field could not be read. The HasPRN field should contain 'TRUE' or 'FALSE' or be left blank",
                _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null),
            };
        }
    }
}
