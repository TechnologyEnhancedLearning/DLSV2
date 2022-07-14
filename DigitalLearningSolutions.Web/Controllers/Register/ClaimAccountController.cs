namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class ClaimAccountController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly IConfigDataService configDataService;

        public ClaimAccountController(IUserDataService userDataService, IConfigDataService configDataService)
        {
            this.userDataService = userDataService;
            this.configDataService = configDataService;
        }

        [HttpGet]
        [Route("/ClaimAccount/CompleteRegistration")]
        public IActionResult CompleteRegistration(string? email = null, string? code = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var (userId, centreId, centreName) =
                userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(email, code);

            if (userId == null || centreName == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var existingUserAccount = userDataService.GetUserAccountByEmailAddress(email);
            var userClaimingAccount = userDataService.GetUserAccountById(userId.Value);
            var delegateAccount = userDataService.GetDelegateAccountsByUserId(userId.Value)
                .SingleOrDefault(da => da.CentreId == centreId);
            var supportEmail = configDataService.GetConfigValue(ConfigDataService.SupportEmail);

            var model = new ClaimAccountViewModel
            {
                CentreName = centreName,
                CentreSpecificEmail = email,
                RegistrationConfirmationHash = code,
                DelegateId = delegateAccount!.CandidateNumber,
                SupportEmail = supportEmail,
                UserExists = existingUserAccount != null,
                UserActive = existingUserAccount?.Active ?? false,
                PasswordSet = !string.IsNullOrWhiteSpace(userClaimingAccount?.PasswordHash),
            };

            TempData.Set(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult Index()
        {
            var model = TempData.Peek<ClaimAccountViewModel>()!;

            if (userDataService.PrimaryEmailIsInUse(model.CentreSpecificEmail))
            {
                return NotFound();
            }

            if (!model.PasswordSet)
            {
                // TODO HEEDLS-975 Redirect to SetPassword
                return NotFound();
            }

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            var model = TempData.Peek<ClaimAccountViewModel>()!;
            return View(model);
        }
    }
}
