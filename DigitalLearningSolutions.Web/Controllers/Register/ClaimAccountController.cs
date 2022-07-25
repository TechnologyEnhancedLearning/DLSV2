namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class ClaimAccountController : Controller
    {
        private readonly IUserService userService;
        private readonly IUserDataService userDataService;
        private readonly IClaimAccountService claimAccountService;

        public ClaimAccountController(
            IUserService userService,
            IUserDataService userDataService,
            IClaimAccountService claimAccountService
        )
        {
            this.userService = userService;
            this.userDataService = userDataService;
            this.claimAccountService = claimAccountService;
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

            return View(model);
        }

        [Route("/ClaimAccount/CompleteRegistration")]
        [HttpPost]
        public IActionResult CompleteRegistrationPost(string email, string code)
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

            if (userDataService.PrimaryEmailIsInUse(model.Email))
            {
                return NotFound();
            }

            if (!model.PasswordSet)
            {
                // TODO HEEDLS-975 Redirect to SetPassword
                return NotFound();
            }

            claimAccountService.ConvertTemporaryUserToConfirmedUser(
                model.UserId,
                model.CentreId,
                model.Email
            );

            return RedirectToAction(
                "Confirmation",
                new
                {
                    email = model.Email,
                    centreName = model.CentreName,
                    candidateNumber = model.CandidateNumber
                }
            );
        }

        [HttpGet]
        public IActionResult Confirmation(string email, string centreName, string candidateNumber)
        {
            var model = new ClaimAccountViewModel
            {
                Email = email,
                CentreName = centreName,
                CandidateNumber = candidateNumber,
            };

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

        private ClaimAccountViewModel? GetViewModelIfValidParameters(
            string email,
            string code,
            int? loggedInUserId = null
        )
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var (userId, centreId, centreName) =
                userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(email, code);

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
    }
}
