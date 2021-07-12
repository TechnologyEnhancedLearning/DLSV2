﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/BulkUpload")]
    public class BulkUploadController : Controller
    {
        private readonly IDelegateDownloadFileService delegateDownloadFileService;

        public BulkUploadController(IDelegateDownloadFileService delegateDownloadFileService)
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
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
                $"ITSP Delegate for Bulk Update {DateTime.Today:yyyy-MM-dd}.xlsx"
            );
        }
    }
}
