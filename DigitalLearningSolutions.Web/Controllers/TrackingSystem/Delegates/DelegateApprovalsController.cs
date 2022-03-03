﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
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
        private readonly IUserDataService userDataService;

        public DelegateApprovalsController(
            IDelegateApprovalsService delegateApprovalsService,
            IUserDataService userDataService
        )
        {
            this.delegateApprovalsService = delegateApprovalsService;
            this.userDataService = userDataService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var delegates = delegateApprovalsService
                .GetUnapprovedDelegatesWithRegistrationPromptAnswersForCentre(centreId)
                .Select(d => new UnapprovedDelegate(d.delegateUser, d.prompts));

            var model = new DelegateApprovalsViewModel(delegates);
            return View(model);
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/Approve")]
        public IActionResult ApproveDelegate(int delegateId)
        {
            var centreId = User.GetCentreId();
            delegateApprovalsService.ApproveDelegate(delegateId, centreId);
            return RedirectToAction("Index", "DelegateApprovals");
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/Approve/All")]
        public IActionResult ApproveDelegatesForCentre()
        {
            var centreId = User.GetCentreId();
            delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(centreId);
            return RedirectToAction("Index", "DelegateApprovals");
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/Reject")]
        public IActionResult DelegateRejectionPage(int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId);
            var model = new RejectDelegateViewModel(delegateUser);
            return View(model);
        }

        [HttpPost]
        [Route("/TrackingSystem/Delegates/ConfirmReject")]
        public IActionResult RejectDelegate(int delegateId)
        {
            var centreId = User.GetCentreId();
            delegateApprovalsService.RejectDelegate(delegateId, centreId);
            return RedirectToAction("Index", "DelegateApprovals");
        }
    }
}
