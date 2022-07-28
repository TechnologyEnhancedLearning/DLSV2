namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.VerifyEmail;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.BasicUser)]
    public class VerifyEmailController : Controller
    {
        private readonly IUserService userService;

        public VerifyEmailController(IUserService userService)
        {
            this.userService = userService;
        }

        [Route("/VerifyEmail/{emailVerificationReason}")]
        public IActionResult Index(string? emailVerificationReason)
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
