namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.VerifyEmail;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.BasicUser)]
    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class VerifyYourEmailController : Controller
    {
        private readonly IUserService userService;

        public VerifyYourEmailController(IUserService userService)
        {
            this.userService = userService;
        }

        [Route("/VerifyYourEmail/{emailVerificationReason}")]
        public IActionResult Index(EmailVerificationReason? emailVerificationReason)
        {
            if (emailVerificationReason == null)
            {
                return NotFound();
            }

            var userId = User.GetUserIdKnownNotNull();
            var (unverifiedPrimaryEmail, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userId);
            var model = new VerifyEmailViewModel(
                emailVerificationReason,
                unverifiedPrimaryEmail,
                unverifiedCentreEmails.ToList()
            );

            return View(model);
        }
    }
}
