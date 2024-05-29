﻿namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register.ClaimAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class ClaimAccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IClaimAccountService claimAccountService;
        private readonly IConfiguration config;
        private readonly IEmailVerificationService emailVerificationService;

        public ClaimAccountController(
            IUserService userService,
            IClaimAccountService claimAccountService,
            IConfiguration config,
            IEmailVerificationService emailVerificationService
        )
        {
            this.userService = userService;
            this.claimAccountService = claimAccountService;
            this.config = config;
            this.emailVerificationService = emailVerificationService;
        }

        [HttpGet]
        public IActionResult Index(string email, string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LinkDlsAccount", new { email, code });
            }

            var model = GetViewModelIfValidParameters(email, code);

            if (model == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult CompleteRegistration(string email, string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LinkDlsAccount", new { email, code });
            }

            var model = GetViewModelIfValidParameters(email, code);

            if (model == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            return View(
                model.WasPasswordSetByAdmin ? "CompleteRegistrationWithoutPassword" : "CompleteRegistration",
                GetClaimAccountCompleteRegistrationViewModel(model)
            );
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRegistration(
            ConfirmPasswordViewModel formData,
            [FromQuery] string email,
            [FromQuery] string code
        )
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LinkDlsAccount", new { email, code });
            }

            var model = GetViewModelIfValidParameters(email, code);

            if (model == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            if (model.WasPasswordSetByAdmin)
            {
                return NotFound();
            }

            return await CompleteRegistrationPost(model!, formData.Password);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteRegistrationWithoutPassword(string email, string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LinkDlsAccount", new { email, code });
            }

            var model = GetViewModelIfValidParameters(email, code);

            if (model == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            if (!model.WasPasswordSetByAdmin)
            {
                return NotFound();
            }

            return await CompleteRegistrationPost(model!);
        }

        private async Task<IActionResult> CompleteRegistrationPost(
            ClaimAccountViewModel model,
            string? password = null
        )
        {
            if (userService.PrimaryEmailIsInUse(model.Email))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(GetClaimAccountCompleteRegistrationViewModel(model));
            }

            await claimAccountService.ConvertTemporaryUserToConfirmedUser(
                model.UserId,
                model.CentreId,
                model.Email,
                password
            );

            TempData.Set(
                new ClaimAccountConfirmationViewModel
                {
                    Email = model.Email,
                    CentreName = model.CentreName,
                    CandidateNumber = model.CandidateNumber,
                    WasPasswordSetByAdmin = password == null,
                }
            );

            var userEntity = userService.GetUserById(model.UserId);
            IClockUtility clockUtility = new ClockUtility();
            userService.SetPrimaryEmailVerified(userEntity!.UserAccount.Id, model.Email, clockUtility.UtcNow);

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        [ServiceFilter(typeof(RedirectEmptySessionData<ClaimAccountConfirmationViewModel>))]
        public IActionResult Confirmation()
        {
            var model = TempData.Peek<ClaimAccountConfirmationViewModel>()!;

            TempData.Clear();

            return View(model);
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [HttpGet]
        public IActionResult LinkDlsAccount(string email, string code)
        {
            var loggedInUserId = User.GetUserIdKnownNotNull();
            var model = GetViewModelIfValidParameters(email, code, loggedInUserId);
            var actionResult = ValidateClaimAccountViewModelForLinkingAccounts(loggedInUserId, model);

            if (actionResult != null)
            {
                return actionResult;
            }

            return View(model);
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [Route("/ClaimAccount/LinkDlsAccount")]
        [HttpPost]
        public IActionResult LinkDlsAccountPost(string email, string code)
        {
            var loggedInUserId = User.GetUserIdKnownNotNull();
            var model = GetViewModelIfValidParameters(email, code, loggedInUserId);
            var actionResult = ValidateClaimAccountViewModelForLinkingAccounts(loggedInUserId, model);

            if (actionResult != null)
            {
                return actionResult;
            }

            claimAccountService.LinkAccount(model!.UserId, loggedInUserId, model.CentreId);

            return RedirectToAction("AccountsLinked", new { centreName = model.CentreName });
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [HttpGet]
        public IActionResult AccountsLinked(string centreName)
        {
            var model = new ClaimAccountViewModel { CentreName = centreName };
            return View(model);
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [HttpGet]
        public IActionResult WrongUser(string email, string centreName)
        {
            var model = new ClaimAccountViewModel { Email = email, CentreName = centreName };
            return View(model);
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [HttpGet]
        public IActionResult AccountAlreadyExists(string email, string centreName)
        {
            var model = new ClaimAccountViewModel { Email = email, CentreName = centreName };
            return View(model);
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [HttpGet]
        public IActionResult AdminAccountAlreadyExists(string email, string centreName)
        {
            var model = new ClaimAccountViewModel { Email = email, CentreName = centreName };
            return View(model);
        }

        private ClaimAccountViewModel? GetViewModelIfValidParameters(
            string? email,
            string? code,
            int? loggedInUserId = null
        )
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var (userId, centreId, centreName) =
                userService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(email, code);

            if (userId == null)
            {
                return null;
            }

            var model = claimAccountService.GetAccountDetailsForClaimAccount(
                userId.Value,
                centreId.Value,
                centreName,
                email,
                loggedInUserId
            );

            model.RegistrationConfirmationHash = code;

            return model;
        }

        private IActionResult? ValidateClaimAccountViewModelForLinkingAccounts(
            int loggedInUserId,
            ClaimAccountViewModel? model
        )
        {
            if (model == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var adminAccounts = userService.GetUserById(loggedInUserId)!.AdminAccounts;

            if (adminAccounts.Any())
            {
                return RedirectToAction(
                    "AdminAccountAlreadyExists",
                    new { email = model.Email, centreName = model.CentreName }
                );
            }

            var delegateAccounts = userService.GetUserById(loggedInUserId)!.DelegateAccounts;

            if (delegateAccounts.Any(account => account.CentreId == model.CentreId))
            {
                return RedirectToAction(
                    "AccountAlreadyExists",
                    new { email = model.Email, centreName = model.CentreName }
                );
            }

            if (model.IdOfUserMatchingEmailIfAny != null && model.IdOfUserMatchingEmailIfAny != loggedInUserId)
            {
                return RedirectToAction("WrongUser", new { email = model.Email, centreName = model.CentreName });
            }

            return null;
        }

        private static ClaimAccountCompleteRegistrationViewModel GetClaimAccountCompleteRegistrationViewModel(
            ClaimAccountViewModel model
        )
        {
            return new ClaimAccountCompleteRegistrationViewModel
            {
                Email = model.Email,
                Code = model.RegistrationConfirmationHash,
                CentreName = model.CentreName,
                WasPasswordSetByAdmin = model.WasPasswordSetByAdmin,
            };
        }
    }
}
