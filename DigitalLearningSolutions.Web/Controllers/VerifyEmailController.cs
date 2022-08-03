namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Transactions;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.BasicUser)]
    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class VerifyEmailController : Controller
    {
        private readonly IUserService userService;
        private readonly IClockUtility clockUtility;

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

            var emailVerificationDetails = userService.GetEmailVerificationDetails(email, code);

            if (emailVerificationDetails == null)
            {
                return NotFound();
            }

            if (emailVerificationDetails.IsEmailVerified)
            {
                return View();
            }

            if (emailVerificationDetails.HasVerificationExpired(clockUtility))
            {
                return View("VerificationLinkExpired");
            }

            using var transaction = new TransactionScope();

            userService.SetEmailVerified(
                emailVerificationDetails.UserId,
                emailVerificationDetails.Email,
                clockUtility.UtcNow
            );

            transaction.Complete();

            return View();
        }
    }
}
