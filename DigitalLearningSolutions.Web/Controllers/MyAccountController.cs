namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Mvc;

    public class MyAccountController : Controller
    {
        private readonly IUserService userService;

        public MyAccountController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            var userAdminId = User.GetCustomClaim(CustomClaimTypes.UserAdminId);
            var userDelegateId = User.GetCustomClaim(CustomClaimTypes.LearnCandidateId);
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var model = new MyAccountViewModel(adminUser, delegateUser);

            return View(model);
        }
    }
}
