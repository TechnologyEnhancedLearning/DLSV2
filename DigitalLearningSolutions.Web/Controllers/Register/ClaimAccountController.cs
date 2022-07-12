namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class ClaimAccountController : Controller
    {
        private readonly IUserDataService userDataService;

        public ClaimAccountController(IUserDataService userDataService)
        {
            this.userDataService = userDataService;
        }

        [HttpGet]
        [Route("/ClaimAccount/CompleteRegistration")]
        public IActionResult CompleteRegistration(string? email = null, string? code = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var (userId, centreName) =
                userDataService.GetUserIdAndCentreNameForCentreEmailRegistrationConfirmationHashPair(email, code);

            if (userId == null || centreName == null)
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var userAccount = userDataService.GetUserAccountByEmailAddress(email);
            var model = new ClaimAccountViewModel
            {
                CentreName = centreName,
                CentreSpecificEmail = email,
                UserExists = userAccount != null,
                UserActive = userAccount?.Active ?? false,
            };
            return RedirectToAction("AccessDenied", "LearningSolutions");
        }
    }
}
