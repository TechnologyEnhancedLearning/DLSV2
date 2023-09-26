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
            var model = new VerifyYourEmailViewModel(
                emailVerificationReason,
                unverifiedPrimaryEmail,
                unverifiedCentreEmails.ToList()
            );

            return View(model);
        }

        [Route("/VerifyYourEmail/ResendVerificationEmails")]
        public IActionResult ResendVerificationEmails()
        {
            var userId = User.GetUserIdKnownNotNull();
            var userEntity = userService.GetUserById(userId);
            int hashID = userEntity.UserAccount.EmailVerificationHashID ?? 0;

            var unverifiedCentreEmailsList = userService.GetUnverifiedCentreEmailListForUser(userId);

            Dictionary<string, string> EmailAndHashes = unverifiedCentreEmailsList
            .ToDictionary(t => t.centreEmail, t => t.EmailVerificationHashID);

            if (hashID > 0)
            {
                string EmailVerificationHash = userService.GetEmailVerificationHashesFromEmailVerificationHashID(hashID);
                EmailAndHashes.Add(userEntity.UserAccount.PrimaryEmail, EmailVerificationHash);
            }

            emailVerificationService.ResendVerificationEmails(
                userEntity!.UserAccount,
                EmailAndHashes,
                config.GetAppRootPath()
            );
            return RedirectToAction(
                "Index",
                new { emailVerificationReason = EmailVerificationReason.EmailNotVerified }
            );
        }
    }
}
