﻿namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
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
            var userDelegateId = User.GetCandidateId();
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
            var userDelegateId = User.GetCandidateId();
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
                _ => RedirectToAction("Error", "LearningSolutions")
            };
        }

        private IActionResult EditDetailsPostSave(EditDetailsViewModel model)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();

            if (userDelegateId.HasValue)
            {
                ValidateJobGroup(model);
                ValidateCustomPrompts(model);
            }

            if (model.ProfileImageFile != null)
            {
                ModelState.AddModelError(nameof(EditDetailsViewModel.ProfileImageFile),
                    "Preview your new profile picture before saving");
            }

            if (model.Password != null && !userService.IsPasswordValid(userAdminId, userDelegateId, model.Password))
            {
                ModelState.AddModelError(nameof(EditDetailsViewModel.Password), CommonValidationErrorMessages.IncorrectPassword);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!userService.NewEmailAddressIsValid(model.Email!, userAdminId, userDelegateId, User.GetCentreId()))
            {
                ModelState.AddModelError(nameof(EditDetailsViewModel.Email),
                        "A user with this email address is already registered at this centre");
                return View(model);
            }

            var (accountDetailsData, centreAnswersData) = MapToUpdateAccountData(model, userAdminId, userDelegateId);

            userService.UpdateUserAccountDetails(accountDetailsData, centreAnswersData);

            return RedirectToAction("Index");
        }

        private IActionResult EditDetailsPostPreviewImage(EditDetailsViewModel model)
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearErrorsForAllFieldsExcept(nameof(EditDetailsViewModel.ProfileImageFile));

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
            ModelState.ClearAllErrors();

            ModelState.Remove(nameof(EditDetailsViewModel.ProfileImage));
            model.ProfileImage = null;
            return View(model);
        }

        private IEnumerable<SelectListItem> GetJobGroupItems(int? selectedId)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            return SelectListHelper.MapOptionsToSelectListItems(jobGroups, selectedId);
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithEnteredAnswers(EditDetailsViewModel model)
        {
            return customPromptHelper.GetEditCustomFieldViewModelsForCentre(User.GetCentreId(),
                model.Answer1, model.Answer2, model.Answer3, model.Answer4,
                model.Answer5, model.Answer6);
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithDelegateAnswers(DelegateUser? delegateUser)
        {
            return customPromptHelper.GetEditCustomFieldViewModelsForCentre(User.GetCentreId(),
                delegateUser?.Answer1, delegateUser?.Answer2, delegateUser?.Answer3, delegateUser?.Answer4,
                delegateUser?.Answer5, delegateUser?.Answer6);
        }

        private void ValidateJobGroup(EditDetailsViewModel model)
        {
            if (!model.JobGroupId.HasValue)
            {
                ModelState.AddModelError(nameof(EditDetailsViewModel.JobGroupId), "Select a job group");
            }
        }

        private void ValidateCustomPrompts(EditDetailsViewModel model)
        {
            customPromptHelper.ValidateCustomPrompts(User.GetCentreId(),
                model.Answer1, model.Answer2, model.Answer3, model.Answer4,
                model.Answer5, model.Answer6, ModelState);
        }

        private (AccountDetailsData, CentreAnswersData?) MapToUpdateAccountData(EditDetailsViewModel model, int? userAdminId, int? userDelegateId)
        {
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
            return (accountDetailsData, centreAnswersData);
        }
    }
}
