namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
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

            TempData.Set(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteRegistration()
        {
            var model = TempData.Peek<ClaimAccountViewModel>()!;

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    "LinkDlsAccount",
                    new { email = model.Email, code = model.RegistrationConfirmationHash }
                );
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

            TempData.Clear();

            return View("Confirmation", model);
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [HttpGet]
        public IActionResult LinkDlsAccount(string email, string code)
        {
            var loggedInUserId = User.GetUserIdKnownNotNull();
            var model = GetViewModelIfValidParameters(email, code, loggedInUserId);
            var actionResult = ValidateClaimAccountViewModelForLinkingAccounts(loggedInUserId, model);

            TempData.Set(model);

            if (actionResult != null)
            {
                return actionResult;
            }

            return View(model);
        }

        [Authorize(Policy = CustomPolicies.BasicUser)]
        [HttpPost]
        public IActionResult LinkDlsAccount()
        {
            var model = TempData.Peek<ClaimAccountViewModel>()!;
            var loggedInUserId = User.GetUserIdKnownNotNull();
            var actionResult = ValidateClaimAccountViewModelForLinkingAccounts(loggedInUserId, model);

            if (actionResult != null)
            {
                return actionResult;
            }

            claimAccountService.LinkAccount(
                model.UserId,
                loggedInUserId,
                model.CentreId
            );

            TempData.Clear();

            return View("AccountsLinked", model);
        }

        [HttpGet]
        public IActionResult WrongUser()
        {
            var model = TempData.Peek<ClaimAccountViewModel>();

            TempData.Clear();

            return model == null ? NotFound() as IActionResult : View(model);
        }

        [HttpGet]
        public IActionResult AccountAlreadyExists()
        {
            var model = TempData.Peek<ClaimAccountViewModel>();

            TempData.Clear();

            return model == null ? NotFound() as IActionResult : View(model);
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

            var model = claimAccountService.GetAccountDetailsForCompletingRegistration(
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
                TempData.Clear();
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var delegateAccounts = userService.GetUserById(loggedInUserId)!.DelegateAccounts;

            if (delegateAccounts.Any(account => account.CentreId == model.CentreId))
            {
                return RedirectToAction("AccountAlreadyExists");
            }

            if (model.EmailIsTaken)
            {
                return RedirectToAction("WrongUser");
            }

            return null;
        }
    }
}
