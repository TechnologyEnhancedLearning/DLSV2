﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/Delegates/Approve")]
    public class DelegateApprovalsController : Controller
    {
        public IActionResult Index()
        {
            var model = new DelegateApprovalsViewModel(new List<UnapprovedDelegate>());
            return View("~/Views/TrackingSystem/Delegates/DelegateApprovals/index.cshtml", model);
        }
    }
}
