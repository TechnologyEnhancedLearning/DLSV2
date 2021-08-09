namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.DelegateUpload;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/BulkUpload")]
    public class BulkUploadController : Controller
    {
        private readonly IDelegateDownloadFileService delegateDownloadFileService;
        private readonly IDelegateUploadFileService delegateUploadFileService;

        public BulkUploadController(
            IDelegateDownloadFileService delegateDownloadFileService,
            IDelegateUploadFileService delegateUploadFileService
        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
            this.delegateUploadFileService = delegateUploadFileService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("DownloadDelegates")]
        public IActionResult DownloadDelegates()
        {
            var content = delegateDownloadFileService.GetDelegateDownloadFileForCentre(User.GetCentreId());
            return File(
                content,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DLS Delegates for Bulk Update {DateTime.Today:yyyy-MM-dd}.xlsx"
            );
        }

        [Route("StartUpload")]
        [HttpGet]
        public IActionResult StartUpload()
        {
            TempData.Clear();
            var model = new UploadDelegatesViewModel();
            return View("StartUpload", model);
        }

        [Route("StartUpload")]
        [HttpPost]
        public IActionResult StartUpload(UploadDelegatesViewModel model)
        {
            model.ClearDateIfNotSendEmail();

            if (!ModelState.IsValid)
            {
                return View("StartUpload", model);
            }

            try
            {
                var centreId = User.GetCentreId();
                var table = delegateUploadFileService.OpenDelegatesTable(model.DelegatesFile);
                var results = delegateUploadFileService.ProcessDelegatesFile(
                    table,
                    centreId,
                    model.GetWelcomeEmailDate()
                );
                TempData.Set(results);
                return RedirectToAction("UploadCompleted");
            }
            catch (InvalidHeadersException)
            {
                return RedirectToAction("UploadFailed");
            }
        }

        [Route("UploadFailed")]
        [HttpGet]
        public IActionResult UploadFailed()
        {
            return View("UploadFailed");
        }

        [Route("UploadCompleted")]
        [HttpGet]
        [ServiceFilter(typeof(RedirectEmptySessionData<BulkUploadResult>))]
        public IActionResult UploadCompleted()
        {
            var results = TempData.Get<BulkUploadResult>()!;
            var model = new BulkUploadResultsViewModel(results);
            return View("UploadCompleted", model);
        }
    }
}
