namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly CustomPromptHelper customPromptHelper;
        private readonly ICustomPromptsService customPromptsService;
        private readonly IImageResizeService imageResizeService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserService userService;

        public MyAccountController(
            ICustomPromptsService customPromptsService,
            IUserService userService,
            IImageResizeService imageResizeService,
            IJobGroupsDataService jobGroupsDataService,
            CustomPromptHelper customPromptHelper)
        {
            this.customPromptsService = customPromptsService;
            this.userService = userService;
            this.imageResizeService = imageResizeService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.customPromptHelper = customPromptHelper;
        }

        public IActionResult Index()
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetNullableCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var customPrompts =
                customPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(User.GetCentreId(),
                    delegateUser);

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

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            ViewBag.JobGroupOptions =
                SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, delegateUser?.JobGroupName);
            ViewBag.CustomFields = GetCustomFieldsWithDelegateAnswers(delegateUser);

            var model = new EditDetailsViewModel(adminUser, delegateUser, jobGroups);

            return View(model);
        }

        [HttpPost]
        public IActionResult EditDetails(EditDetailsViewModel model, string action)
        {
            ViewBag.JobGroupOptions = GetJobGroupItems(model.JobGroupId);
            ViewBag.CustomFields = GetCustomFieldsWithEnteredAnswers(model);
            return action switch
            {
                "save" => EditDetailsPostSave(model),
                "previewImage" => EditDetailsPostPreviewImage(model),
                "removeImage" => EditDetailsPostRemoveImage(model),
                _ => View(model)
            };
        }

        private IActionResult EditDetailsPostSave(EditDetailsViewModel model)
        {
            ValidateCustomPrompts(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ProfileImageFile != null)
            {
                ModelState.AddModelError(nameof(EditDetailsViewModel.ProfileImageFile),
                    "Preview your new profile picture before saving");
                return View(model);
            }

            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetNullableCandidateId();

            var accountDetailsData = new AccountDetailsData(userAdminId,
                userDelegateId,
                model.Password!,
                model.FirstName!,
                model.LastName!,
                model.Email!,
                model.ProfileImage);

            var centreAnswersData = userDelegateId == null
                ? null
                : new CentreAnswersData(
                    User.GetCentreId(),
                    model.JobGroupId!.Value,
                    model.Answer1,
                    model.Answer2,
                    model.Answer3,
                    model.Answer4,
                    model.Answer5,
                    model.Answer6);

            if (!userService.TryUpdateUserAccountDetails(accountDetailsData, centreAnswersData))
            {
                ModelState.AddModelError(nameof(EditDetailsViewModel.Password),
                    "The password you have entered is incorrect.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        private IActionResult EditDetailsPostPreviewImage(EditDetailsViewModel model)
        {
            // We don't want to display validation errors on other fields in this case
            foreach (var key in ModelState.Keys.Where(k => k != nameof(EditDetailsViewModel.ProfileImageFile)))
            {
                ModelState[key].Errors.Clear();
                ModelState[key].ValidationState = ModelValidationState.Valid;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ProfileImageFile != null)
            {
                ModelState.Remove(nameof(EditDetailsViewModel.ProfileImage));
                model.ProfileImage = imageResizeService.ResizeProfilePicture(model.ProfileImageFile);
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

            ModelState.Remove(nameof(EditDetailsViewModel.ProfileImage));
            model.ProfileImage = null;
            return View(model);
        }

        private IEnumerable<SelectListItem> GetJobGroupItems(int? selectedId)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            return SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue(jobGroups, selectedId);
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithEnteredAnswers(EditDetailsViewModel model)
        {
            return customPromptHelper.GetCustomFieldViewModelsForCentre(User.GetCentreId(),
                model.Answer1, model.Answer2, model.Answer3, model.Answer4,
                model.Answer5, model.Answer6);
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithDelegateAnswers(DelegateUser? delegateUser)
        {
            return customPromptHelper.GetCustomFieldViewModelsForCentre(User.GetCentreId(),
                delegateUser?.Answer1, delegateUser?.Answer2, delegateUser?.Answer3, delegateUser?.Answer4,
                delegateUser?.Answer5, delegateUser?.Answer6);
        }

        private void ValidateCustomPrompts(EditDetailsViewModel model)
        {
            var customFields = GetCustomFieldsWithEnteredAnswers(model);

            foreach (var customField in customFields)
            {
                if (customField.Mandatory && customField.Answer == null)
                {
                    var errorMessage = $"{(customField.Options.Any() ? "Select" : "Enter")} a {customField.CustomPrompt.ToLower()}";
                    ModelState.AddModelError("Answer" + customField.CustomFieldId, errorMessage);
                }

                if (customField.Answer?.Length > 100)
                {
                    var errorMessage = $"{customField.CustomPrompt} must be at most 100 characters";
                    ModelState.AddModelError("Answer" + customField.CustomFieldId, errorMessage);
                }
            }
        }
    }
}
