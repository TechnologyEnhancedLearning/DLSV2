namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/ConfirmSupervisor/{supervisorDelegateId}")]
        public IActionResult ConfirmSupervisor(int supervisorDelegateId)
        {
            var candidateId = GetCandidateId();
            var supervisorDelegate = supervisorService.GetSupervisorDelegateDetailsById(supervisorDelegateId);
            if(supervisorDelegate.CandidateID != candidateId | supervisorDelegate.Confirmed != null | supervisorDelegate.Removed != null )
            {
                logger.LogWarning($"Attempt to display confirm supervisor screen for where candidate id ({candidateId}) did not match supervise delegate candidate id ({supervisorDelegate.CandidateID}). SuperviseDelegateID: {supervisorDelegateId}");
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 403 });
            }
            return View("ConfirmSupervisor", supervisorDelegate);
        }
        public IActionResult AcceptSupervisorDelegateInvite(int supervisorDelegateId)
        {
            if(supervisorService.ConfirmSupervisorDelegateById(supervisorDelegateId, GetCandidateId(), 0))
            {
                frameworkNotificationService.SendSupervisorDelegateAcceptance(supervisorDelegateId);
            }
            return RedirectToAction("Current");
        }
        public IActionResult RejecttSupervisorDelegateInvite(int supervisorDelegateId)
        {
            if(supervisorService.RemoveSupervisorDelegateById(supervisorDelegateId, GetCandidateId(), 0))
            {
                frameworkNotificationService.SendSupervisorDelegateRejected(supervisorDelegateId);
            }
            return RedirectToAction("Current");
        }
    }
}
