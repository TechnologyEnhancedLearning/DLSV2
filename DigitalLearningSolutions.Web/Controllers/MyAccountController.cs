namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
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
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Route("/{dlsSubApplication}/MyAccount", Order = 1)]
    [Route("/MyAccount", Order = 2)]
    [ValidateAllowedDlsSubApplication]
    [SetDlsSubApplication(determiningRouteParameter: "dlsSubApplication")]
    [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
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
        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var customPrompts =
                centreCustomPromptsService.GetCentreCustomPromptsWithAnswersByCentreIdAndDelegateUser(
                    User.GetCentreId(),
                    delegateUser
                );

            var model = new MyAccountViewModel(adminUser, delegateUser, customPrompts, dlsSubApplication);

            return View(model);
        }

        [NoCaching]
        [HttpGet("EditDetails")]
        public IActionResult EditDetails(DlsSubApplication dlsSubApplication)
        {
            var userAdminId = User.GetAdminId();
            var userDelegateId = User.GetCandidateId();
            var (adminUser, delegateUser) = userService.GetUsersById(userAdminId, userDelegateId);

            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical().ToList();
            ViewBag.JobGroupOptions =
                SelectListHelper.MapOptionsToSelectListItemsWithSelectedText(jobGroups, delegateUser?.JobGroupName);
            ViewBag.CustomFields = GetCustomFieldsWithDelegateAnswers(delegateUser);

            var model = new EditDetailsViewModel(adminUser, delegateUser, jobGroups, dlsSubApplication);

            return View(model);
        }

        [NoCaching]
        [HttpPost("EditDetails")]
        public IActionResult EditDetails(
            EditDetailsFormData formData,
            string action,
            DlsSubApplication dlsSubApplication
        )
        {
            ViewBag.JobGroupOptions = GetJobGroupItems(formData.JobGroupId);
            ViewBag.CustomFields = GetCustomFieldsWithEnteredAnswers(formData);
            return action switch
            {
                "save" => EditDetailsPostSave(formData, dlsSubApplication),
                "previewImage" => EditDetailsPostPreviewImage(formData, dlsSubApplication),
                "removeImage" => EditDetailsPostRemoveImage(formData, dlsSubApplication),
                _ => new StatusCodeResult(500),
            };
        }

        private IActionResult EditDetailsPostSave(EditDetailsFormData formData, DlsSubApplication dlsSubApplication)
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
                var model = new EditDetailsViewModel(formData, dlsSubApplication);
                var validationResults = model.Validate(new ValidationContext(model, null, null));
                foreach (var result in validationResults)
                {
                    var key = string.Join("", result.MemberNames);
                    if (ModelState.TryGetValue(key, out var value))
                    {
                        if (value.ValidationState == ModelValidationState.Invalid)
                        {
                            continue;
                        }
                    }

                    ModelState.TryAddModelError(
                        key,
                        result.ErrorMessage
                    );
                }

                return View(model);
            }

            if (!userService.NewEmailAddressIsValid(formData.Email!, userAdminId, userDelegateId, User.GetCentreId()))
            {
                ModelState.AddModelError(
                    nameof(EditDetailsFormData.Email),
                    "A user with this email address is already registered at this centre"
                );
                var model = new EditDetailsViewModel(formData, dlsSubApplication);
                return View(model);
            }

            var (accountDetailsData, centreAnswersData) = MapToUpdateAccountData(formData, userAdminId, userDelegateId);
            userService.UpdateUserAccountDetails(accountDetailsData, centreAnswersData);

            return RedirectToAction("Index", new { application = dlsSubApplication.UrlSegment });
        }

        private IActionResult EditDetailsPostPreviewImage(
            EditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearErrorsForAllFieldsExcept(nameof(EditDetailsFormData.ProfileImageFile));

            if (!ModelState.IsValid)
            {
                return View(new EditDetailsViewModel(formData, dlsSubApplication));
            }

            if (formData.ProfileImageFile != null)
            {
                ModelState.Remove(nameof(EditDetailsFormData.ProfileImage));
                formData.ProfileImage = imageResizeService.ResizeProfilePicture(formData.ProfileImageFile);
            }

            var model = new EditDetailsViewModel(formData, dlsSubApplication);
            return View(model);
        }

        private IActionResult EditDetailsPostRemoveImage(
            EditDetailsFormData formData,
            DlsSubApplication dlsSubApplication
        )
        {
            // We don't want to display validation errors on other fields in this case
            ModelState.ClearAllErrors();

            ModelState.Remove(nameof(EditDetailsFormData.ProfileImage));
            formData.ProfileImage = null;

            var model = new EditDetailsViewModel(formData, dlsSubApplication);
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
                formData.ProfessionalRegistrationNumber,
                formData.ProfessionalRegNumberSelectionAnswer,
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
