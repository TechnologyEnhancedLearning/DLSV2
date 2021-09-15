namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Route("/{application}/MyAccount", Order = 1)]
    [Route("/MyAccount", Order = 2)]
    [ValidateAllowedApplicationType]
    [Authorize]
    public class MyAccountController : Controller
    {
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly ICentreCustomPromptsService centreCustomPromptsService;
        private readonly IImageResizeService imageResizeService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserService userService;

        public MyAccountController(
            ICentreCustomPromptsService centreCustomPromptsService,
            IUserService userService,
            IImageResizeService imageResizeService,
            IJobGroupsDataService jobGroupsDataService,
            CentreCustomPromptHelper customPromptHelper
        )
        {
            this.centreCustomPromptsService = centreCustomPromptsService;
            this.userService = userService;
            this.imageResizeService = imageResizeService;
            this.jobGroupsDataService = jobGroupsDataService;
            centreCustomPromptHelper = customPromptHelper;
        }

        [NoCaching]
        public IActionResult Index(ApplicationType application)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var customPrompts =
                centreCustomPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(
                    User.GetCentreId(),
                    delegateUser
                );

            var model = new MyAccountViewModel(adminUser, delegateUser, customPrompts, application);

            return View(model);
        }

        [NoCaching]
        [HttpGet("EditDetails")]
        public IActionResult EditDetails(ApplicationType application)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            ViewBag.JobGroupOptions =
                SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, delegateUser?.JobGroupName);
            ViewBag.CustomFields = GetCustomFieldsWithDelegateAnswers(delegateUser);

            var formData = new EditDetailsFormData(adminUser, delegateUser, jobGroups);
            var model = new EditDetailsViewModel(formData, application);

            return View(model);
        }

        [NoCaching]
        [HttpPost("EditDetails")]
        public IActionResult EditDetails(EditDetailsFormData formData, string action, ApplicationType application)
        {
            ViewBag.JobGroupOptions = GetJobGroupItems(formData.JobGroupId);
            ViewBag.CustomFields = GetCustomFieldsWithEnteredAnswers(formData);
            return action switch
            {
                "save" => EditDetailsPostSave(formData, application),
                "previewImage" => EditDetailsPostPreviewImage(formData, application),
                "removeImage" => EditDetailsPostRemoveImage(formData, application),
                _ => new StatusCodeResult(500)
            };
        }

        private IActionResult EditDetailsPostSave(EditDetailsFormData formData, ApplicationType application)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();

            if (userDelegateId.HasValue)
            {
                ValidateJobGroup(formData);
                ValidateCustomPrompts(formData);
            }

            if (formData.ProfileImageFile != null)
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.ProfileImageFile),
                    "Preview your new profile picture before saving"
                );
            }

            if (formData.Password != null &&
                !userService.IsPasswordValid(userAdminId, userDelegateId, formData.Password))
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.Password),
                    CommonValidationErrorMessages.IncorrectPassword
                );
            }

            if (!ModelState.IsValid)
            {
                var model = new EditDetailsViewModel(formData, application);
                return View(model);
            }

            if (!userService.NewEmailAddressIsValid(formData.Email!, userAdminId, userDelegateId, User.GetCentreId()))
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.Email),
                    "A user with this email address is already registered at this centre"
                );
                var model = new EditDetailsViewModel(formData, application);
                return View(model);
            }

            var (accountDetailsData, centreAnswersData) = MapToUpdateAccountData(formData, userAdminId, userDelegateId);

            userService.UpdateUserAccountDetails(accountDetailsData, centreAnswersData);

            return RedirectToAction("Index", new { application = application.UrlSnippet });
        }

        private IActionResult EditDetailsPostPreviewImage(EditDetailsFormData formData, ApplicationType application)
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearErrorsForAllFieldsExcept(nameof(EditDetailsFormData.ProfileImageFile));
            var model = new EditDetailsViewModel(formData, application);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (formData.ProfileImageFile != null)
            {
                ModelState.Remove(nameof(EditDetailsFormData.ProfileImage));
                formData.ProfileImage = imageResizeService.ResizeProfilePicture(formData.ProfileImageFile);
            }

            return View(model);
        }

        private IActionResult EditDetailsPostRemoveImage(EditDetailsFormData formData, ApplicationType application)
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearAllErrors();

            ModelState.Remove(nameof(EditDetailsFormData.ProfileImage));
            formData.ProfileImage = null;

            var model = new EditDetailsViewModel(formData, application);
            return View(model);
        }

        private IEnumerable<SelectListItem> GetJobGroupItems(int? selectedId)
        {
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            return SelectListHelper.MapOptionsToSelectListItems(jobGroups, selectedId);
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithEnteredAnswers(EditDetailsFormData formData)
        {
            return centreCustomPromptHelper.GetEditCustomFieldViewModelsForCentre(
                User.GetCentreId(),
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6
            );
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsWithDelegateAnswers(DelegateUser? delegateUser)
        {
            return centreCustomPromptHelper.GetEditCustomFieldViewModelsForCentre(
                User.GetCentreId(),
                delegateUser?.Answer1,
                delegateUser?.Answer2,
                delegateUser?.Answer3,
                delegateUser?.Answer4,
                delegateUser?.Answer5,
                delegateUser?.Answer6
            );
        }

        private void ValidateJobGroup(EditDetailsFormData formData)
        {
            if (!formData.JobGroupId.HasValue)
            {
                ModelState.AddModelError(nameof(EditDetailsFormData.JobGroupId), "Select a job group");
            }
        }

        private void ValidateCustomPrompts(EditDetailsFormData formData)
        {
            centreCustomPromptHelper.ValidateCustomPrompts(
                User.GetCentreId(),
                formData.Answer1,
                formData.Answer2,
                formData.Answer3,
                formData.Answer4,
                formData.Answer5,
                formData.Answer6,
                ModelState
            );
        }

        private (AccountDetailsData, CentreAnswersData?) MapToUpdateAccountData(
            EditDetailsFormData formData,
            int? userAdminId,
            int? userDelegateId
        )
        {
            var accountDetailsData = new AccountDetailsData(
                userAdminId,
                userDelegateId,
                formData.Password!,
                formData.FirstName!,
                formData.LastName!,
                formData.Email!,
                formData.ProfileImage
            );

            var centreAnswersData = userDelegateId == null
                ? null
                : new CentreAnswersData(
                    User.GetCentreId(),
                    formData.JobGroupId!.Value,
                    formData.Answer1,
                    formData.Answer2,
                    formData.Answer3,
                    formData.Answer4,
                    formData.Answer5,
                    formData.Answer6
                );
            return (accountDetailsData, centreAnswersData);
        }
    }
}
