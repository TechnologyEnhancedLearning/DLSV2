namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.VerifyEmail;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
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
            var userEntity = userService.GetUserById(userId)!;
            var unverifiedPrimaryEmail =
                userEntity.UserAccount.EmailVerified == null ? userEntity.UserAccount.PrimaryEmail : null;
            var unverifiedCentreSpecificEmails = userService.GetUnverifiedCentreEmailsForUser(userId).Where(
                pair => userEntity.AdminAccounts.Any(
                            account => account.CentreName.Equals(pair.centreName) && account.Active
                        ) ||
                        userEntity.DelegateAccounts.Any(
                            account => account.CentreName.Equals(pair.centreName) && account.Active
                        )
            );

            var model = new VerifyEmailViewModel(
                emailVerificationReason,
                unverifiedPrimaryEmail,
                unverifiedCentreSpecificEmails.ToList()
            );

            return View(model);
        }
    }
}
