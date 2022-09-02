namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Transactions;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.VerifyEmail;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class VerifyEmailController : Controller
    {
        private static readonly TimeSpan EmailVerificationHashLifetime = TimeSpan.FromDays(3);

        private readonly IClockUtility clockUtility;
        private readonly IUserService userService;

        public VerifyEmailController(IUserService userService, IClockUtility clockUtility)
        {
            this.userService = userService;
            this.clockUtility = clockUtility;
        }

        public IActionResult Index(string? email, string? code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return NotFound();
            }

            var emailVerificationData =
                userService.GetEmailVerificationDataIfCodeMatchesAnyUnverifiedEmails(email, code);

            if (emailVerificationData == null)
            {
                return NotFound();
            }

            if (emailVerificationData.HashCreationDate + EmailVerificationHashLifetime < clockUtility.UtcNow)
            {
                return View("VerificationLinkExpired");
            }

            using var transaction = new TransactionScope();

            userService.SetEmailVerified(
                emailVerificationData.UserId,
                email,
                clockUtility.UtcNow
            );

            transaction.Complete();

            return View(new EmailVerifiedViewModel(emailVerificationData.CentreIdIfEmailIsForUnapprovedDelegate));
        }
    }
}
