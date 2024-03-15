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
    using Microsoft.AspNetCore.Hosting;
    using System.IO;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;

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
                data.ToProcessCount = resultsModel.ToProcessCount;
                data.ToRegisterCount = resultsModel.ToRegisterCount;
                data.ToUpdateCount = resultsModel.ToUpdateOrSkipCount;
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
            var model = new WelcomeEmailViewModel() { Day = data.Day, Month = data.Month, Year = data.Year, DelegatesToRegister = data.ToRegisterCount };
            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitWelcomeEmail(WelcomeEmailViewModel model)
        {
            var data = GetBulkUploadData();
            model.DelegatesToRegister = data.ToRegisterCount;
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
            var model = new AddToGroupViewModel(data.AddToGroupOption, existingGroups: groupSelect, data.ExistingGroupId, data.NewGroupName, data.NewGroupDescription, registeringDelegates: data.ToRegisterCount > 0, updatingDelegates: data.ToUpdateCount > 0);
            return View(model);
        }

        [HttpPost]
        [Route("SubmitAddToGroup")]
        public IActionResult SubmitAddToGroup(AddToGroupViewModel model)
        {
            if (model.AddToGroupOption == 3)
            {
                return RedirectToAction("UploadSummary");
            }
            var data = GetBulkUploadData();
            if (!ModelState.IsValid)
            {
                var centreId = User.GetCentreIdKnownNotNull();
                var groupSelect = groupsService.GetUnlinkedGroupsSelectListForCentre(centreId, data.ExistingGroupId);
                model.ExistingGroups = groupSelect;
                model.RegisteringDelegates = data.ToRegisterCount > 0;
                model.UpdatingDelegates = data.ToUpdateCount > 0;
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

            if (data.ToRegisterCount > 0 && data.ToUpdateCount > 0)
            {
                setBulkUploadData(data);
                return RedirectToAction("AddWhoToGroup");
            }
            else
            {
                if (data.ToUpdateCount > 0)
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
            return View();
        }

        [Route("UploadSummary")]
        public IActionResult UploadSummary()
        {
            return View();
        }

        [Route("CancelUpload")]
        public IActionResult CancelUpload()
        {
            var data = GetBulkUploadData();
            FileHelper.DeleteFile(webHostEnvironment, data.DelegatesFileName);
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
    }
}
