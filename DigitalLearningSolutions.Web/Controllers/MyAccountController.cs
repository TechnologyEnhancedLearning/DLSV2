namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly IUserService userService;
        private readonly ICustomPromptsService customPromptsService;
        private readonly IImageResizeService imageResizeService;

        public MyAccountController(
            ICustomPromptsService customPromptsService,
            IUserService userService,
            IImageResizeService imageResizeService)
        {
            this.customPromptsService = customPromptsService;
            this.userService = userService;
            this.imageResizeService = imageResizeService;
        }

        public IActionResult Index()
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetNullableCandidateId();
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

            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetNullableCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var model = new EditDetailsViewModel(adminUser, delegateUser);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditDetails(EditDetailsViewModel model, string action)
        {
            switch (action)
            {
                case "save":
                    return EditDetailsPostSave(model);
                case "previewImage":
                    return EditDetailsPostPreviewImage(model);
                case "removeImage":
                    return EditDetailsPostRemoveImage(model);
                default:
                    return View(model);
            }
        }

        private IActionResult EditDetailsPostSave(EditDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetNullableCandidateId();

            if (!userService.TryUpdateUserAccountDetails(userAdminId, userDelegateId, model.Password,  model.FirstName, model.LastName, model.Email))
            {
                ModelState.AddModelError("Password", "The password you have entered is incorrect.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        private IActionResult EditDetailsPostPreviewImage(EditDetailsViewModel model)
        {
            // We don't want to display errors in this case
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            if (model.ProfilePicture != null)
            {
                model.ProfileImage = imageResizeService.ResizeProfilePicture(model.ProfilePicture);
            }

            return View(model);
        }

        private IActionResult EditDetailsPostRemoveImage(EditDetailsViewModel model)
        {
            // We don't want to display errors in this case
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            model.ProfilePicture = null;
            model.ProfileImage = null;

            return View(model);
        }
    }
}
