namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
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
            return action switch
            {
                "save" => EditDetailsPostSave(model),
                "previewImage" => EditDetailsPostPreviewImage(model),
                "removeImage" => EditDetailsPostRemoveImage(model),
                _ => View(model),
            };
        }

        private IActionResult EditDetailsPostSave(EditDetailsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ProfilePicture != null)
            {
                ModelState.AddModelError("ProfilePicture", "Preview your new profile picture before saving");
                return View(model);
            }

            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetNullableCandidateId();

            var profileImageToSave = model.HasProfileImageBeenRemoved ? null : model.NewProfileImage ?? model.CurrentProfileImage;

            if (!userService.TryUpdateUserAccountDetails(userAdminId, userDelegateId, model.Password,  model.FirstName, model.LastName, model.Email, profileImageToSave))
            {
                ModelState.AddModelError("Password", "The password you have entered is incorrect.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        private IActionResult EditDetailsPostPreviewImage(EditDetailsViewModel model)
        {
            // We don't want to display validation errors on other fields in this case
            foreach (var key in ModelState.Keys.Where(k => k != "ProfilePicture"))
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Remove the old new profile image value from the ModelState
            ModelState.Remove("NewProfileImage");
            if (model.ProfilePicture != null)
            {
                model.NewProfileImage = imageResizeService.ResizeProfilePicture(model.ProfilePicture);
            }

            return View(model);
        }

        private IActionResult EditDetailsPostRemoveImage(EditDetailsViewModel model)
        {
            // We don't want to display validation errors on other fields in this case
            foreach (var key in ModelState.Keys)
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }
            
            ModelState.Remove("HasProfileImageBeenRemoved");
            model.ProfilePicture = null;
            model.NewProfileImage = null;
            model.HasProfileImageBeenRemoved = true;

            return View(model);
        }
    }
}
