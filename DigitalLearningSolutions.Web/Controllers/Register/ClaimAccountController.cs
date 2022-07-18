namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class ClaimAccountController : Controller
    {
        private readonly IUserDataService userDataService;
        private readonly IClaimAccountService claimAccountService;

        public ClaimAccountController(
            IUserDataService userDataService,
            IClaimAccountService claimAccountService
        )
        {
            this.userDataService = userDataService;
            this.claimAccountService = claimAccountService;
        }

        [HttpGet]
        [Route("/ClaimAccount/CompleteRegistration")]
        public IActionResult CompleteRegistration(string email, string code)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var (userId, centreId, centreName) =
                userDataService.GetUserIdAndCentreForCentreEmailRegistrationConfirmationHashPair(email, code);

            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var model = claimAccountService.CreateModelForCompleteRegistration(
                userId.Value,
                centreId.Value,
                centreName,
                email
            );

            model.RegistrationConfirmationHash = code;
            TempData.Set(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult CompleteRegistration()
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

            claimAccountService.ConvertTemporaryUserToConfirmedUser(
                model.UserId,
                model.CentreId,
                model.CentreSpecificEmail
            );

            TempData.Clear();
            return View("Confirmation", model);
        }
    }
}
