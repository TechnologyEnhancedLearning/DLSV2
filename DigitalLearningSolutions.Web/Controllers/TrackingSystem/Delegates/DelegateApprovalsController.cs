namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserCentreAdminOnly)]
    [Route("/TrackingSystem/Delegates/Approve")]
    public class DelegateApprovalsController : Controller
    {
        private readonly IDelegateApprovalsService delegateApprovalsService;

        public DelegateApprovalsController(IDelegateApprovalsService delegateApprovalsService)
        {
            this.delegateApprovalsService = delegateApprovalsService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            var delegates = delegateApprovalsService
                .GetUnapprovedDelegatesWithCustomPromptAnswersForCentre(centreId)
                .Select(d => new UnapprovedDelegate(d.delegateUser, d.prompts));

            var model = new DelegateApprovalsViewModel(centreId, delegates);
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
        [Route("/TrackingSystem/Delegates/Approve/ApproveForCentre")]
        public IActionResult ApproveDelegatesForCentre()
        {
            var centreId = User.GetCentreId();
            delegateApprovalsService.ApproveAllUnapprovedDelegatesForCentre(centreId);
            return RedirectToAction("Index", "DelegateApprovals");
        }
    }
}
