namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly IUserService userService;
        private readonly ICustomPromptsService customPromptsService;

        public MyAccountController(ICustomPromptsService customPromptsService, IUserService userService)
        {
            this.customPromptsService = customPromptsService;
            this.userService = userService;
        }

        public IActionResult Index()
        {
            var userAdminId = User.GetCustomClaim(CustomClaimTypes.UserAdminId);
            var userDelegateId = User.GetCustomClaim(CustomClaimTypes.LearnCandidateId);
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var customPrompts = customPromptsService.GetCustomPromptsForCentreByCentreId(delegateUser?.CentreId);

            var model = new MyAccountViewModel(adminUser, delegateUser, customPrompts);

            return View(model);
        }

        [HttpGet]
        public IActionResult EditDetails()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            var userAdminId = User.GetCustomClaim(CustomClaimTypes.UserAdminId);
            var userDelegateId = User.GetCustomClaim(CustomClaimTypes.LearnCandidateId);
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var model = new EditDetailsViewModel(adminUser, delegateUser);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditDetails(EditDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userAdminId = User.GetCustomClaim(CustomClaimTypes.UserAdminId);
            var userDelegateId = User.GetCustomClaim(CustomClaimTypes.LearnCandidateId);
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            if (adminUser != null)
            {
                adminUser.FirstName = model.FirstName;
                adminUser.LastName = model.LastName;
                adminUser.EmailAddress = model.Email;
            }
            
            if (delegateUser != null)
            {
                delegateUser.FirstName = model.FirstName;
                delegateUser.LastName = model.LastName;
                delegateUser.EmailAddress = model.Email;
            }

            if (!userService.TryUpdateUsers(adminUser, delegateUser, model.Password))
            {
                ModelState.AddModelError("Password", "The password you have entered is incorrect.");
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
