namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.VerifyEmail;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [Authorize(Policy = CustomPolicies.BasicUser)]
    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class VerifyYourEmailController : Controller
    {
        private readonly IConfiguration config;
        private readonly IEmailVerificationService emailVerificationService;
        private readonly IUserService userService;

        public VerifyYourEmailController(
            IUserService userService,
            IEmailVerificationService emailVerificationService,
            IConfiguration config
        )
        {
            this.userService = userService;
            this.emailVerificationService = emailVerificationService;
            this.config = config;
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
            var unverifiedEmails = new List<string>();
            if (unverifiedPrimaryEmail != null)
            {
                unverifiedEmails.Add(unverifiedPrimaryEmail);
            }

            if (unverifiedCentreEmails.Any())
            {
                unverifiedEmails.AddRange(unverifiedCentreEmails.Select(uce => uce.centreEmail));
            }
            var userEntity = userService.GetUserById(userId);
            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userEntity!.UserAccount,
                unverifiedEmails,
                config.GetAppRootPath()
            );
            var model = new VerifyYourEmailViewModel(
                emailVerificationReason,
                unverifiedPrimaryEmail,
                unverifiedCentreEmails.ToList()
            );

            return View(model);
        }

        public IActionResult ResendVerificationEmails()
        {
            var userId = User.GetUserIdKnownNotNull();
            var userEntity = userService.GetUserById(userId);
            var (primaryEmailIfUnverified, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userId);

            var unverifiedEmails = new List<string>();
            if (primaryEmailIfUnverified != null)
            {
                unverifiedEmails.Add(primaryEmailIfUnverified);
            }

            if (unverifiedCentreEmails.Any())
            {
                unverifiedEmails.AddRange(unverifiedCentreEmails.Select(uce => uce.centreEmail));
            }

            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userEntity!.UserAccount,
                unverifiedEmails,
                config.GetAppRootPath()
            );

            return RedirectToAction(
                "Index",
                new { emailVerificationReason = EmailVerificationReason.EmailNotVerified }
            );
        }
    }
}
