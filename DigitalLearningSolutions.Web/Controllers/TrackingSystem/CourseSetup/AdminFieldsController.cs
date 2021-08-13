namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup")]
    public class AdminFieldsController : Controller
    {
        public const string DeleteAction = "delete";
        public const string AddPromptAction = "addPrompt";
        public const string SaveAction = "save";
        public const string BulkAction = "bulk";
        private const string EditPromptCookieName = "EditRegistrationPromptData";
        private static readonly DateTimeOffset CookieExpiry = DateTimeOffset.UtcNow.AddDays(7);
        private readonly ICustomPromptsService customPromptsService;

        public AdminFieldsController(ICustomPromptsService customPromptsService)
        {
            this.customPromptsService = customPromptsService;
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields")]
        public IActionResult AdminFields(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseCustomPrompts = customPromptsService.GetCustomPromptsForCourse(
                customisationId,
                centreId,
                categoryId.Value
            );

            if (courseCustomPrompts == null)
            {
                return NotFound();
            }

            var model = new AdminFieldsViewModel(courseCustomPrompts.CourseAdminFields, customisationId);
            return View(model);
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields/{promptNumber:int}/Edit/Start")]
        public IActionResult EditAdminFieldStart(int customisationId, int promptNumber)
        {
            TempData.Clear();

            return RedirectToAction("EditAdminField", new { customisationId, promptNumber });
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields/{promptNumber:int}/Edit")]
        public IActionResult EditAdminField(int customisationId, int promptNumber)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var courseCustomPrompt = customPromptsService.GetCustomPromptsForCourse(
                    customisationId,
                    centreId,
                    categoryId.Value
                ).CourseAdminFields
                .Single(cp => cp.CustomPromptNumber == promptNumber);

            var data = TempData.Get<EditAdminFieldData>();

            var model = data?.EditModel ?? new EditAdminFieldViewModel(courseCustomPrompt, customisationId);

            return View(model);
        }

        [HttpPost]
        [Route("{customisationId}/AdminFields/{promptNumber:int}/Edit")]
        public IActionResult EditAdminField(EditAdminFieldViewModel model, string action)
        {
            if (action.StartsWith(DeleteAction) && TryGetAnswerIndexFromDeleteAction(action, out var index))
            {
                return AdminFieldAnswersPostRemovePrompt(model, index);
            }

            return action switch
            {
                SaveAction => EditAdminFieldPostSave(model),
                AddPromptAction => AdminFieldAnswersPostAddPrompt(model, true),
                BulkAction => EditAdminFieldBulk(model),
                _ => new StatusCodeResult(500)
            };
        }

        [HttpGet]
        [Route("{customisationId}/AdminFields/{promptNumber:int}/Edit/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionData<EditAdminFieldData>))]
        public IActionResult EditAdminFieldBulk(int customisationId, int promptNumber)
        {
            var data = TempData.Peek<EditAdminFieldData>()!;

            var model = new BulkAdminFieldAnswersViewModel(
                data.EditModel.OptionsString,
                false,
                customisationId,
                promptNumber
            );

            return View("BulkAdminFieldAnswers", model);
        }

        [HttpPost]
        [Route("AdminFieldsEdit/Bulk")]
        [ServiceFilter(typeof(RedirectEmptySessionData<EditAdminFieldData>))]
        public IActionResult EditAdminFieldBulkPost(
            BulkAdminFieldAnswersViewModel model
        )
        {
            ValidateBulkOptionsString(model.OptionsString);
            if (!ModelState.IsValid)
            {
                return View("BulkAdminFieldAnswers", model);
            }

            var editData = TempData.Peek<EditAdminFieldData>()!;
            editData.EditModel!.OptionsString =
                NewlineSeparatedStringListHelper.RemoveEmptyOptions(model.OptionsString);
            TempData.Set(editData);

            return RedirectToAction(
                "EditAdminField",
                new { customisationId = model.CustomisationId, promptNumber = model.PromptNumber }
            );
        }

        private IActionResult EditAdminFieldPostSave(EditAdminFieldViewModel model)
        {
            ModelState.ClearAllErrors();

            customPromptsService.UpdateCustomPromptForCourse(
                model.CustomisationId,
                model.PromptNumber,
                model.Mandatory,
                model.OptionsString
            );

            return RedirectToAction("AdminFields", new { customisationId = model.CustomisationId });
        }

        private IActionResult EditAdminFieldBulk(EditAdminFieldViewModel model)
        {
            SetEditAdminFieldTempData(model);

            return RedirectToAction(
                "EditAdminFieldBulk",
                new { customisationId = model.CustomisationId, promptNumber = model.PromptNumber }
            );
        }

        private void SetEditAdminFieldTempData(EditAdminFieldViewModel model)
        {
            var data = new EditAdminFieldData(model);
            var id = data.Id;

            Response.Cookies.Append(
                EditPromptCookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = CookieExpiry
                }
            );
            TempData.Set(data);
        }

        private IActionResult AdminFieldAnswersPostAddPrompt(
            AdminFieldAnswersViewModel model,
            bool saveToTempData = false
        )
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var optionsString =
                NewlineSeparatedStringListHelper.AddStringToNewlineSeparatedList(model.OptionsString, model.Answer!);

            if (optionsString.Length > 1000)
            {
                SetTotalAnswersLengthTooLongError(model);
                return View(model);
            }

            SetAdminFieldAnswersViewModelOptions(model, optionsString);

            return View(model);
        }

        private IActionResult AdminFieldAnswersPostRemovePrompt(
            AdminFieldAnswersViewModel model,
            int index
        )
        {
            ModelState.ClearAllErrors();

            var optionsString =
                NewlineSeparatedStringListHelper.RemoveStringFromNewlineSeparatedList(model.OptionsString!, index);

            SetAdminFieldAnswersViewModelOptions(model, optionsString);

            return View(model);
        }

        private void SetAdminFieldAnswersViewModelOptions(
            AdminFieldAnswersViewModel model,
            string optionsString
        )
        {
            ModelState.Remove(nameof(AdminFieldAnswersViewModel.OptionsString));
            model.OptionsString = optionsString;

            ModelState.Remove(nameof(AdminFieldAnswersViewModel.Answer));
            model.Answer = null;
        }

        private void SetTotalAnswersLengthTooLongError(AdminFieldAnswersViewModel model)
        {
            if (model.OptionsString == null || model.OptionsString.Length < 2)
            {
                return;
            }

            var remainingLength = 1000 - (model.OptionsString?.Length - 2 ?? 0);
            ModelState.AddModelError(
                nameof(AdminFieldAnswersViewModel.Answer),
                $"The complete list of answers must be 1000 characters or fewer ({remainingLength} characters remaining for the new answer)"
            );
        }

        private static bool TryGetAnswerIndexFromDeleteAction(string action, out int index)
        {
            return int.TryParse(action.Remove(0, DeleteAction.Length), out index);
        }

        private void ValidateBulkOptionsString(string? optionsString)
        {
            if (optionsString != null && optionsString.Length > 1000)
            {
                ModelState.AddModelError(
                    nameof(BulkAdminFieldAnswersViewModel.OptionsString),
                    "The complete list of answers must be 1000 characters or fewer"
                );
            }

            var optionsList = NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(optionsString);
            if (optionsList.Any(o => o.Length > 100))
            {
                ModelState.AddModelError(
                    nameof(BulkAdminFieldAnswersViewModel.OptionsString),
                    "Each answer must be 100 characters or fewer"
                );
            }
        }
    }
}
