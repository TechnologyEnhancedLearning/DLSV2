﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("/TrackingSystem/Delegates/Approve")]
    public class DelegateApprovalsController : Controller
    {
        private readonly IDelegateApprovalsService delegateApprovalsService;
        private readonly IUserService userService;

        public DelegateApprovalsController(
            IDelegateApprovalsService delegateApprovalsService,
            IUserService userService
        )
        {
            this.delegateApprovalsService = delegateApprovalsService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();

            var delegates = delegateApprovalsService
                .GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre(centreId)
                .Select(d => new UnapprovedDelegate(d.delegateEntity, d.prompts));

            var model = new DelegateApprovalsViewModel(delegates);
            return View(model);
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/Approve")]
        public IActionResult ApproveDelegate(int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            delegateApprovalsService.ApproveDelegate(delegateId, centreId);
            return RedirectToAction("Index", "DelegateApprovals");
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/Approve/All")]
        public IActionResult ApproveDelegatesForCentre()
        {
            var centreId = User.GetCentreIdKnownNotNull();
            delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(centreId);
            return RedirectToAction("Index", "DelegateApprovals");
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/Reject")]
        public IActionResult DelegateRejectionPage(int delegateId)
        {
            var delegateEntity = userService.GetDelegateById(delegateId);
            var model = new RejectDelegateViewModel(delegateEntity);
            return View(model);
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/ConfirmReject")]
        public IActionResult RejectDelegate(int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            delegateApprovalsService.RejectDelegate(delegateId, centreId);
            return RedirectToAction("Index", "DelegateApprovals");
        }
    }
}
